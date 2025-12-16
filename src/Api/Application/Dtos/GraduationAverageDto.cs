namespace Api.Application.Dtos;

public class GraduationAverageDto
{
    public decimal? AverageScore { get; set; }
    public decimal? AverageGatherScore { get; set; }
    public decimal? AverageScore4 { get; set; }
    public decimal? AverageGatherScore4 { get; set; }
    public bool? IsModified { get; set; }
}
