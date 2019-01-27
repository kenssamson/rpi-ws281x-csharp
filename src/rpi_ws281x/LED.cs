using System.Drawing;

namespace rpi_ws281x
{
	/// <summary>
	/// Represents a LED which can be controlled by the WS281x controller
	/// </summary>
	public class LED
	{
		/// <summary>
		/// LED which can be controlled by the WS281x controller
		/// </summary>
		internal LED()
		{
			Color = Color.Empty;
		}

		/// <summary>
		/// Gets or sets the color for the LED
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Returns the RGB value of the color
		/// </summary>
		internal int RGBValue => Color.ToArgb();
	}
}
