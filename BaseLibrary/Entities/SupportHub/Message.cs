namespace BaseLibrary.Entities.SupportHub;

public class Message
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}