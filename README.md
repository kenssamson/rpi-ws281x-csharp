[![Nuget](https://img.shields.io/nuget/v/kenssamson.rpi-ws281x-csharp.svg?label=version&style=popout)](ReleaseNotes.md)
![Framework](https://img.shields.io/static/v1.svg?label=.NET&nbsp;Standard&message=v2.0&color=blue)
[![License](https://img.shields.io/badge/License-BSD%202--Clause-orange.svg)](https://opensource.org/licenses/BSD-2-Clause)
[![Nuget](https://img.shields.io/nuget/dt/kenssamson.rpi-ws281x-csharp.svg?color=brightgreen&style=popout)](https://www.nuget.org/packages/digitalhigh.rpi-ws281x-csharp/)

# rpi-ws281x-csharp
This fork contains the following changes:
* Add proper channel creation for various GPIO Pins, auto controller selection based on GPIO PIN input
* Add clamping method for RGB+W LEDs to return proper white value and avoid brownouts
* Simplify strip creation/usage by removing need to call the controller directly
* Add on-the-fly changes for strip brightness and count without needing to re-initialize the strip
* Added fix for [.Net Core Exception Issue](https://github.com/rpi-ws281x/rpi-ws281x-csharp/issues/2).
* Added default Gamma Correction Map based on [this Adafruit document](https://learn.adafruit.com/led-tricks-gamma-correction/the-issue).
* Added support for additional LED strips.



Please review latest [Release Notes](ReleaseNotes.md) for most recent changes.
##

This is a C# wrapper for the great C assembly by Jeremy Garff to control WS281X LEDs by a Raspberry PI ([https://github.com/jgarff/rpi_ws281x](https://github.com/jgarff/rpi_ws281x)).
It uses the P/Invoke calls to access the native assembly.

## Usage
It is very easy to use the wrapper in your own C# / .NET project.
Just see the example below:

```csharp
// The default settings uses a frequency of 800000 Hz and the DMA channel 10, and enables gamma correction.
// Pass false to the constructor to disable gamma correction.
var settings = Settings.CreateDefaultSettings(false);

//Use 16 LEDs and GPIO Pin 18.
//Set brightness to maximum (255)
//Use Unknown as strip type. Then the type will be set in the native assembly.
var controller = settings.AddController(16, Pin.Gpio18, StripType.WS2812_STRIP, 255, false)

using (var rpi = new WS281x(settings))
{
  // Set the color of the first LED of controller 0 to blue
  rpi.SetLED(0, Color.Blue);
  //Set the color of the second LED of controller 0 to red
  rpi.SetLED(1, Color.Red);
  rpi.Render();
  // Retrieve the existing brightness
  var brightness = rpi.GetBrightness();
  // Set the strip brightness to 128 - render is automatically called
  rpi.SetBrightness(128);
  // Retrieve existing LED Count
  var ledCount = rpi.GetLedCount();
  // Update the strip count to 32 - render is automatically called
  rpi.SetLedCount(32);
  // Set all the LED colors to Red - render is automatically called
  rpi.SetAll(Color.Green);
  // Reset the strip and destroy it
  rpi.Rest();
  rpi.Dispose();
}
```
Please have a look at the [example program](src/CoreTestApp/Program.cs) and get familiar with the usage.

## Installation
The library can be downloaded from NuGet: [![Nuget](https://img.shields.io/nuget/v/kenssamson.rpi-ws281x-csharp.svg?style=popout)](https://www.nuget.org/packages/digitalhigh.rpi-ws281x-csharp)

**Package Manager**
> PM\> Install-Package digitalhigh.rpi-ws281x-csharp

**.NET CLI** 
> \> dotnet add package digitalhigh.rpi-ws281x-csharp


In order to get the wrapper working, you need build the shared C library first.  Please refer to the 
README in the [lib](lib) folder for more information.

The P/Invoke functionality has a [special search pattern](http://www.mono-project.com/docs/advanced/pinvoke/#library-handling) to find the required assembly.
For my tests, I copied the ws2811.so assembly to /usr/lib folder (as suggested by the link above).

## Test status
The wrapper was tested with following setup:

| Raspberry model   | Controller | GPIO Pin | DMA channel | Result |
|-------------------|------------|----------|-------------|--------|
| 3 Model B Rev 2   | WS2812B    | 18       | 5, 10       | Success|
| 4 Model B Rev 2   | WS2812B    | 18       | 5, 10       | Success|

Please feel free to add some more test cases.
