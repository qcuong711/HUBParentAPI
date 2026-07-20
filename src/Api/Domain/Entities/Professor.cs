namespace Api.Domain.Entities;

public class Professor
{
    public string ProfessorID { get; set; } = default!;
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? FirstName { get; set; }
    public string? MobilePhone { get; set; }
    public string? Email { get; set; }
}
