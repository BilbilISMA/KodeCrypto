using Hangfire;
using Microsoft.AspNetCore.Mvc;

public class JobController : Controller
{
    private readonly IBackgroundJobService _backgroundJobService;

    public JobController(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public IActionResult SyncProtofolioUserDataJob()
    {
        // Enqueue a background job
        BackgroundJob.Enqueue(() => _backgroundJobService.SyncProtofolioUserDataJob());

        return Ok("Background job enqueued!");
    }
}
