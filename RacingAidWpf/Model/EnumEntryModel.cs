using System.Collections.ObjectModel;
using EnumsNET;

namespace RacingAidWpf.Model;

public class EnumEntryModel<T>(T value) where T: struct, Enum
{
    public string Name { get; } = value.AsString(EnumFormat.Description);
    public T Value { get; } = value;
}

public static class EnumEntryModelUtility
{
    public static ObservableCollection<EnumEntryModel<T>> CreateObservableEnumCollection<T>() where T : struct, Enum
    {
        var entries = new ObservableCollection<EnumEntryModel<T>>();
        foreach (var entry in Enum.GetValues(typeof(T)).Cast<T>())
            entries.Add(new EnumEntryModel<T>(entry));
        return entries;
    }
}