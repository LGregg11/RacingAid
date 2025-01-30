using System.Collections.ObjectModel;
using System.Windows;
using RacingAidData.Core.Models;
using RacingAidWpf.Configuration;
using RacingAidWpf.Extensions;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class RelativeOverlayViewModel : OverlayViewModel
{
    private static readonly RelativeConfigSection RelativeConfigSection = ConfigSectionSingleton.RelativeSection;
    
    private ObservableCollection<RelativeGridRow> relative = [];
    public ObservableCollection<RelativeGridRow> Relative
    {
        get => relative;
        private set
        {
            if (relative == value)
                return;
            
            relative = value;
            OnPropertyChanged();
        }
    }
    
    #region Visibility properties

    public Visibility CarNumberColumnVisibility => RelativeConfigSection.DisplayCarNumber.ToVisibility();
    public Visibility SafetyColumnVisibility => RelativeConfigSection.DisplaySafetyRating.ToVisibility();
    public Visibility SkillColumnVisibility => RelativeConfigSection.DisplaySkillRating.ToVisibility();
    public Visibility LastLapColumnVisibility => RelativeConfigSection.DisplayLastLap.ToVisibility();
    public Visibility FastestLapColumnVisibility => RelativeConfigSection.DisplayFastestLap.ToVisibility();
    public Visibility DeltaToLocalColumnVisibility => RelativeConfigSection.DisplayGapToLocal.ToVisibility();
    
    #endregion
    
    public RelativeOverlayViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;

        RelativeConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    private void UpdateProperties()
    {
        UpdateDriversDataGrid();
    }

    private void UpdateDriversDataGrid()
    {
        // We need to order the drivers by their current lap distance and determine the delta from the current (local) driver's lap distance
        // Because the current driver could be 0.9 (90%) of the way through the lap, or in the pits, and you'd want
        // to see who is behind/ ahead of you.
        // i.e. local at 0.9, other at 0.6 -> -0.3 lap distance behind, local at 0.3, other at 0.9 -> -0.4 lap distance behind

        var relativeModel = RacingAidSingleton.Instance.Relative;
        var relativeModelEntries = relativeModel.Entries;

        if (relativeModelEntries.Count == 0)
        {
            Relative = [];
            return;
        }
        
        var currentDriver = relativeModel.LocalEntry ?? relativeModelEntries.First();
        var newRelativeGridRows = CreateOrderedRelativeGridRowsByLapDistance(relativeModelEntries, currentDriver);

        var entriesAheadOrBehind = RelativeConfigSection.MaxPositionsAheadOrBehind;
        var currentDriverIndex = newRelativeGridRows.FindIndex(r => r.CarNumber == currentDriver.CarNumber);
        var minEntryIndex = Math.Max(currentDriverIndex - entriesAheadOrBehind, 0);
        var maxEntryIndex = Math.Min(currentDriverIndex + entriesAheadOrBehind + 1, newRelativeGridRows.Count - 1);

        var relativeGridRowsToDisplay = newRelativeGridRows.GetRange(minEntryIndex, maxEntryIndex - minEntryIndex);

        Relative = new ObservableCollection<RelativeGridRow>(relativeGridRowsToDisplay);
    }

    private void OnConfigUpdated()
    {
        OnPropertyChanged(nameof(CarNumberColumnVisibility));
        OnPropertyChanged(nameof(SafetyColumnVisibility));
        OnPropertyChanged(nameof(SkillColumnVisibility));
        OnPropertyChanged(nameof(LastLapColumnVisibility));
        OnPropertyChanged(nameof(FastestLapColumnVisibility));
        OnPropertyChanged(nameof(DeltaToLocalColumnVisibility));
    }

    private List<RelativeGridRow> CreateOrderedRelativeGridRowsByLapDistance(List<RelativeEntryModel> relativeModelEntries, RelativeEntryModel currentDriver)
    {
        const float centerPercentage = 0.5f;
        var lapDistancePercentageDelta = currentDriver.LapDistancePercentage - centerPercentage;
        
        var updatedRelativeModeEntries = new List<RelativeGridRow>();
        
        foreach (var relativeModelEntry in relativeModelEntries)
        {
            var updatedLapDistancePercentage = relativeModelEntry.LapDistancePercentage - lapDistancePercentageDelta;
            switch (updatedLapDistancePercentage)
            {
                case < 0:
                    updatedLapDistancePercentage += 1;
                    break;
                case > 1:
                    updatedLapDistancePercentage -= 1;
                    break;
            }

            var relativeGridRow = RelativeModelToGridRow(relativeModelEntry);
            relativeGridRow.LapDistancePercentage = updatedLapDistancePercentage;
            
            updatedRelativeModeEntries.Add(relativeGridRow);
        }

        return updatedRelativeModeEntries.OrderByDescending(e => e.LapDistancePercentage).ToList();
    }

    private static RelativeGridRow RelativeModelToGridRow(RelativeEntryModel relativeEntryModel)
    {
        return new RelativeGridRow(
            relativeEntryModel.OverallPosition,
            relativeEntryModel.ClassPosition,
            relativeEntryModel.FullName,
            relativeEntryModel.SkillRating,
            relativeEntryModel.SafetyRating,
            relativeEntryModel.CarModel,
            relativeEntryModel.CarNumber,
            relativeEntryModel.LastLapMs,
            relativeEntryModel.FastestLapMs,
            relativeEntryModel.GapToLocalMs,
            relativeEntryModel.LapDistancePercentage,
            relativeEntryModel.IsLocal,
            false);
    }
}