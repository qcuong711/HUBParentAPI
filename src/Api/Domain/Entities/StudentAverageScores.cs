namespace Api.Domain.Entities;

public class StudentAverageScores
{
    public string? StudentID { get; set; }
    public string? StudyProgramID { get; set; }
    public decimal? AverageScore { get; set; }
    public decimal? AverageGatherScore { get; set; }
    public decimal? AverageScoreFirstMark { get; set; }
    public decimal? AverageGatherScoreFirstMark { get; set; }
    public decimal? MandatoryCredits { get; set; }
    public decimal? MandatoryGatherCredits { get; set; }
    public decimal? SelectiveCredits { get; set; }
    public decimal? SelectiveGatherCredits { get; set; }
    public bool? IsModified { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? UpdateStaff { get; set; }
    public int? NumberOfSecondExams { get; set; }
    public int? NumberOfFails { get; set; }
    public decimal? SumCreditsOfSecondExams { get; set; }
    public decimal? AverageScore4 { get; set; }
    public decimal? AverageScoreFirstMark4 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public decimal? AverageGatherScoreFirstMark4 { get; set; }
    public decimal? AverageScoreBeforeProject { get; set; }
    public string? GraduationCourseID { get; set; }
    public decimal? AverageScoreBeforeProject4 { get; set; }
    public decimal? SumCreditsOfFails { get; set; }
    public decimal? AverageGatherScore10 { get; set; }
    public decimal? AverageGatherScoreFirstMark10 { get; set; }
    public decimal? AverageScore10 { get; set; }
    public decimal? AverageScoreFirstMark10 { get; set; }
    public decimal? NumberOfCreditsFails { get; set; }
    public decimal? NumberOfCreditsSecondExams { get; set; }
    public decimal? NumberOfCurriculum { get; set; }
    public decimal? NumberOfCurriculumFails { get; set; }
    public decimal? NumberOfCurriculumSecondExam { get; set; }
    public decimal? SumOfCredits { get; set; }
    public decimal? SumOfGatherCredits { get; set; }
    public decimal? CreditForFirstMark { get; set; }
    public decimal? GatherCreditForFirstMark { get; set; }
    public decimal? NumberOfCurriculumSecondLearn { get; set; }
    public decimal? NumberOfCreditsSecondLearn { get; set; }
    public int? TongSoMonDangKy { get; set; }
    public int? TongSoTinChiDangKy { get; set; }
    public int? TongSoTinChiDangKyDat { get; set; }
    public int? TongSoTinChiDangKyKhongDat { get; set; }
}

