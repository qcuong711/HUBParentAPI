namespace Api.Domain.Entities;

public class StudentScheduleStudyUnit
{
    public string StudentID { get; set; } = default!;
    public string ScheduleStudyUnitID { get; set; } = default!;
    public int? RegistStatus { get; set; }
}
