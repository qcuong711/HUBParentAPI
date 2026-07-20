namespace Api.Domain.Entities;

public class Examination
{
    public int ExaminationID { get; set; }
    public string? StudyUnitID { get; set; }
    public string? AssignmentID { get; set; }
    public string? BeginTime { get; set; }
    public string? EndTime { get; set; }
    public int? DayOfWeek { get; set; }
    public int? Week { get; set; }
    public int? Year { get; set; }
    public string? RoomID { get; set; }
    public bool? IsUsed { get; set; }
    public string? Note { get; set; }
    public string? ScheduleStudyUnitID { get; set; }
    public int? ExaminationScheduleID { get; set; }
    public DateTime? PlannedExamDate { get; set; }
}
