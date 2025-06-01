using Newtonsoft.Json;

namespace RacingAidData.Core.Replay;

public static class ReplaySettings
{
    public static JsonSerializerSettings DefaultJsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
}