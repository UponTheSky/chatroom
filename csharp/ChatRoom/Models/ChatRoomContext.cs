using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Models;

public class ChatRoomContext(DbContextOptions<ChatRoomContext> options) : DbContext(options)
{
    public DbSet<ChatRoom> ChatRooms { get; set; } = null!;
    public DbSet<ChatLog> ChatLogs { get; set; } = null!;
}
