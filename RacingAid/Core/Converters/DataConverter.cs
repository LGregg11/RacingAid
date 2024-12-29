using RacingAid.Core.Models;

namespace RacingAid.Core.Converters;

/// <summary>
/// Abstract class for converting data into models
/// </summary>
public abstract class DataConverter
{
    /// <summary>
    /// Create a driver model from the given data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract DriverModel DataToDriverModel(byte[] data);

    /// <summary>
    /// Create a telemetry model from the given data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract TelemetryModel DataToTelemetryModel(byte[] data);
}