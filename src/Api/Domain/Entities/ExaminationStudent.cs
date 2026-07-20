namespace Api.Domain.Entities;

public class ExaminationStudent
{
    public int ExaminationID { get; set; }
    public string StudentID { get; set; } = default!;
    public string? Note { get; set; }
    public bool? IsAbsent { get; set; }
}
