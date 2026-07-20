namespace Api.Application.Dtos;

public class AdvisorDto
{
    public string StudentID { get; set; } = default!;
    public string YearStudy { get; set; } = default!;
    public string TermID { get; set; } = default!;
    public string ClassStudentID { get; set; } = default!;
    public string ProfessorID { get; set; } = default!;
    public string ProfessorName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public DateTime? AssignDate { get; set; }
    public string? TrainingGroup { get; set; }
}
