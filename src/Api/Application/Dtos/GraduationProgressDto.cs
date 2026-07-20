namespace Api.Application.Dtos;

public class GraduationProgressDto
{
    public string StudentID { get; set; } = default!;
    public string? StudyProgramID { get; set; }
    public string? StudyProgramName { get; set; }
    public decimal? MandatoryCredits { get; set; }
    public decimal? MandatoryGatherCredits { get; set; }
    public decimal? SelectiveCredits { get; set; }
    public decimal? SelectiveGatherCredits { get; set; }
    public decimal RequiredCredits { get; set; }
    public decimal CompletedCredits { get; set; }
    public decimal RemainingCredits { get; set; }
    public decimal? ProgressPercent { get; set; }
    public int? NumberOfCurriculumFails { get; set; }
    public int? NumberOfCreditsFails { get; set; }
    public decimal? AverageGatherScore10 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public DateTime? UpdateDate { get; set; }
}
