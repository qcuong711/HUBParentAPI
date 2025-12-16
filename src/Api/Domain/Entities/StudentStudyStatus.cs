namespace Api.Domain.Entities;

public class StudentStudyStatus
{
    public string StudentID { get; set; } = default!;
    public string? StudyProgramID { get; set; }
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string? ClassStudentID { get; set; }
    public string? StudentTypeID { get; set; }
}

