namespace Api.Application.Dtos;

public class TermGatherAverageDto
{
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public int? OrderTerm { get; set; }
    public decimal? AverageScore10 { get; set; }
    public decimal? AverageScore4 { get; set; }
    public decimal? AverageGatherScore10 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public string? UpdateDate { get; set; }
}

