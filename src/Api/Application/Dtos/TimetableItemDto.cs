namespace Api.Application.Dtos;

public class TimetableItemDto
{
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string ScheduleStudyUnitID { get; set; } = default!;
    public string? ScheduleStudyUnitAlias { get; set; }
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public int Year { get; set; }
    public int Week { get; set; }
    public int DayOfWeek { get; set; }
    public int? PeriodID { get; set; }
    public string? PeriodName { get; set; }
    public string? BeginTime { get; set; }
    public string? EndTime { get; set; }
    public int? NumberOfPeriods { get; set; }
    public string? RoomID { get; set; }
    public string? RoomName { get; set; }
    public IReadOnlyList<string> Professors { get; set; } = [];
    public int Status { get; set; }
    public string? UnitContent { get; set; }
}
