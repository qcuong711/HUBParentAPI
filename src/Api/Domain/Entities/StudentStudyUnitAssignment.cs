namespace Api.Domain.Entities;

public class StudentStudyUnitAssignment
{
    public string StudentID { get; set; } = default!;
    public string StudyUnitID { get; set; } = default!;
    public byte StudyUnitTypeID { get; set; }
    public string AssignmentID { get; set; } = default!;
    public decimal? FirstMark { get; set; }
    public decimal? SecondMark { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string ScheduleStudyUnitID { get; set; } = default!;
}
