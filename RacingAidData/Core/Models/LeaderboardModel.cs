namespace RacingAidData.Core.Models;

public sealed class LeaderboardModel : RaceDataModel
{
    public LeaderboardEntryModel? LocalEntry { get; init; }

    public List<LeaderboardEntryModel> Entries { get; init; } = [];
}