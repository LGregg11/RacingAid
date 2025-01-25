using System.IO;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Resources;

namespace RacingAidWpf.TrackMaps;

public class TrackMapController
{
    private static readonly string TrackMapsJsonFullPath =
        Path.Combine(Resource.DataDirectory, "TrackMaps.json");
    
    private readonly IHandleData<TrackMaps> trackMapDataHandler;
    private readonly List<TrackMap> trackMaps = [];

    public TrackMapController(IHandleData<TrackMaps> trackMapDataHandler)
    {
        this.trackMapDataHandler = trackMapDataHandler;
        if (trackMapDataHandler.TryDeserializeFromFile(TrackMapsJsonFullPath, out var trackMapData))
            trackMaps = trackMapData.Maps;
    }
    
    public bool TryGetTrackMap(string trackName, out TrackMap? trackMap)
    {
        trackMap = trackMaps.FirstOrDefault(m => m.Name == trackName);
        return trackMap != null;
    }

    public void SaveTrackMap(TrackMap trackMap)
    {
        if (trackMaps.Any(m => m.Name == trackMap.Name))
            return;
        
        trackMaps.Add(trackMap);
        if (!trackMapDataHandler.TrySerializeToFile(TrackMapsJsonFullPath, new TrackMaps(trackMaps)))
        {
            // TODO: Add error log here
        }
    }
}