namespace RacingAidData.Core.Models;

public class TimesheetModel<T> : RaceDataModel where T : TimesheetEntryModel
{
    public T? LocalEntry { get; init; }

    public List<T> Entries { get; init; } = [];
}