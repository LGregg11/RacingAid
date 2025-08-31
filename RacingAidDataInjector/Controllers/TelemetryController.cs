using Microsoft.AspNetCore.Mvc;
using RacingAidGrpc;

namespace RacingAidDataInjector.Controllers;

public class TelemetryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> BroadcastSessionStatus(bool newStatus)
    {
        TelemetryService.BroadcastSessionStatus(newStatus);
        return RedirectToAction("Index");
    }
}