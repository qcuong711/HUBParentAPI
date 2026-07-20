using Api.Application;
using Api.Application.Dtos;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Services;

public class StudentQueryService : IStudentQueryService
{
    private readonly AppDbContext _db;

    public StudentQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<StudentRegistrationsDto> GetRegistrationsAsync(
        string studentCode,
        string? yearStudy,
        string? termId)
    {
        var items = await (
            from registration in _db.StudentScheduleStudyUnits.AsNoTracking()
            where registration.StudentID == studentCode
            join scheduledUnit in _db.ScheduleStudyUnits.AsNoTracking()
                on registration.ScheduleStudyUnitID equals scheduledUnit.ScheduleStudyUnitID
            join studyUnit in _db.StudyUnits.AsNoTracking()
                on scheduledUnit.StudyUnitID equals studyUnit.StudyUnitID
            join curriculum in _db.Curriculums.AsNoTracking()
                on studyUnit.CurriculumID equals curriculum.CurriculumID
            join term in _db.Terms.AsNoTracking()
                on new { studyUnit.YearStudy, studyUnit.TermID }
                equals new { term.YearStudy, term.TermID } into termJoin
            from term in termJoin.DefaultIfEmpty()
            where (yearStudy == null || studyUnit.YearStudy == yearStudy)
                && (termId == null || studyUnit.TermID == termId)
            orderby studyUnit.YearStudy, term.OrderTerm, curriculum.CurriculumName
            select new RegisteredCourseDto
            {
                ScheduleStudyUnitID = scheduledUnit.ScheduleStudyUnitID,
                ScheduleStudyUnitAlias = scheduledUnit.ScheduleStudyUnitAlias,
                StudyUnitID = studyUnit.StudyUnitID,
                StudyUnitAlias = studyUnit.StudyUnitAlias,
                CurriculumID = curriculum.CurriculumID,
                CurriculumName = curriculum.CurriculumName,
                Credits = curriculum.Credits,
                TheoryCredits = curriculum.TheoryCredits,
                PracticeCredits = curriculum.PracticeCredits,
                StartDate = scheduledUnit.StartDate,
                EndDate = scheduledUnit.EndDate,
                Status = registration.RegistStatus ?? (int?)scheduledUnit.Status,
                YearStudy = studyUnit.YearStudy,
                TermID = studyUnit.TermID
            })
            .ToListAsync();

        var distinctItems = items
            .GroupBy(x => x.ScheduleStudyUnitID, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToList();

        return new StudentRegistrationsDto
        {
            StudentID = studentCode,
            YearStudy = yearStudy,
            TermID = termId,
            TotalCourses = distinctItems.Count,
            TotalCredits = distinctItems.Sum(x => x.Credits),
            Items = distinctItems
        };
    }

    public async Task<IReadOnlyList<StudentStudyStatusDto>> GetStudyStatusesAsync(
        string studentCode,
        string? yearStudy,
        string? termId)
    {
        var warningHistory = await (
            from result in _db.StudentRegulationResults.AsNoTracking()
            where result.StudentID == studentCode
                && (yearStudy == null || result.YearStudy == yearStudy)
                && (termId == null || result.TermID == termId)
            join status in _db.StudyStatuses.AsNoTracking()
                on result.StudyStatusID equals status.StudyStatusID into statusJoin
            from status in statusJoin.DefaultIfEmpty()
            join term in _db.Terms.AsNoTracking()
                on new { result.YearStudy, result.TermID }
                equals new { term.YearStudy, term.TermID } into termJoin
            from term in termJoin.DefaultIfEmpty()
            orderby result.YearStudy descending, term.OrderTerm descending, result.UpdateDate descending
            select new StudentStudyStatusDto
            {
                StudentID = result.StudentID,
                YearStudy = result.YearStudy,
                TermID = result.TermID,
                StudyProgramID = result.StudyProgramID,
                ClassStudentID = result.ClassStudentID,
                StudyStatusID = result.StudyStatusID,
                StudyStatusName = status.StudyStatusName,
                IsFail = result.IsFail,
                Result = result.Result,
                Note = result.Note,
                Credits = result.Credits,
                GatherCredits = result.GatherCredits,
                AverageScore = result.AverageScore,
                AverageGatherScore = result.AverageGatherScore,
                NumberOfCreditsFails = result.NumberOfCreditsFails,
                UpdateDate = result.UpdateDate
            })
            .ToListAsync();

        var currentStatus = await (
            from studentStatus in _db.StudentStudyStatuses.AsNoTracking()
            where studentStatus.StudentID == studentCode
                && (yearStudy == null || studentStatus.YearStudy == yearStudy)
                && (termId == null || studentStatus.TermID == termId)
            join status in _db.StudyStatuses.AsNoTracking()
                on studentStatus.StudyStatusID equals status.StudyStatusID into statusJoin
            from status in statusJoin.DefaultIfEmpty()
            join term in _db.Terms.AsNoTracking()
                on new { studentStatus.YearStudy, studentStatus.TermID }
                equals new { term.YearStudy, term.TermID } into termJoin
            from term in termJoin.DefaultIfEmpty()
            orderby studentStatus.YearStudy descending, term.OrderTerm descending, studentStatus.UpdateDate descending
            select new StudentStudyStatusDto
            {
                StudentID = studentStatus.StudentID,
                YearStudy = studentStatus.YearStudy,
                TermID = studentStatus.TermID,
                StudyProgramID = studentStatus.StudyProgramID,
                ClassStudentID = studentStatus.ClassStudentID,
                StudyStatusID = studentStatus.StudyStatusID,
                StudyStatusName = status.StudyStatusName,
                UpdateDate = studentStatus.UpdateDate
            })
            .FirstOrDefaultAsync();

        if (currentStatus is not null
            && !warningHistory.Any(x =>
                x.YearStudy == currentStatus.YearStudy
                && x.TermID == currentStatus.TermID
                && x.StudyStatusID == currentStatus.StudyStatusID))
        {
            warningHistory.Insert(0, currentStatus);
        }

        return warningHistory;
    }

    public async Task<IReadOnlyList<AdvisorDto>> GetAdvisorsAsync(
        string studentCode,
        string? yearStudy,
        string? termId)
    {
        var studentClass = await (
            from studentStatus in _db.StudentStudyStatuses.AsNoTracking()
            where studentStatus.StudentID == studentCode
                && studentStatus.ClassStudentID != null
                && (yearStudy == null || studentStatus.YearStudy == yearStudy)
                && (termId == null || studentStatus.TermID == termId)
            join term in _db.Terms.AsNoTracking()
                on new { studentStatus.YearStudy, studentStatus.TermID }
                equals new { term.YearStudy, term.TermID } into termJoin
            from term in termJoin.DefaultIfEmpty()
            orderby studentStatus.YearStudy descending, term.OrderTerm descending, studentStatus.UpdateDate descending
            select new
            {
                studentStatus.ClassStudentID
            })
            .FirstOrDefaultAsync();

        if (studentClass?.ClassStudentID is null)
        {
            return [];
        }

        var assignmentsQuery =
            from assignment in _db.ProfessorClassStudents.AsNoTracking()
            where assignment.ClassStudentID == studentClass.ClassStudentID
                && (yearStudy == null || assignment.YearStudy == yearStudy)
                && (termId == null || assignment.TermID == termId)
            join professor in _db.Professors.AsNoTracking()
                on assignment.ProfessorID equals professor.ProfessorID
            join term in _db.Terms.AsNoTracking()
                on new { assignment.YearStudy, assignment.TermID }
                equals new { term.YearStudy, term.TermID } into termJoin
            from term in termJoin.DefaultIfEmpty()
            select new AdvisorRow
            {
                ProfessorID = professor.ProfessorID,
                LastName = professor.LastName,
                MiddleName = professor.MiddleName,
                FirstName = professor.FirstName,
                Email = professor.Email,
                Mobile = professor.MobilePhone,
                YearStudy = assignment.YearStudy,
                TermID = assignment.TermID,
                OrderTerm = term.OrderTerm,
                AssignDate = assignment.AssignDate,
                TrainingGroup = assignment.TrainingGroup
            };

        var rows = await assignmentsQuery.ToListAsync();

        return rows
            .GroupBy(x => x.ProfessorID, StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderByDescending(x => x.YearStudy)
                .ThenByDescending(x => x.OrderTerm)
                .ThenByDescending(x => x.AssignDate)
                .First())
            .Select(x => new AdvisorDto
            {
                StudentID = studentCode,
                YearStudy = x.YearStudy,
                TermID = x.TermID,
                ClassStudentID = studentClass.ClassStudentID,
                ProfessorID = x.ProfessorID,
                ProfessorName = JoinName(x.LastName, x.MiddleName, x.FirstName),
                Email = x.Email,
                Mobile = x.Mobile,
                AssignDate = x.AssignDate,
                TrainingGroup = x.TrainingGroup
            })
            .OrderBy(x => x.ProfessorName)
            .ToList();
    }

    private static string JoinName(params string?[] parts)
    {
        return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!.Trim()));
    }

    private sealed class AdvisorRow
    {
        public string ProfessorID { get; set; } = default!;
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string YearStudy { get; set; } = default!;
        public string TermID { get; set; } = default!;
        public int? OrderTerm { get; set; }
        public DateTime? AssignDate { get; set; }
        public string? TrainingGroup { get; set; }
    }
}
