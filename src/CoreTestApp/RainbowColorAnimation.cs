using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace CoreTestApp
{
    public class RainbowColorAnimation : IAnimation
    {
        private static int colorOffset = 0;
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
                var colors = GetAnimationColors();
                while (!request.IsAbortRequested)
                {
                    for (int i = 0; i < controller.Settings.Channels[channelNumber].LEDCount; i++)
                    {
                        var colorIndex = (i + colorOffset) % colors.Count;
                        controller.SetLEDColor(channelNumber, i, colors[colorIndex]);
                    }
                    controller.Render();
                    colorOffset = (colorOffset + 1) % colors.Count;

                    Thread.Sleep(500);
                }
                controller.Reset();
            }
        }

        public static List<Color> GetAnimationColors()
        {
            var result = new List<Color>();

            result.Add(Color.Red);
            result.Add(Color.DarkOrange);
            result.Add(Color.Yellow);
            result.Add(Color.Green);
            result.Add(Color.Blue);
            result.Add(Color.Purple);
            result.Add(Color.DeepPink);

            return result;
        }

    }
}