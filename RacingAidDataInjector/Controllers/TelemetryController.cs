using Microsoft.AspNetCore.Mvc;

namespace RacingAidDataInjector.Controllers;

public class TelemetryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}