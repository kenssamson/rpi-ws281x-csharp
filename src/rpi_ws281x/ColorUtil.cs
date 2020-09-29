using System;
using System.Drawing;

namespace rpi_ws281x {
    public class ColorUtil {
        /// <summary>
        /// Converts a normal color to one that can be used by RGB+W Strips properly
        /// </summary>
        /// <param name="color">The input color to clamp.</param>
        /// <returns>The adjusted rgb+w color value.</returns>
        public static Color ClampAlpha(Color color) {
            // Get the lowest value from our three color values
            var alpha = Math.Min(color.R, color.G);
            alpha = Math.Min(alpha, color.B);
            // If colors are the same, return only the white value
            if (color.R == color.G && color.R == color.B) {
                return Color.FromArgb(alpha, 0, 0, 0);
            }
            // This is probably not necessary
            if (color.R + color.G + color.B > 760) {
                return Color.FromArgb(alpha, 0, 0, 0);
            }
            // Return the alpha-modulated version of the color, giving proper white values
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}