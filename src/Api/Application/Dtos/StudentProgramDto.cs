namespace Api.Application.Dtos;

public class StudentProgramDto
{
    public string StudentID { get; set; } = default!;
    public string StudyProgramID { get; set; } = default!;
    public string? StudyProgramName { get; set; }
    public string? MajorID { get; set; }
    public string? MajorName { get; set; }
    public string? SpecializationID { get; set; }
    public string? SpecializationName { get; set; }
    public int? ProgramType { get; set; }
    public string? StudyStatus { get; set; }
    public DateTime? UpdateDate { get; set; }
}
