using RacingAidData.Core.Models;
using RacingAidWpf.Model;

namespace RacingAidWpf.Core.Timesheets;

public abstract class TimesheetController<T> where T : TimesheetEntryModel
{
    public List<TimesheetInfo> Entries { get; protected set; } = [];

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

    private List<TimesheetInfo> CreateTimesheetEntries(TimesheetModel<T> timesheetData)
    {
        List<TimesheetInfo> timesheetInfoEntries = [];
        timesheetInfoEntries.AddRange(timesheetData.Entries.Select(CreateTimesheetInfo));

        return timesheetInfoEntries;
    }
}