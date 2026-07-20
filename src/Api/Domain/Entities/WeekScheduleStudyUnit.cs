namespace Api.Domain.Entities;

public class WeekScheduleStudyUnit
{
    public string ScheduleStudyUnitID { get; set; } = default!;
    public int WeekScheduleID { get; set; }
    public string? UnitContent { get; set; }
}
