# RacingAid

*This application is still under development, but pre-release versions are available for
anyone that wishes to give it a trial run*

RacingAid is a free, open-source overlay application designed for sim drivers who want a
simple overlay that can be used for a variety of racing games

*NOTE: RacingAid currently only supports iRacing, but support for more racing games is
currently under development.*

### How to use

Simply download the [latest release](https://img.shields.io/github/v/release/LGregg11/RacingAid)

### Required setup

#### iRacing

Go to the iRacing app.ini (Can be found at C:\Users\<user>\documents\iRacing\app.ini)
and under `[Misc]` set `irsdkEnableMem=1`. Make sure to save and restart the application.

## For developers

### RacingAidWpf

RacingAidWpf is a WPF application that displays a variety of information as an overlay 
over a racing game

### RacingAidData

RacingAidData is an open-source .NET Library for developers that want a seamless way to
create an application for a variety of racing games. The library contains a set of data 
subscription classes that feed data output from the desired racing game and deserializes
the information into generic data models.

The aim of this library is to make it easy for developers to create their applications 
using the supported subscribers and deserializers, or to produce their own and for 
unsupported games and inject them into the RacingAid application and still use the same
generic models for the output (NOTE: This latter feature is not currently implemented).

#### Using RacingAid as a developer

Below is a basic example of how you can use the `RacingAidData` .NET library in your
application to easily obtain iRacing data.

```C#
var racingAid = new RacingAid();
racingAid.SetupSimulator(Simulator.iRacing);
racingAid.ModelsUpdated += OnModelsUpdated;
racingAid.Start();

..

private void OnModelsUpdated()
{
    Console.WriteLine($"Driver's latest speed (m/s): {racingAid.Telemetry.SpeedMs}");
}

```

