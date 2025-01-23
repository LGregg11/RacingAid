namespace RacingAidData.Core.Models;

public class RelativeModel : RaceDataModel
{
    public RelativeEntryModel? LocalEntry { get; init; }

    public List<RelativeEntryModel> Entries { get; init; } = [];
}