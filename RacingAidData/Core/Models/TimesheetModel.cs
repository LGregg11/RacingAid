namespace RacingAidData.Core.Models;

public sealed class TimesheetModel : RaceDataModel
{
    public TimesheetEntryModel? LocalEntry { get; init; }

    public List<TimesheetEntryModel> Entries { get; init; } = [];
}