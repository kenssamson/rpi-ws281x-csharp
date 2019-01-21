using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace CoreTestApp
{
    public class ColorWipe : IAnimation
    {
        private static int channelNumber = 0;

        public void Execute(AbortRequest request)
        {
            Console.Clear();
            Console.Write("How many LEDs do you want to use: ");

            var ledCount = Int32.Parse(Console.ReadLine());
            var settings = Settings.CreateDefaultSettings();

            settings.Channels[channelNumber] = new Channel(ledCount, 18, 255, false, StripType.WS2811_STRIP_RGB);

            using (var controller = new WS281x(settings))
            {
                while (!request.IsAbortRequested)
                {
                    Wipe(controller, Color.Red);
                    Wipe(controller, Color.Green);
                    Wipe(controller, Color.Blue);
                }
                controller.Reset();
            }
        }

        private static void Wipe(WS281x controller, Color color)
        {
            for (int i = 0; i < controller.Settings.Channels[channelNumber].LEDCount; i++)
            {
                controller.SetLEDColor(channelNumber, i, color);
                controller.Render();
                Thread.Sleep(500);
            }
        }
    }
}