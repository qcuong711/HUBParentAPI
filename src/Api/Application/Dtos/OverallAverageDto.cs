namespace Api.Application.Dtos;

public class OverallAverageDto
{
    public decimal? AverageScore10 { get; set; }
    public decimal? AverageScore4 { get; set; }
    public decimal? AverageGatherScore10 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public bool? IsModified { get; set; }
    public string? UpdateStaff { get; set; }
    public DateTime? UpdateDate { get; set; }
}

