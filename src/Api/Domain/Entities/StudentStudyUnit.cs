namespace Api.Domain.Entities;

public class StudentStudyUnit
{
    public string StudentID { get; set; } = default!;
    public string StudyUnitID { get; set; } = default!;
    public decimal? Mark10 { get; set; }
    public decimal? Mark4 { get; set; }
    public string? MarkLetter { get; set; }
    public decimal? Mark10_2 { get; set; }
    public decimal? Mark4_2 { get; set; }
    public string? MarkLetter_2 { get; set; }
    public string? Note { get; set; }
    public bool? IsPass { get; set; }
    public bool? IsInUsed { get; set; }
    public byte? IsOfficial { get; set; }
    public string? UpdateStaff { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool? IsAccepted { get; set; }
    public string? SpecialScores { get; set; }
    public string? SpecialScores_2 { get; set; }
    public int? TinhTrang { get; set; }
    public int? StudyUnitLock { get; set; }
    public decimal? Mark100 { get; set; }
    public decimal? Mark100_2 { get; set; }
    public decimal? Mark100_3 { get; set; }
    public decimal? Mark100_4 { get; set; }
    public decimal? Mark100_5 { get; set; }
    public decimal? MaxMark100 { get; set; }
    public string? MaxMark4 { get; set; }
    public string? MaxMark10 { get; set; }
    public string? MaxMarkLetter { get; set; }
}

