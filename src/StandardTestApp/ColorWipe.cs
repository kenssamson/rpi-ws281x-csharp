using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace CoreTestApp
{
    public class ColorWipe : IAnimation
    {
        public void Execute(AbortRequest request)
        {
            Console.Clear();
            Console.Write("How many LEDs do you want to use: ");

            var ledCount = Int32.Parse(Console.ReadLine());
            var settings = Settings.CreateDefaultSettings();

            var controller = settings.AddController(ledCount, Pin.Gpio18, StripType.WS2811_STRIP_RGB);
            using (var device = new WS281x(settings))
            {
                while (!request.IsAbortRequested)
                {
                    Wipe(device, Color.Red);
                    Wipe(device, Color.Green);
                    Wipe(device, Color.Blue);
                }
                device.Reset();
            }
        }

        private static void Wipe(WS281x device, Color color)
        {
            var controller = device.GetController();
            foreach (var led in controller.LEDs)
            {
                led.Color = color;
                device.Render();

                // wait for a minimum of 5 milliseconds
                var waitPeriod = (int)Math.Max(500.0 / controller.LEDCount, 5.0); 

                Thread.Sleep(waitPeriod);
            }
        }
    }
}