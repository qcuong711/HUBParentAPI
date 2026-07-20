namespace Api.Domain.Entities;

public class Curriculum
{
    public string CurriculumID { get; set; } = default!;
    public string? CurriculumName { get; set; }
    public decimal Credits { get; set; }
    public decimal? TheoryCredits { get; set; }
    public decimal? PracticeCredits { get; set; }
    public string? ForeignName { get; set; }
    public string? FrenchName { get; set; }
    public string? StudyTypeName { get; set; }
    public string? SpecializationName { get; set; }
}

