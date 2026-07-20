namespace Api.Domain.Entities;

public class WeekSchedule
{
    public int WeekScheduleID { get; set; }
    public int? Year { get; set; }
    public int? Week { get; set; }
    public string? RoomID { get; set; }
    public int? DayOfWeek { get; set; }
    public int? PeriodID { get; set; }
    public int? NumberOfPeriods { get; set; }
    public int Status { get; set; }
}
