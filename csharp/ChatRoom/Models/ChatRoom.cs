namespace ChatRoom.Models;

public class ChatRoom
{
    public long Id { get; set; }
    public required string Name { get; set; }

    public string? Description { get; set; }

    public required List<string> CurrentUsers { get; set; }

    public required List<ChatLog> ChatLogs { get; set; }
}

public class ChatLog
{
    public required string IpAddress { get; set; }

    public required DateTime TimeStamp { get; set; }

    public required string Text { get; set; }
}
