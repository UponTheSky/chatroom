using System.Net.WebSockets;
using System.Text;
using ChatRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRoomController(
    ILogger<ChatRoomController> logger,
    ChatRoomContext context
) : ControllerBase
{
    private readonly ILogger<ChatRoomController> _logger = logger;
    private readonly ChatRoomContext _context = context;

    [HttpGet("{id}")]
    public async Task<ActionResult<ChatRoomItem>> GetChatRoom([FromRoute] string id)
    {
        _logger.LogInformation("Get ChatRoom with id: {id}", id);

        if (!Guid.TryParse(id, out Guid idInUUID))
        {
            return BadRequest("invalid ID format");
        }

        var chatRoom = await _context.ChatRoomItems.SingleAsync(item => item.Id == idInUUID);

        if (chatRoom == null)
        {
            return NotFound();
        }

        return chatRoom;
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatRoomItem>>> ListChatRooms([FromQuery] int offset = 0, [FromQuery] int limit = 100)
    {
        _logger.LogInformation("Get ChatRooms with offset: {offset}, limit: {limit}", offset, limit);

        var chatRooms = await _context.ChatRoomItems
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        if (chatRooms == null)
        {
            return new List<ChatRoomItem> { };
        }

        return chatRooms;
    }

    [HttpPost]
    public async Task<ActionResult<ChatRoomItem>> CreateChatRoom(CreateChatRoomDto dto)
    {
        _logger.LogInformation("Create a ChatRoom with dto: {dto}", dto);

        var newChatRoom = new ChatRoomItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CurrentUsers = [],
            ChatLogs = [],
        };

        _context.ChatRoomItems.Add(newChatRoom);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChatRoom), new { id = newChatRoom.Id }, newChatRoom);
    }

    [HttpGet("ws/{id}/join")]
    public async Task JoinChatRoom([FromRoute] string id)
    {
        // TODO: add websocket
        // reference: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-9.0
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var userIpAddress = GetUserIPAddress();

            _logger.LogInformation("User {ip} joins ChatRoom {id}", userIpAddress, id);

            if (!Guid.TryParse(id, out Guid idInUUID))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var chatRoom = await _context.ChatRoomItems.SingleAsync(item => item.Id == idInUUID);

            if (chatRoom == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await RunChatLoop(webSocket, chatRoom, userIpAddress ?? "noname");
        }

    }

    private string? GetUserIPAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private async Task RunChatLoop(WebSocket ws, ChatRoomItem chatRoom, string userIpAddress)
    {

        chatRoom.CurrentUsers.Add(userIpAddress);

        _context.ChangeTracker.DetectChanges();
        Console.WriteLine(_context.ChangeTracker.DebugView.LongView);

        await _context.SaveChangesAsync();

        var buf = new byte[1024 * 4];
        var receiveResult = await ws.ReceiveAsync(
            new ArraySegment<byte>(buf),
            CancellationToken.None
        );

        while (!receiveResult.CloseStatus.HasValue)
        {
            string? log = Encoding.UTF8.GetString(buf, 0, receiveResult.Count);
            ChatLog chatLog = new()
            {
                ChatRoomItemId = chatRoom.Id,
                ChatRoomItem = chatRoom,
                Id = Guid.NewGuid(),
                IpAddress = userIpAddress,
                TimeStamp = DateTime.UtcNow,
                Text = log ?? "",
            };

            _context.ChatLogs.Add(chatLog);
            chatRoom.ChatLogs.Add(chatLog);

            await _context.SaveChangesAsync();

            await ws.SendAsync(
                new ArraySegment<byte>(buf, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None
            );

            receiveResult = await ws.ReceiveAsync(
                new ArraySegment<byte>(buf),
                CancellationToken.None
            );
        }

        await ws.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None
        );

        return;
    }
}
