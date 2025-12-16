namespace Api.Domain.Entities;

public class StudentAverageGatherScoresByTerms
{
    public string? StudentID { get; set; }
    public string? StudyProgramID { get; set; }
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public decimal? AverageScore { get; set; }
    public decimal? AverageGatherScore { get; set; }
    public decimal? AverageScoreFirstMark { get; set; }
    public decimal? AverageGatherScoreFirstMark { get; set; }
    public decimal? MandatoryCredits { get; set; }
    public decimal? MandatoryGatherCredits { get; set; }
    public decimal? SelectiveCredits { get; set; }
    public decimal? SelectiveGatherCredits { get; set; }
    public int? NumberOfFails { get; set; }
    public decimal? AverageScore4 { get; set; }
    public decimal? AverageScoreFirstMark4 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public decimal? AverageGatherScoreFirstMark4 { get; set; }
    public decimal? SumCreditsOfFails { get; set; }
    public decimal? AverageScore10 { get; set; }
    public decimal? AverageScoreFirstMark10 { get; set; }
    public decimal? AverageGatherScore10 { get; set; }
    public decimal? AverageGatherScoreFirstMark10 { get; set; }
    public decimal? NumberOfCurriculum { get; set; }
    public decimal? NumberOfCurriculumFails { get; set; }
    public decimal? NumberOfCurriculumSecondExam { get; set; }
    public decimal? NumberOfCreditsFails { get; set; }
    public decimal? NumberOfCreditsSecondExams { get; set; }
    public string? UpdateDate { get; set; }
    public string? SemesterID { get; set; }
    public string? StudyYear { get; set; }
    public int? OrderTerm { get; set; }
    public decimal? SumOfCredits { get; set; }
    public decimal? SumOfGatherCredits { get; set; }
    public decimal? CreditForFirstMark { get; set; }
    public decimal? GatherCreditForFirstMark { get; set; }
    public decimal? NumberOfCreditsSecondLearn { get; set; }
    public int? NumberOfCurriculumSecondLearn { get; set; }
    public int? TongSoMonDangKy { get; set; }
    public decimal? TongSoTinChiDangKy { get; set; }
    public decimal? TongSoTinChiDangKyDat { get; set; }
    public decimal? TongSoTinChiDangKyKhongDat { get; set; }
    public string? UpdateStaff { get; set; }
    public decimal? SelectiveGatherCredits_HB { get; set; }
    public decimal? MandatoryGatherCredits_HB { get; set; }
}

