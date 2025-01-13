using RacingAid.Core.Models;

namespace RacingAid.Core.Deserializers;

/// <summary>
/// Interface for classes that will deserialize data into <see cref="RaceDataModel"/>s
/// </summary>
public interface IDeserializeData
{
    public List<DataType> ValidDataTypes { get; }

    public bool TryDeserializeData(byte[] data, out List<RaceDataModel>? models);
}