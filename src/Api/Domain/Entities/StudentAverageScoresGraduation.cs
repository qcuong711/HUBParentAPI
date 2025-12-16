namespace Api.Domain.Entities;

public class StudentAverageScoresGraduation
{
    public string? StudentID { get; set; }
    public string? StudyProgramID { get; set; }
    public decimal? AverageScore { get; set; }
    public decimal? AverageGatherScore { get; set; }
    public decimal? AverageScoreFirstMark { get; set; }
    public decimal? AverageGatherScoreFirstMark { get; set; }
    public decimal? AverageScore4 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public decimal? AverageScoreFirstMark4 { get; set; }
    public decimal? AverageGatherScoreFirstMark4 { get; set; }
    public decimal? MandatoryCredits { get; set; }
    public decimal? MandatoryGatherCredits { get; set; }
    public decimal? SelectiveCredits { get; set; }
    public decimal? SelectiveGatherCredits { get; set; }
    public bool? IsModified { get; set; }
    public int? NumberOfCurriculumFails { get; set; }
    public int? NumberOfCreditsFails { get; set; }
    public int? NumberOfCurriculumSecondExam { get; set; }
    public int? NumberOfCreditsSecondExams { get; set; }
    public DateTime? UpdateDate { get; set; }
    public decimal? AverageScore10 { get; set; }
    public decimal? AverageGatherScore10 { get; set; }
    public decimal? AverageScoreFirstMark10 { get; set; }
    public decimal? AverageGatherScoreFirstMark10 { get; set; }
}

