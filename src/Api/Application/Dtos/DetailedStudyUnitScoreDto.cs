namespace Api.Application.Dtos;

public class DetailedStudyUnitScoreDto
{
    public string? StudyUnitID { get; set; }
    public string? StudyUnitAlias { get; set; }
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string? ClassStudentID { get; set; }
    public string? ClassStudentName { get; set; }
    public string? StudyProgramID { get; set; }
    public string? StudyProgramName { get; set; }
    public string? StudyTypeID { get; set; }
    public string? StudyTypeName { get; set; }
    public decimal? Mark10 { get; set; }
    public decimal? Mark4 { get; set; }
    public string? MarkLetter { get; set; }
}

