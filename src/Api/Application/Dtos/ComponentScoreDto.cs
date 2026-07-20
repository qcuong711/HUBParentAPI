namespace Api.Application.Dtos;

public class ComponentScoreDto
{
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string StudyUnitID { get; set; } = default!;
    public string ScheduleStudyUnitID { get; set; } = default!;
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public string AssignmentID { get; set; } = default!;
    public string? AssignmentName { get; set; }
    public string? Abbreviation { get; set; }
    public decimal? FirstMark { get; set; }
    public decimal? SecondMark { get; set; }
    public DateTime? UpdateDate { get; set; }
}
