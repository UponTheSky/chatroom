using System.Net;
using System.Resources;
using ChatRoom.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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

    [Route("ws/[controller]/{id}/join")]
    public async Task JoinChatRoom([FromRoute] string id)
    {
        // TODO: add websocket
        // reference: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-9.0
        var userIpAddress = GetUserIPAddress();

        _logger.LogInformation("User {ip} joins ChatRoom {id}", userIpAddress, id);

        if (!Guid.TryParse(id, out Guid idInUUID))
        {
            return BadRequest("invalid ID format");
        }

        var chatRoom = await _context.ChatRoomItems.SingleAsync(item => item.Id == idInUUID);

        if (chatRoom == null)
        {
            return NotFound();
        }

        // TODO: implement ws
        return null!;
    }

    private string? GetUserIPAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
