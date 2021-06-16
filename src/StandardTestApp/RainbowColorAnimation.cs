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

        public void Execute(AbortRequest request, int gpioPin){
            Console.Clear();
            Console.Write("How many LEDs do you want to use: ");

            var ledCount = Int32.Parse(Console.ReadLine());
            var settings = Settings.CreateDefaultSettings();
            var pin = Pin.Gpio18;
            if (gpioPin == 19) {
                pin = Pin.Gpio19;
            }

            if (gpioPin == 10) {
                pin = Pin.Gpio10;
            }

            if (gpioPin == 21) {
                pin = Pin.Gpio21;
            }
            var controller = settings.AddController(ledCount, pin, StripType.WS2811_STRIP_RGB);

            using var device = new WS281x(settings);
            var colors = GetAnimationColors();
            while (!request.IsAbortRequested)
            {
                for (var i = 0; i < controller.LEDCount; i++)
                {
                    var colorIndex = (i + colorOffset) % colors.Count;
                    device.SetLed(i, colors[colorIndex]);
                }
                device.Render();
                colorOffset = (colorOffset + 1) % colors.Count;

                Thread.Sleep(500);
            }
            device.Reset();
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