using Microsoft.AspNetCore.Mvc;
using RacingAidGrpc;

namespace RacingAidDataInjector.Controllers;

public class TelemetryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken] // Important for security with POST requests from forms
    public async Task<IActionResult> BroadcastSessionStatus(bool newStatus)
    {
        // Call the static method on your gRPC service to broadcast the status
        await TelemetryService.BroadcastSessionStatus(newStatus);
        
        // You can add a TempData message to show success
        TempData["Message"] = $"Session status updated to: {newStatus}";

        // Redirect back to the page where the button was, or to a dashboard
        return RedirectToAction("Index"); 
        // Or if you prefer to return to a different view/action: return RedirectToAction("SomeOtherAction");
    }
}