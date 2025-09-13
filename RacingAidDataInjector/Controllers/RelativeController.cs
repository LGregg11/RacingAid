using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Mvc;
using RacingAidDataInjector.Models;
using RacingAidGrpc;
using Color = System.Drawing.Color;

namespace RacingAidDataInjector.Controllers;

public class RelativeController : Controller
{
    public IActionResult Index()
    {
        var drivers = PopulateDrivers(10);

        var viewModel = new RelativeViewModel { Drivers = drivers };
        return View(viewModel);
    }

    private static List<RelativeEntryModel> PopulateDrivers(int nDrivers)
    {
        var drivers = new List<RelativeEntryModel>();
        for (var i = 0; i < nDrivers; i++)
            drivers.Add(CreateRelativeEntryModel(i));

        return drivers;
    }

    private static RelativeEntryModel CreateRelativeEntryModel(int i)
    {
        return new RelativeEntryModel
        {
            ClassPosition = i + 1,
            OverallPosition = i + 1,
            FullName = $"Driver {i + 1}",
            CarModel = "ABC",
            CarNumber = i + 1,
            InPits = false,
            IsLocal = i == 0,
            LapsDriven = 1.23f - (i / 100f),
            LastLapMs = 123400,
            SkillRating = $"{2001 + i}",
            SafetyRating = $"A (3.{i + 1})",
            FastestLapMs = 0,
            GapToLocalMs = 0
        };
    }

    [HttpPost]
    public async Task<IActionResult> BroadcastRelative(RelativeViewModel model)
    {
        var response = RelativeResponseFromModel(model);
        await TelemetryService.BroadcastRelative(response);
        
        return RedirectToAction("Index");
    }

    private static RelativeResponse RelativeResponseFromModel(RelativeViewModel model)
    {
        var drivers = new RepeatedField<RelativeDriver>();

        foreach (var modelDriver in model.Drivers)
        {
            var timesheetEntry = TimesheetEntryFromModel(modelDriver);
            drivers.Add(new RelativeDriver
            {
                GapToLocalMs = modelDriver.GapToLocalMs,
                TimesheetEntry = timesheetEntry,
            });
        }

        var response = new RelativeResponse();
        response.Drivers.AddRange(drivers);

        return response;
    }

    private static TimesheetEntry TimesheetEntryFromModel(TimesheetEntryModel model)
    {
        var safetyRatingCode = model.SafetyRating;
        var safetyRatingColor = Color.Red;
        if (safetyRatingCode.Contains("A"))
            safetyRatingColor = Color.Blue;
        if (safetyRatingCode.Contains("B"))
            safetyRatingColor = Color.Green;
        if (safetyRatingCode.Contains("C"))
            safetyRatingColor = Color.Yellow;
        if (safetyRatingCode.Contains("D"))
            safetyRatingColor = Color.Orange;
        
        return new TimesheetEntry
        {
            OverallPosition = model.OverallPosition,
            ClassPosition = model.ClassPosition,
            FullName = model.FullName,
            CarModel = model.CarModel,
            CarNumber = model.CarNumber,
            InPits = model.InPits,
            IsLocal = model.IsLocal,
            LapsDriven = model.LapsDriven,
            SkillRating = model.SkillRating,
            SafetyRating = new SafetyRating
            {
                Rating = model.SafetyRating,
                Color = new RacingAidGrpc.Color
                {
                    ARGB = safetyRatingColor.ToArgb()
                }
            },
            FastestLapMs = model.FastestLapMs,
            LastLapMs = model.LastLapMs
        };
    }
}