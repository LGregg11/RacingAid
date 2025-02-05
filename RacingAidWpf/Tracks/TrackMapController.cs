using System.IO;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Logging;
using RacingAidWpf.Resources;

namespace RacingAidWpf.Tracks;

public class TrackMapController
{
    private static readonly string TrackMapsJsonFullPath =
        Path.Combine(Resource.DataDirectory, "TrackMaps.json");
    
    private readonly IHandleData<TrackMaps> trackMapDataHandler;
    private readonly List<TrackMap> trackMaps = [];

    private readonly ILogger logger;

    public TrackMapController(IHandleData<TrackMaps> trackMapDataHandler, ILogger logger = null)
    {
        this.logger = logger ?? LoggerFactory.GetLogger<TrackMapController>();
        
        this.trackMapDataHandler = trackMapDataHandler;
        if (!trackMapDataHandler.TryDeserializeFromFile(TrackMapsJsonFullPath, out var trackMapData))
        {
            this.logger?.LogError($"No track map data found at '{TrackMapsJsonFullPath}'");
            return;
        }
        
        trackMaps = trackMapData.Maps;
    }
    
    public bool TryGetTrackMap(string trackName, out TrackMap trackMap)
    {
        trackMap = trackMaps.FirstOrDefault(m => m.Name == trackName);
        return trackMap != null;
    }

    public void AddTrackMap(TrackMap trackMap, bool forceReplace = false)
    {
        // We don't want to override unless force save is applied
        if (trackMaps.FirstOrDefault(m => m.Name == trackMap.Name) is { } existingTrackMap)
        {
            if (forceReplace)
            {
                logger?.LogInformation($"Replacing existing track map data for '{trackMap.Name}'");
                trackMaps.Remove(existingTrackMap);
            }
            else
                return;
        }
        
        logger?.LogInformation($"Added track map data for '{trackMap.Name}'");
        trackMaps.Add(trackMap);
        
        if (!trackMapDataHandler.TrySerializeToFile(TrackMapsJsonFullPath, new TrackMaps(trackMaps)))
            logger?.LogError($"Failed to serialize track map data for '{trackMap.Name}' to {TrackMapsJsonFullPath}");
        else
            logger?.LogInformation($"Saved {trackMap.Name} track data to {TrackMapsJsonFullPath}");
    }
}