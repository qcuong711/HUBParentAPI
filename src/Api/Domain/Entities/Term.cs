namespace Api.Domain.Entities;

public class Term
{
    public string YearStudy { get; set; } = default!;
    public string TermID { get; set; } = default!;
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OrderTerm { get; set; }
}
