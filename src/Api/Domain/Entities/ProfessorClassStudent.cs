namespace Api.Domain.Entities;

public class ProfessorClassStudent
{
    public string ProfessorID { get; set; } = default!;
    public string ClassStudentID { get; set; } = default!;
    public string YearStudy { get; set; } = default!;
    public string TermID { get; set; } = default!;
    public DateTime? AssignDate { get; set; }
    public string? TrainingGroup { get; set; }
}
