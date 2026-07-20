namespace Api.Domain.Entities;

public class Period
{
    public int PeriodID { get; set; }
    public string PeriodName { get; set; } = default!;
    public string BeginTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
}
