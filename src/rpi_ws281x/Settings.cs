using Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace rpi_ws281x
{
	/// <summary>
	/// Settings which are required to initialize the WS281x controller
	/// </summary>
	public class Settings
	{
		public static readonly uint DEFAULT_TARGET_FREQ = 800000;
		public static readonly int DEFAULT_DMA_CHANNEL = 10;

		/// <summary>
		/// Gamma Correction Factor 
		/// 1.0 = no correction, higher values result in dimmer midrange colors
		/// </summary>
		public static readonly float DEFAULT_GAMMA_CORRECTION = 2.8f;

		/// <summary>
		/// Number of Colors (0 based) used in code (default is 255 - 8-bit colors)
		/// </summary>
		public static readonly int DEFAULT_COLOR_IN_MAX = 255;

		/// <summary>
		/// Number of Colors (0 based) used by strip. Default (255) is for 8-bit color strips.
		/// Some strips, like LD8806 use 7-bit so 127 should be used instead of default
		/// </summary>
		public static readonly int DEFAULT_COLOR_OUT_MAX = 255;
		
		/// <summary>
		/// Settings to initialize the WS281x controller
		/// </summary>
		/// <param name="frequency">Set frequency in Hz</param>
		/// <param name="dmaChannel">Set DMA channel to use</param>
		public Settings(uint frequency, int dmaChannel)
		{
			Frequency = frequency;
			DMAChannel = dmaChannel;
			Controllers = new Dictionary<int, Controller>(PInvoke.RPI_PWM_CHANNELS);

            #pragma warning disable 618
            Channels = new ChannelCollection(Controllers);
            #pragma warning restore 618

            GammaCorrection = null;		
		}

		/// <summary>
		/// Returns default settings.
		/// Use a frequency of 800000 Hz and DMA channel 10
		/// Gamma Correction factor of 2.8 and 256 colors.
		/// </summary>
		public static Settings CreateDefaultSettings()
		{
			var settings = new Settings(DEFAULT_TARGET_FREQ, DEFAULT_DMA_CHANNEL);
			settings.SetGammaCorrection(DEFAULT_GAMMA_CORRECTION, DEFAULT_COLOR_IN_MAX, DEFAULT_COLOR_OUT_MAX);

			return settings;
		}

		/// <summary>
		/// Create Gamma Correction Map to adjust Colors when using PWM to control LEDs. This should only be used before the device has been initialized. 
		/// The <paramref name="gamma"/> is used to set the correction factor with higher values resulting in dimmer midrange colors and lower values being brighter, setting the value
		/// to 1.0 will cause no correction.
		/// </summary>
		/// <param name="gamma">correction factor</param>
		/// <param name="max_out">output color range</param>
		/// <param name="max_in">input color range</param>
		/// <returns>true if set, false otherwise</returns>
		/// <remarks>
		/// See <a href="https://learn.adafruit.com/led-tricks-gamma-correction/the-issue">Gamma Correction Issue</a>.
		/// </remarks>
		public bool SetGammaCorrection(float gamma, int max_in, int max_out)
		{
			if (gamma >= 1.0f)
			{
				GammaCorrection = Enumerable.Range(0, max_in)
					.Select(i => (byte)(Math.Pow((float)i / (float)max_in, gamma) * max_out + 0.5)).ToList();
			}
			else
			{
				GammaCorrection = null;
			}
			return true;
		}

		/// <summary>
		/// Adds/Updates controller using provided settings.
		/// </summary>
		/// <param name="ledCount">number of LEDs</param>
		/// <param name="pin">GPIO pin used to controller strip</param>
		/// <param name="stripType">type of strip</param>
		/// <param name="controllerType">type of controller - should be supported by selected pin</param>
		/// <param name="brightness">maximum brightness for LEDs</param>
		/// <param name="invert">true if signal should be inverted because polarity is reversed</param>
		public Controller AddController(int ledCount, Pin pin, 
			StripType stripType = StripType.Unknown, 
			ControllerType controllerType = ControllerType.PWM0, 
			byte brightness = 255, 
			bool invert = false)
		{
			int channelNumber = (controllerType == ControllerType.PWM1) ? 1 : 0;
			Controllers[channelNumber] = new Controller(ledCount, pin, brightness, invert, stripType, controllerType);
		
			return Controllers[channelNumber];
		}

		/// <summary>
		/// Adds/Updates controller using default GPIO pin - should only be used with 40-PIN GPIO Boards.
		/// </summary>
		/// <param name="controllerType">type of controller - PWM, PCM, SPI</param>
		/// <param name="ledCount">number of LEDs</param>
		/// <param name="stripType">type of strip</param>
		/// <param name="brightness">maximum brightness for LEDs</param>
		/// <param name="invert">true if signal should be inverted because polarity is reversed</param>
		public Controller AddController(ControllerType controllerType, int ledCount, 
			StripType stripType = StripType.Unknown, 
			byte brightness = 255, 
			bool invert = false)
		{
			Controller controller = null;
			switch (controllerType)
			{
				case ControllerType.PWM0:
					controller = AddController(ledCount, Pin.Gpio18, stripType, controllerType, brightness, invert);
					break;

				case ControllerType.PWM1:
					controller = AddController(ledCount, Pin.Gpio13, stripType, controllerType, brightness, invert);
					break;

				case ControllerType.PCM:
					controller = AddController(ledCount, Pin.Gpio21, stripType, controllerType, brightness, invert);
					break;

				case ControllerType.SPI:
					controller = AddController(ledCount, Pin.Gpio10, stripType, controllerType, brightness, invert);
					break;
			}
			return controller;
		}

		/// <summary>
		/// Returns the used frequency in Hz
		/// </summary>
		internal uint Frequency { get; private set; }

		/// <summary>
		/// Returns the DMA channel
		/// </summary>
		internal int DMAChannel { get; private set; }

		/// <summary>
		/// Returns the channels which holds the LEDs
		/// </summary>
		internal Dictionary<int,Controller> Controllers { get; private set; }

		/// <summary>
		/// Returns the Gamma Corrections Map
		/// </summary>
		internal List<Byte> GammaCorrection { get; private set; }

        #region Obsolete

        [Obsolete("Accessing Channels directly is deprecated, please use AddController() instead.")]
        public ChannelCollection Channels { get; private set; }

        #endregion
    }
}
