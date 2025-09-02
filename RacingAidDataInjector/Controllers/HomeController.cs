using Microsoft.AspNetCore.Mvc;
using RacingAidGrpc;

namespace RacingAidDataInjector.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> BroadcastSessionStatus(bool newStatus)
    {
        await TelemetryService.BroadcastSessionStatus(new SessionStatusResponse { SessionActive = newStatus });
        
        return RedirectToAction("Index");
    }
}