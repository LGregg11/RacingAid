using Newtonsoft.Json;

namespace RacingAidData.Core.Replay;

public static class ReplaySettings
{
    public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
}