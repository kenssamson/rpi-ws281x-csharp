using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace rpi_ws281x
{
    /// <summary>
    /// Represents the controller which drives the LEDs
    /// </summary>
    public class Controller
	{
		internal Controller(int ledCount, Pin pin, byte  brightness, bool invert, StripType stripType, ControllerType controllerType)
		{
			Pin = pin;
			GPIOPin = (int)pin;
			Invert = invert;
			Brightness = brightness;
			StripType = stripType;
			ControllerType = controllerType;

			LEDColors = Enumerable.Repeat(new LED(), ledCount).ToList();
		}

		public void SetLED(int ledID, Color color)
		{
			LEDColors[ledID].Color = color;
		}

		public void SetAll(Color color)
		{
			LEDColors.ForEach(led => led.Color = color);
		}

		public void Reset()
		{
			LEDColors.ForEach(led => led.Color = Color.Empty);
		}

		internal int[] GetColors()
		{
			return LEDColors.Select(x => x.RGBValue).ToArray();
		}
		
		internal List<LED> LEDColors { get; private set; }

		/// <summary>
		/// Returns the GPIO pin which is connected to the LED strip
		/// </summary>
		internal int GPIOPin { get; private set; }

		/// <summary>
		/// Returns the Pin used to connect to the LED strip
		/// </summary>
		public Pin Pin { get; private set; }

		/// <summary>
		/// Returns a value which indicates if the signal needs to be inverted.
		/// Set to true to invert the signal (when using NPN transistor level shift).
		/// </summary>
		public bool Invert { get; private set; }

		/// <summary>
		/// Gets or sets the brightness of the LEDs
		/// 0 = darkes, 255 = brightest
		/// </summary>
		public byte Brightness { get; set; }

		/// <summary>
		/// Returns the type of the channel.
		/// The type defines the ordering of the colors.
		/// </summary>
		public StripType StripType { get; private set; }

        /// <summary>
        /// Returns all LEDs on this channel
        /// </summary>
        public IReadOnlyCollection<LED> LEDs => LEDColors.AsReadOnly();

        public int LEDCount => LEDColors.Count;
		
		public ControllerType ControllerType { get; private set; }
	}
}
