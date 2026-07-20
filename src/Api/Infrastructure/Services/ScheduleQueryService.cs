using System.Globalization;
using Api.Application;
using Api.Application.Dtos;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Services;

public class ScheduleQueryService : IScheduleQueryService
{
    private readonly AppDbContext _db;

    public ScheduleQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TimetableItemDto>> GetTimetableAsync(
        string studentCode,
        string? yearStudy,
        string? termId)
    {
        var term = await ResolveTermAsync(yearStudy, termId);
        if (term is null)
        {
            return [];
        }

        var rows = await (
            from registration in _db.StudentScheduleStudyUnits.AsNoTracking()
            where registration.StudentID == studentCode
            join scheduledUnit in _db.ScheduleStudyUnits.AsNoTracking()
                on registration.ScheduleStudyUnitID equals scheduledUnit.ScheduleStudyUnitID
            join studyUnit in _db.StudyUnits.AsNoTracking()
                on scheduledUnit.StudyUnitID equals studyUnit.StudyUnitID
            where studyUnit.YearStudy == term.YearStudy && studyUnit.TermID == term.TermID
            join curriculum in _db.Curriculums.AsNoTracking()
                on studyUnit.CurriculumID equals curriculum.CurriculumID
            join weekStudyUnit in _db.WeekScheduleStudyUnits.AsNoTracking()
                on scheduledUnit.ScheduleStudyUnitID equals weekStudyUnit.ScheduleStudyUnitID
            join weekSchedule in _db.WeekSchedules.AsNoTracking()
                on weekStudyUnit.WeekScheduleID equals weekSchedule.WeekScheduleID
            where weekSchedule.Year.HasValue
                && weekSchedule.Year != 1055
                && weekSchedule.Week.HasValue
                && weekSchedule.Week > 0
                && weekSchedule.DayOfWeek.HasValue
            join period in _db.Periods.AsNoTracking()
                on weekSchedule.PeriodID equals period.PeriodID into periodJoin
            from period in periodJoin.DefaultIfEmpty()
            join room in _db.Rooms.AsNoTracking()
                on weekSchedule.RoomID equals room.RoomID into roomJoin
            from room in roomJoin.DefaultIfEmpty()
            select new TimetableRow
            {
                WeekScheduleID = weekSchedule.WeekScheduleID,
                ScheduleStudyUnitID = scheduledUnit.ScheduleStudyUnitID,
                ScheduleStudyUnitAlias = scheduledUnit.ScheduleStudyUnitAlias,
                CurriculumID = curriculum.CurriculumID,
                CurriculumName = curriculum.CurriculumName,
                Year = weekSchedule.Year!.Value,
                Week = weekSchedule.Week!.Value,
                DayOfWeek = weekSchedule.DayOfWeek!.Value,
                PeriodID = weekSchedule.PeriodID,
                PeriodName = period.PeriodName,
                BeginTime = period.BeginTime,
                EndTime = period.EndTime,
                NumberOfPeriods = weekSchedule.NumberOfPeriods,
                RoomID = weekSchedule.RoomID,
                RoomName = room.RoomName,
                Status = weekSchedule.Status,
                UnitContent = weekStudyUnit.UnitContent
            })
            .ToListAsync();

        var distinctRows = rows
            .GroupBy(x => new { x.WeekScheduleID, x.ScheduleStudyUnitID })
            .Select(x => x.First())
            .ToList();

        var weekScheduleIds = distinctRows.Select(x => x.WeekScheduleID).Distinct().ToList();
        var professorRows = weekScheduleIds.Count == 0
            ? []
            : await (
                from assignment in _db.ProfessorWeekSchedules.AsNoTracking()
                where weekScheduleIds.Contains(assignment.WeekScheduleID)
                join professor in _db.Professors.AsNoTracking()
                    on assignment.ProfessorID equals professor.ProfessorID
                select new
                {
                    assignment.WeekScheduleID,
                    professor.ProfessorID,
                    professor.LastName,
                    professor.MiddleName,
                    professor.FirstName
                })
                .Distinct()
                .ToListAsync();

        var professorsByWeek = professorRows
            .GroupBy(x => x.WeekScheduleID)
            .ToDictionary(
                x => x.Key,
                x => (IReadOnlyList<string>)x
                    .Select(p => JoinName(p.LastName, p.MiddleName, p.FirstName))
                    .Where(name => name.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToList());

        return distinctRows
            .Select(x => new TimetableItemDto
            {
                StudentID = studentCode,
                YearStudy = term.YearStudy,
                TermID = term.TermID,
                ScheduleStudyUnitID = x.ScheduleStudyUnitID,
                ScheduleStudyUnitAlias = x.ScheduleStudyUnitAlias,
                CurriculumID = x.CurriculumID,
                CurriculumName = x.CurriculumName,
                Year = x.Year,
                Week = x.Week,
                DayOfWeek = x.DayOfWeek,
                PeriodID = x.PeriodID,
                PeriodName = x.PeriodName,
                BeginTime = x.BeginTime,
                EndTime = x.EndTime,
                NumberOfPeriods = x.NumberOfPeriods,
                RoomID = x.RoomID,
                RoomName = x.RoomName,
                Professors = professorsByWeek.GetValueOrDefault(x.WeekScheduleID, []),
                Status = x.Status,
                UnitContent = x.UnitContent
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Week)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => ParseTime(x.BeginTime))
            .ToList();
    }

    public async Task<IReadOnlyList<ExamScheduleItemDto>> GetExamsAsync(
        string studentCode,
        string? yearStudy,
        string? termId)
    {
        var term = await ResolveTermAsync(yearStudy, termId);
        if (term is null)
        {
            return [];
        }

        var rows = await (
            from examinationStudent in _db.ExaminationStudents.AsNoTracking()
            where examinationStudent.StudentID == studentCode
            join examination in _db.Examinations.AsNoTracking()
                on examinationStudent.ExaminationID equals examination.ExaminationID
            join scheduledUnit in _db.ScheduleStudyUnits.AsNoTracking()
                on examination.ScheduleStudyUnitID equals scheduledUnit.ScheduleStudyUnitID into scheduledUnitJoin
            from scheduledUnit in scheduledUnitJoin.DefaultIfEmpty()
            join studyUnit in _db.StudyUnits.AsNoTracking()
                on (examination.StudyUnitID ?? scheduledUnit.StudyUnitID) equals studyUnit.StudyUnitID
            where studyUnit.YearStudy == term.YearStudy && studyUnit.TermID == term.TermID
            join curriculum in _db.Curriculums.AsNoTracking()
                on studyUnit.CurriculumID equals curriculum.CurriculumID
            join assignment in _db.Assignments.AsNoTracking()
                on examination.AssignmentID equals assignment.AssignmentID into assignmentJoin
            from assignment in assignmentJoin.DefaultIfEmpty()
            join examinationSchedule in _db.ExaminationSchedules.AsNoTracking()
                on examination.ExaminationScheduleID equals (int?)examinationSchedule.ExaminationScheduleID into scheduleJoin
            from examinationSchedule in scheduleJoin.DefaultIfEmpty()
            join room in _db.Rooms.AsNoTracking()
                on examination.RoomID equals room.RoomID into roomJoin
            from room in roomJoin.DefaultIfEmpty()
            select new ExamRow
            {
                ExaminationID = examination.ExaminationID,
                ExaminationScheduleID = examination.ExaminationScheduleID,
                ScheduleStudyUnitID = examination.ScheduleStudyUnitID,
                CurriculumID = curriculum.CurriculumID,
                CurriculumName = curriculum.CurriculumName,
                AssignmentID = examination.AssignmentID,
                AssignmentName = assignment.AssignmentName,
                Year = examination.Year ?? examinationSchedule.Year,
                Week = examination.Week ?? examinationSchedule.Week,
                DayOfWeek = examination.DayOfWeek ?? examinationSchedule.DayOfWeek,
                PlannedExamDate = examination.PlannedExamDate ?? examinationSchedule.PlannedExamDate,
                BeginTime = examination.BeginTime ?? examinationSchedule.BeginTime,
                EndTime = examination.EndTime ?? examinationSchedule.EndTime,
                RoomID = examination.RoomID,
                RoomName = room.RoomName,
                Status = examination.IsUsed,
                IsAbsent = examinationStudent.IsAbsent,
                Note = examinationStudent.Note ?? examination.Note
            })
            .ToListAsync();

        var examinationIds = rows.Select(x => x.ExaminationID).Distinct().ToList();
        var fallbackRooms = examinationIds.Count == 0
            ? []
            : await (
                from examinationRoom in _db.ExaminationRooms.AsNoTracking()
                where examinationRoom.ExaminationID.HasValue
                    && examinationIds.Contains(examinationRoom.ExaminationID.Value)
                join room in _db.Rooms.AsNoTracking()
                    on examinationRoom.RoomID equals room.RoomID into roomJoin
                from room in roomJoin.DefaultIfEmpty()
                orderby examinationRoom.ID
                select new
                {
                    ExaminationID = examinationRoom.ExaminationID!.Value,
                    examinationRoom.RoomID,
                    RoomName = room.RoomName
                })
                .ToListAsync();

        var fallbackRoomByExam = fallbackRooms
            .GroupBy(x => x.ExaminationID)
            .ToDictionary(x => x.Key, x => x.First());

        return rows
            .GroupBy(x => x.ExaminationID)
            .Select(x => x.First())
            .Select(x =>
            {
                fallbackRoomByExam.TryGetValue(x.ExaminationID, out var fallbackRoom);
                return new ExamScheduleItemDto
                {
                    StudentID = studentCode,
                    YearStudy = term.YearStudy,
                    TermID = term.TermID,
                    ExaminationID = x.ExaminationID,
                    ExaminationScheduleID = x.ExaminationScheduleID,
                    ScheduleStudyUnitID = x.ScheduleStudyUnitID,
                    CurriculumID = x.CurriculumID,
                    CurriculumName = x.CurriculumName,
                    AssignmentID = x.AssignmentID,
                    AssignmentName = x.AssignmentName,
                    Year = x.Year,
                    Week = x.Week,
                    DayOfWeek = x.DayOfWeek,
                    ExamDate = x.PlannedExamDate ?? ToExamDate(x.Year, x.Week, x.DayOfWeek),
                    BeginTime = x.BeginTime,
                    EndTime = x.EndTime,
                    RoomID = x.RoomID ?? fallbackRoom?.RoomID,
                    RoomName = x.RoomName ?? fallbackRoom?.RoomName,
                    Status = x.Status,
                    IsAbsent = x.IsAbsent,
                    Note = x.Note
                };
            })
            .OrderBy(x => x.ExamDate)
            .ThenBy(x => ParseTime(x.BeginTime))
            .ThenBy(x => x.CurriculumName)
            .ToList();
    }

    private async Task<TermKey?> ResolveTermAsync(string? yearStudy, string? termId)
    {
        if (yearStudy is not null && termId is not null)
        {
            return new TermKey(yearStudy, termId);
        }

        var today = DateTime.Today;
        return await _db.Terms.AsNoTracking()
            .Where(x => x.BeginDate <= today
                && x.EndDate >= today
                && (yearStudy == null || x.YearStudy == yearStudy)
                && (termId == null || x.TermID == termId))
            .OrderByDescending(x => x.BeginDate)
            .Select(x => new TermKey(x.YearStudy, x.TermID))
            .FirstOrDefaultAsync();
    }

    private static DateTime? ToExamDate(int? year, int? week, int? dayOfWeek)
    {
        if (year is null || week is null || dayOfWeek is null || week < 1)
        {
            return null;
        }

        DayOfWeek? day = dayOfWeek.Value switch
        {
            2 => DayOfWeek.Monday,
            3 => DayOfWeek.Tuesday,
            4 => DayOfWeek.Wednesday,
            5 => DayOfWeek.Thursday,
            6 => DayOfWeek.Friday,
            7 => DayOfWeek.Saturday,
            8 => DayOfWeek.Sunday,
            1 => DayOfWeek.Monday,
            _ => null
        };

        if (day is null)
        {
            return null;
        }

        try
        {
            return ISOWeek.ToDateTime(year.Value, week.Value, day.Value);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    private static TimeSpan ParseTime(string? value)
    {
        return TimeSpan.TryParse(value, out var time) ? time : TimeSpan.MaxValue;
    }

    private static string JoinName(params string?[] parts)
    {
        return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!.Trim()));
    }

    private sealed record TermKey(string YearStudy, string TermID);

    private sealed class TimetableRow
    {
        public int WeekScheduleID { get; set; }
        public string ScheduleStudyUnitID { get; set; } = default!;
        public string? ScheduleStudyUnitAlias { get; set; }
        public string? CurriculumID { get; set; }
        public string? CurriculumName { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public int DayOfWeek { get; set; }
        public int? PeriodID { get; set; }
        public string? PeriodName { get; set; }
        public string? BeginTime { get; set; }
        public string? EndTime { get; set; }
        public int? NumberOfPeriods { get; set; }
        public string? RoomID { get; set; }
        public string? RoomName { get; set; }
        public int Status { get; set; }
        public string? UnitContent { get; set; }
    }

    private sealed class ExamRow
    {
        public int ExaminationID { get; set; }
        public int? ExaminationScheduleID { get; set; }
        public string? ScheduleStudyUnitID { get; set; }
        public string? CurriculumID { get; set; }
        public string? CurriculumName { get; set; }
        public string? AssignmentID { get; set; }
        public string? AssignmentName { get; set; }
        public int? Year { get; set; }
        public int? Week { get; set; }
        public int? DayOfWeek { get; set; }
        public DateTime? PlannedExamDate { get; set; }
        public string? BeginTime { get; set; }
        public string? EndTime { get; set; }
        public string? RoomID { get; set; }
        public string? RoomName { get; set; }
        public bool? Status { get; set; }
        public bool? IsAbsent { get; set; }
        public string? Note { get; set; }
    }
}
