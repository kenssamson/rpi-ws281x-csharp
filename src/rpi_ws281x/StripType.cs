using System;
using System.Collections.Generic;
using System.Text;

namespace rpi_ws281x
{
	/// <summary>
	/// The type of the LED strip defines the ordering of the colors (e. g. RGB, GRB, ...).
	/// Maybe the RGBValue property of the LED class needs to be changed if there are other strip types.
	/// </summary>
    public enum StripType
    {
		Unknown = 0,
				
		// 4 color R, G, B and W ordering
		SK6812_STRIP_RGBW = 0x18100800,
		SK6812_STRIP_RBGW = 0x18100008,
		SK6812_STRIP_GRBW = 0x18081000,
		SK6812_STRIP_GBRW = 0x18080010,
 		SK6812_STRIP_BRGW = 0x18001008,
 		SK6812_STRIP_BGRW = 0x18000810,

		// 3 color R, G and B ordering
		WS2811_STRIP_RGB = 0x00100800,
		WS2811_STRIP_RBG = 0x00100008,
		WS2811_STRIP_GRB = 0x00081000,
		WS2811_STRIP_GBR = 0x00080010,
		WS2811_STRIP_BRG = 0x00001008,
		WS2811_STRIP_BGR = 0x00000810,

		WS2812_STRIP = 0x00081000,		// WS2811_STRIP_GRB
		SK6812_STRIP = 0x00081000,		// WS2811_STRIP_GRB
		SK6812W_STRIP = 0x18081000		// SK6812_STRIP_GRBW
	}
}
