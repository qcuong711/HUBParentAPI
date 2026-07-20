namespace Api.Domain.Entities;

public class ExaminationRoom
{
    public int ID { get; set; }
    public int? ExaminationID { get; set; }
    public string? RoomID { get; set; }
}
