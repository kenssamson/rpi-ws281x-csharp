using rpi_ws281x;
using System;
using System.Drawing;

namespace TestApp
{
    class ColorWipe : IAnimation
	{
		public void Execute(AbortRequest request)
		{
			Console.Clear();
			Console.Write("How many LEDs to you want to use: ");

			var ledCount = Int32.Parse(Console.ReadLine());

			//The default settings uses a frequency of 800000 Hz and the DMA channel 10.
			var settings = Settings.CreateDefaultSettings();

            //Set brightness to maximum (255)
            //Use Unknown as strip type. Then the type will be set in the native assembly.
            settings.AddController(ledCount, Pin.Gpio18, StripType.WS2812_STRIP, ControllerType.PWM0, 255, false);

			using (var device = new WS281x(settings))
			{
				while(!request.IsAbortRequested)
				{
					Wipe(device, Color.Red);
					Wipe(device, Color.Green);
					Wipe(device, Color.Blue);
				}
			}
		}

		private static void Wipe(WS281x device, Color color)
		{
            var controller = device.GetController();
			for (int i = 0; i < controller.LEDCount; i++)
			{
				controller.SetLED(i, color);
				device.Render();
				System.Threading.Thread.Sleep(1000 / 15);
			}
		}
	}
}
