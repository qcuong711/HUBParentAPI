namespace Api.Domain.Entities;

public class Assignment
{
    public string AssignmentID { get; set; } = default!;
    public string? AssignmentName { get; set; }
    public string? EnglishAssignmentName { get; set; }
    public string? Abbreviation { get; set; }
}

