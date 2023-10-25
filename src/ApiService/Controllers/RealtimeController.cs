[ApiController]
[Route("realtime")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Policy = "HasRealtimeScopeClaim")]
public class RealtimeController : Controller
{
    [HttpPost]
    public async Task<IActionResult> ProcessRealtimeEvent(
    )
    {
        throw new NotImplementedException();
    }
}
