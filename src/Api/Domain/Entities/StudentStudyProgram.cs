namespace Api.Domain.Entities;

public class StudentStudyProgram
{
    public string StudentID { get; set; } = default!;
    public string StudyProgramID { get; set; } = default!;
    public string? SpecializationID { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? Type { get; set; }
    public string? StudyStatus { get; set; }
}
