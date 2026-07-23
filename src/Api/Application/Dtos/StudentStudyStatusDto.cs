namespace Api.Application.Dtos;

public class StudentStudyStatusDto
{
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    /// <summary>Academic year whose results caused the warning; null for current-status-only items.</summary>
    public string? WarningForYearStudy { get; set; }

    /// <summary>Term whose results caused the warning; null for current-status-only items.</summary>
    public string? WarningForTermID { get; set; }

    /// <summary>Time when the academic warning result was evaluated or updated.</summary>
    public DateTime? EvaluatedAt { get; set; }
    public string? StudyProgramID { get; set; }
    public string? ClassStudentID { get; set; }
    public string? StudyStatusID { get; set; }
    public string? StudyStatusName { get; set; }
    public bool? IsFail { get; set; }
    public string? Result { get; set; }
    public string? Note { get; set; }
    public int? Credits { get; set; }
    public int? GatherCredits { get; set; }
    public decimal? AverageScore { get; set; }
    public decimal? AverageGatherScore { get; set; }
    public int? NumberOfCreditsFails { get; set; }
    public DateTime? UpdateDate { get; set; }
}
