namespace Api.Domain.Entities;

public class ScheduleStudyUnit
{
    public string ScheduleStudyUnitID { get; set; } = default!;
    public string StudyUnitID { get; set; } = default!;
    public string? ScheduleStudyUnitAlias { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte? Status { get; set; }
}
