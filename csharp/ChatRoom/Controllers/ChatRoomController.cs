using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRoomController(
    ILogger<ChatRoomController> logger
) : ControllerBase
{
    private readonly ILogger<ChatRoomController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> GetChatRoom([FromRoute] long id)
    {
        _logger.LogInformation("Get ChatRoom with id: {id}", id);
        return "test";
    }

    [HttpGet]
    public async Task<ActionResult<string>> ListChatRooms([FromQuery] long offset, [FromQuery] long limit)
    {
        _logger.LogInformation("Get ChatRooms with offset: {offset}, limit: {limit}", offset, limit);

        return "test";
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateChatRoom()
    {
        _logger.LogInformation("Create a ChatRoom with dto: {dto}");

        return "test";
    }

    [HttpGet("{id}/join")]
    public async Task<ActionResult<string>> JoinChatRoom([FromRoute] long id)
    {
        // TODO: add websocket
        // reference: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-9.0
        var userIpAddress = GetUserIPAddress();

        _logger.LogInformation("User {ip} joins ChatRoom {id}", userIpAddress, id);

        return "test";
    }

    private string? GetUserIPAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
