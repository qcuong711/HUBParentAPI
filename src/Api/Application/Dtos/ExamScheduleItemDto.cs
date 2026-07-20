namespace Api.Application.Dtos;

public class ExamScheduleItemDto
{
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public int ExaminationID { get; set; }
    public int? ExaminationScheduleID { get; set; }
    public string? ScheduleStudyUnitID { get; set; }
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public string? AssignmentID { get; set; }
    public string? AssignmentName { get; set; }
    public int? Year { get; set; }
    public int? Week { get; set; }
    public int? DayOfWeek { get; set; }
    public DateTime? ExamDate { get; set; }
    public string? BeginTime { get; set; }
    public string? EndTime { get; set; }
    public string? RoomID { get; set; }
    public string? RoomName { get; set; }
    public bool? Status { get; set; }
    public bool? IsAbsent { get; set; }
    public string? Note { get; set; }
}
