namespace Api.Domain.Entities;

public class GraduationCriteriaStudyProgram
{
    public int GraduationCriteriaID { get; set; }
    public string? GraduationCriteriaName { get; set; }
    public byte GraduationCriteriaTypeID { get; set; }
    public string StudyProgramID { get; set; } = default!;
    public bool? IsLanguageCertificate { get; set; }
    public bool? IsITCertificate { get; set; }
    public bool? IsKTTiengAnhTangCuong { get; set; }
    public bool? IsKyNangMem { get; set; }
    public bool? IsNCKH { get; set; }
    public DateTime ApplyDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
