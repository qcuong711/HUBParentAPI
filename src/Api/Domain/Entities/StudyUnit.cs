namespace Api.Domain.Entities;

public class StudyUnit
{
    public string StudyUnitID { get; set; } = default!;
    public string? StudyUnitAlias { get; set; }
    public string? CurriculumID { get; set; }
    public string? StudyTypeID { get; set; }
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
}

