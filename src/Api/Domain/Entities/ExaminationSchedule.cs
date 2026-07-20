namespace Api.Domain.Entities;

public class ExaminationSchedule
{
    public int ExaminationScheduleID { get; set; }
    public string? BeginTime { get; set; }
    public string? EndTime { get; set; }
    public int? DayOfWeek { get; set; }
    public int? Week { get; set; }
    public int? Year { get; set; }
    public DateTime? PlannedExamDate { get; set; }
}
