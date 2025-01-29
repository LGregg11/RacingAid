using EnumsNET;

namespace RacingAidWpf.Model;

public class EnumEntryModel<T>(T value) where T: struct, Enum
{
    public string Name { get; } = value.AsString(EnumFormat.Description);
    public T Value { get; } = value;
}