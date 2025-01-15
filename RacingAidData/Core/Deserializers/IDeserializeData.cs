using RacingAidData.Core.Models;

namespace RacingAidData.Core.Deserializers;

/// <summary>
/// Interface for classes that will deserialize data into <see cref="RaceDataModel"/>s
/// </summary>
public interface IDeserializeData
{
    public bool TryDeserializeData(object data, out List<RaceDataModel> models);
}