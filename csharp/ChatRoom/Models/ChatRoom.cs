using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Models;

public class CreateChatRoomDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class ChatRoomItem
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public string? Description { get; set; }

    public required List<string> CurrentUsers { get; set; } = [];

    public required ICollection<ChatLog> ChatLogs { get; set; } = [];
}

public class ChatLog
{
    public required Guid ChatRoomItemId { get; set; }

    public required ChatRoomItem ChatRoomItem { get; set; } = null!;

    public required Guid Id { get; set; }
    public required string IpAddress { get; set; }

    public required DateTime TimeStamp { get; set; }

    public required string Text { get; set; }
}
