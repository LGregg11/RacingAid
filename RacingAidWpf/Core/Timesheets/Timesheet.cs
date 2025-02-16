using RacingAidData.Core.Models;
using RacingAidWpf.Core.Logging;
using RacingAidWpf.Model;

namespace RacingAidWpf.Core.Timesheets;

public abstract class Timesheet<T>(ILogger logger = null) where T : TimesheetEntryModel
{
    protected ILogger Logger { get; } = logger ?? LoggerFactory.GetLogger<Timesheet<T>>();

    protected List<TimesheetInfo> Entries { get; private set; } = [];

    public void Clear()
    {
        Entries?.Clear();
    }

    public void UpdateFromData(TimesheetModel<T> timesheetData)
    {
        if (timesheetData == null || timesheetData.Entries.Count == 0)
        {
            Clear();
            return;
        }
        
        Entries = CreateTimesheetEntries(timesheetData);
    }
    
    protected abstract TimesheetInfo CreateTimesheetInfo(T timesheetEntryData);

    protected virtual List<TimesheetInfo> CreateTimesheetEntries(TimesheetModel<T> timesheetData)
    {
        List<TimesheetInfo> timesheetInfoEntries = [];
        timesheetInfoEntries.AddRange(timesheetData.Entries.Select(CreateTimesheetInfo));

        return timesheetInfoEntries;
    }
}