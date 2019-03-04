[![Nuget](https://img.shields.io/nuget/dt/kenssamson.rpi-ws281x-csharp.svg?color=brightgreen&style=popout)](https://www.nuget.org/packages/kenssamson.rpi-ws281x-csharp/)
[![License](https://img.shields.io/badge/License-BSD%202--Clause-orange.svg)](https://opensource.org/licenses/BSD-2-Clause)

# rpi-ws281x-csharp
This fork contains the following changes:

* Added fix for [.Net Core Exception Issue](https://github.com/rpi-ws281x/rpi-ws281x-csharp/issues/2).
* Added default Gamma Correction Map based on [this Adafruit document](https://learn.adafruit.com/led-tricks-gamma-correction/the-issue).
* Added support for additional LED strips.
* Added new [.Net Core Sample Program](src/CoreTestApp)
##

This is a C# wrapper for the great C assembly by Jeremy Garff to control WS281X LEDs by a Raspberry PI ([https://github.com/jgarff/rpi_ws281x](https://github.com/jgarff/rpi_ws281x)).
It uses the P/Invoke calls to access the native assembly.

## Usage
It is very easy to use the wrapper in your own C# / .NET project.
Just see the example below:

```csharp
//The default settings uses a frequency of 800000 Hz and the DMA channel 10.
var settings = Settings.CreateDefaultSettings();

//Use 16 LEDs and GPIO Pin 18.
//Set brightness to maximum (255)
//Use Unknown as strip type. Then the type will be set in the native assembly.
var controller = settings.AddController(16, Pin.Gpio18, StripType.WS2812_STRIP, ControllerType.PWM0, 255, false)

using (var rpi = new WS281x(settings))
{
  //Set the color of the first LED of controller 0 to blue
  controller.SetLED(0, Color.Blue);
  //Set the color of the second LED of controller 0 to red
  controller.SetLED(1, Color.Red);
  rpi.Render();
}
```
Please have a look at the [example program](src/CoreTestApp/Program.cs) and get familiar with the usage.

## Installation
The library can be downloaded from NuGet: [![Nuget](https://img.shields.io/nuget/v/kenssamson.rpi-ws281x-csharp.svg?style=popout)](https://www.nuget.org/packages/kenssamson.rpi-ws281x-csharp/)

**Package Manager**
> PM\> Install-Package kenssamson.rpi-ws281x-csharp

**.NET CLI** 
> \> dotnet add package kenssamson.rpi-ws281x-csharp


In order to get the wrapper working, you need build the shared C library first.  Please refer to the 
README in the [lib](lib) folder for more information.

The P/Invoke functionality has a [special search pattern](http://www.mono-project.com/docs/advanced/pinvoke/#library-handling) to find the required assembly.
For my tests, I copied the ws2811.so assembly to /usr/lib folder (as suggested by the link above).

## Test status
The wrapper was tested with following setup:

|Raspberry model | Controller | GPIO Pin | DMA channel | Result |
|----------------|------------|----------|-------------|--------|
|Model B Rev 2   | WS2812B    | 18       | 5, 10       | Success|

Please feel free to add some more test cases.
