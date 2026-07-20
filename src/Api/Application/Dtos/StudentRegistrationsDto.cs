namespace Api.Application.Dtos;

public class StudentRegistrationsDto
{
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public int TotalCourses { get; set; }
    public decimal TotalCredits { get; set; }
    public IReadOnlyList<RegisteredCourseDto> Items { get; set; } = [];
}

public class RegisteredCourseDto
{
    public string ScheduleStudyUnitID { get; set; } = default!;
    public string? ScheduleStudyUnitAlias { get; set; }
    public string StudyUnitID { get; set; } = default!;
    public string? StudyUnitAlias { get; set; }
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public decimal Credits { get; set; }
    public decimal? TheoryCredits { get; set; }
    public decimal? PracticeCredits { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Status { get; set; }
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
}
