namespace RacingAidWpf.FileHandlers;

public interface IHandleData<T> where T : class
{
    public bool TryDeserializeFromFile(string filePath, out T? data);
    public bool TrySerializeToFile(string filePath, T data);
}
