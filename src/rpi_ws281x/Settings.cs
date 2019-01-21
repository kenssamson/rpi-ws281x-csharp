using Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace rpi_ws281x
{
	/// <summary>
	/// Settings which are required to initialize the WS281x controller
	/// </summary>
	public class Settings
	{
		public static uint DEFAULT_TARGET_FREQ = 800000;
		public static int DEFAULT_DMA_CHANNEL = 10;

		/// <summary>
		/// Gamma Correction Factor 
		/// 1.0 = no correction, higher values result in dimmer midrange colors
		/// </summary>
		public static float DEFAULT_GAMMA_CORRECTION = 2.8f;

		/// <summary>
		/// Number of Colors (0 based) used in code (default is 255 - 8-bit colors)
		/// </summary>
		public static int DEFAULT_COLOR_IN_MAX = 255;

		/// <summary>
		/// Number of Colors (0 based) used by strip. Default (255) is for 8-bit color strips.
		/// Some strips, like LD8806 use 7-bit so 127 should be used instead of default
		/// </summary>
		public static int DEFAULT_COLOR_OUT_MAX = 255;
		
		/// <summary>
		/// Settings to initialize the WS281x controller
		/// </summary>
		/// <param name="frequency">Set frequency in Hz</param>
		/// <param name="dmaChannel">Set DMA channel to use</param>
		public Settings(uint frequency, int dmaChannel)
		{
			Frequency = frequency;
			DMAChannel = dmaChannel;
			Channels = new Dictionary<int, Channel>(PInvoke.RPI_PWM_CHANNELS);
			GammaCorrection = null;				
		}

		/// <summary>
		/// Returns default settings.
		/// Use a frequency of 800000 Hz and DMA channel 10
		/// </summary>
		/// <returns></returns>
		public static Settings CreateDefaultSettings()
		{
			var settings = new Settings(DEFAULT_TARGET_FREQ, DEFAULT_DMA_CHANNEL);
			settings.SetGammaCorrection(DEFAULT_GAMMA_CORRECTION, DEFAULT_COLOR_IN_MAX, DEFAULT_COLOR_OUT_MAX);

			return settings;
		}

				/// <summary>
		/// Create Gamma Correction Map to adjust Colors when using PWM to control LEDs
		/// The <paramref name="gamma"/> is used to set the correction factor with higher values resulting in dimmer midrange colors and lower values being brighter, setting the value
		/// to 1.0 will cause no correction.
		/// </summary>
		/// <param name="gamma">correction factor - default 2.8</param>
		/// <param name="max_out">output color range - default is 255</param>
		/// <param name="max_in">input color range - default is 255</param>
		/// <remarks>
		/// See <a href="https://learn.adafruit.com/led-tricks-gamma-correction/the-issue">Gamma Correction Issue</a>.
		/// </remarks>
		public void SetGammaCorrection(float gamma, int max_in, int max_out)
		{
			GammaCorrection = new List<byte>(max_in);
			for (int i = 0; i < max_in; i++)
			{
				GammaCorrection[i] = (byte)(Math.Pow((float)i / (float)max_in, gamma) * max_out + 0.5);
			}
		}

		/// <summary>
		/// Returns the used frequency in Hz
		/// </summary>
		public uint Frequency { get; private set; }

		/// <summary>
		/// Returns the DMA channel
		/// </summary>
		public int DMAChannel { get; private set; }

		/// <summary>
		/// Returns the channels which holds the LEDs
		/// </summary>
		public Dictionary<int,Channel> Channels { get; private set; }

		/// <summary>
		/// Gamma Corrections Map
		/// </summary>
		public List<Byte> GammaCorrection { get; private set; }
	}
}
