using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using PersistenceService.Stores;
using ApiService.Kafka.Producer;

namespace ApiService.Controllers;

[ApiController]
[Route("realtime")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Policy = "HasRealtimeScopeClaim")]
public class RealtimeController : Controller
{
    private IServiceProvider Provider { get; set; }
    private KafkaProducer KafkaProducer { get; set; }

    public RealtimeController(
        IServiceProvider serviceProvider,
        KafkaProducer kafkaProducer
    )
        : base()
    {
        Provider = serviceProvider;
        KafkaProducer = kafkaProducer;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessRealtimeEvent(
        [FromBody] RealtimeEventDetails realtimeEvent
    )
    {
        if (realtimeEvent.Type == RealtimeEvent.WorkspaceSignin)
        {
            try
            {
                await DbSignin(
                    Guid.Parse(realtimeEvent.UserId),
                    Guid.Parse(realtimeEvent.WorkspaceId)
                );
                KafkaProducer.ProduceMessageEvent(
                    "signin",
                    $"user:{realtimeEvent.UserId},workspace:{realtimeEvent.WorkspaceId},"
                );
                return Ok("asdf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        throw new NotImplementedException();
    }

    private async Task DbSignin(Guid userId, Guid workspaceId)
    {
        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        await workspaceStore.SignInUser(userId, workspaceId);
    }

    private async Task DbSignout(Guid userId, Guid workspaceId)
    {
        throw new NotImplementedException();
    }
}

public struct RealtimeEventDetails
{
    public RealtimeEvent Type { get; set; }
    public string UserId { get; set; }
    public string WorkspaceId { get; set; }
    public string? GroupId { get; set; }
    public string? Content { get; set; }
    public List<string>? Headers { get; set; }
}

public enum RealtimeEvent
{
    WorkspaceSignin,
    WorkspaceSignout,
}
