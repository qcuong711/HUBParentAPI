namespace Api.Application.Dtos;

public class GraduationStandardsDto
{
    public string StudentID { get; set; } = default!;
    public bool HasStudyProgram { get; set; }
    public string? StudyProgramID { get; set; }
    public string? StudyProgramName { get; set; }
    public bool CriteriaConfigured { get; set; }
    public int? GraduationCriteriaID { get; set; }
    public string? GraduationCriteriaName { get; set; }
    public DateTime? CriteriaApplyDate { get; set; }
    public DateTime? CriteriaUpdateDate { get; set; }
    public DateTime? LatestEvidenceDate { get; set; }
    public IReadOnlyList<GraduationStandardDto> Standards { get; set; } = [];
}

public class GraduationStandardDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool? Required { get; set; }
    public bool Achieved { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? LastUpdated { get; set; }
    public IReadOnlyList<string> EvidenceSources { get; set; } = [];
}
