using IRSDKSharper;
using RacingAidData.Core.Deserializers;
using RacingAidData.Core.Models;

namespace RacingAidData.Simulators.Debug;

public class DebugDeserializer : IDeserializeData
{
    public bool TryDeserializeData(object data, out List<RaceDataModel> models)
    {
        models = [];
        
        return models.Count > 0;
    }

    private static List<LeaderboardEntryModel> CreateTimesheetEntries(IRacingSdkData iRacingData)
    {
        return [];
    }

    private static List<RelativeEntryModel> CreateRelativeEntries(IRacingSdkData iRacingData)
    {
        return [];
    }

    private static RaceDataModel CreateTelemetryModel(IRacingSdkData iRacingData)
    {
        return new TelemetryModel();
    }

    private static DriverDataModel CreateDriverModel(IRacingSdkData iRacingData)
    {
        return new DriverDataModel();
    }
}