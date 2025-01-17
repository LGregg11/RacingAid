﻿using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using RacingAidData;
using RacingAidData.Simulators;

namespace RacingAidWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    private readonly RacingAid racingAid = new();
    
    private readonly Thread updateThread;
    private bool keepUpdating;
    
    public string Speed { get; set; } = "0.0 m/s";
    public string Brake { get; set; } = "0 %";
    public string Throttle { get; set; } = "0 %";
    public string Gear { get; set; } = "0";
    public string SteeringAngle { get; set; } = "0 deg";
    public string DriverName { get; set; } = "Test Name";
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        
        racingAid.SetupSimulator(Simulator.IRacing);
        racingAid.Start();

        keepUpdating = true;
        updateThread = new Thread(UpdateLoop);
        updateThread.Start();
    }

    ~MainWindow()
    {
        racingAid.Stop();
        
        keepUpdating = false;
        updateThread.Join();
    }

    private void UpdateLoop()
    {
        while (keepUpdating)
        {
            if (Application.Current?.Dispatcher is { } dispatcher)
                dispatcher.Invoke(UpdateProperties);
            
            Thread.Sleep(100);
        }
    }

    private void UpdateProperties()
    {
        Speed = $"{racingAid.Telemetry.SpeedMetresPerSecond.ToString(CultureInfo.InvariantCulture)} m/s";
        OnPropertyChanged(nameof(Speed));
        
        Brake = $"{racingAid.Telemetry.BrakePercentage.ToString(CultureInfo.InvariantCulture)} %";
        OnPropertyChanged(nameof(Brake));
        
        Throttle = $"{racingAid.Telemetry.ThrottlePercentage.ToString(CultureInfo.InvariantCulture)} %";
        OnPropertyChanged(nameof(Throttle));
        OnPropertyChanged(nameof(Brake));
        
        Gear = $"{racingAid.Telemetry.Gear.ToString(CultureInfo.InvariantCulture)}";
        OnPropertyChanged(nameof(Gear));
        
        SteeringAngle = $"{racingAid.Telemetry.SteeringAngleDegrees.ToString(CultureInfo.InvariantCulture)} deg";
        OnPropertyChanged(nameof(SteeringAngle));
        
        var fullName = racingAid.Drivers.LocalDriver.FullName;
        if (!string.IsNullOrEmpty(fullName))
        {
            DriverName = fullName;
            OnPropertyChanged(nameof(DriverName));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}