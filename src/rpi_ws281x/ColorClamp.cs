using System;
using System.Drawing;

namespace rpi_ws281x {
    public static class ColorClamp {
        /// <summary>
        /// Converts a normal color to one that can be used by RGB+W Strips properly
        /// </summary>
        /// <param name="color">The input color to clamp.</param>
        /// <returns>The adjusted rgb+w color value.</returns>
        public static Color ClampAlpha(Color tCol) {
            var rI = tCol.R;
            var gI = tCol.G;
            var bI = tCol.B;
            float tM = Math.Max(rI, Math.Max(gI, bI));

            //If the maximum value is 0, immediately return pure black.
            if(tM == 0) { return Color.FromArgb(0, 0, 0,0); }

            //This section serves to figure out what the color with 100% hue is
            var multiplier = 255.0f / tM;
            var hR = rI * multiplier;
            var hG = gI * multiplier;
            var hB = bI * multiplier;  

            //This calculates the Whiteness (not strictly speaking Luminance) of the color
            var maxWhite = Math.Max(hR, Math.Max(hG, hB));
            var minWhite = Math.Min(hR, Math.Min(hG, hB));
            var luminance = ((maxWhite + minWhite) / 2.0f - 127.5f) * (255.0f/127.5f) / multiplier;

            //Calculate the output values
            var wO = Convert.ToInt32(luminance);
            var bO = Convert.ToInt32(bI - luminance);
            var rO = Convert.ToInt32(rI - luminance);
            var gO = Convert.ToInt32(gI - luminance);

            //Trim them so that they are all between 0 and 255
            if (wO < 0) wO = 0;
            if (bO < 0) bO = 0;
            if (rO < 0) rO = 0;
            if (gO < 0) gO = 0;
            if (wO > 255) wO = 255;
            if (bO > 255) bO = 255;
            if (rO > 255) rO = 255;
            if (gO > 255) gO = 255;
            return Color.FromArgb(wO, rO, gO, bO);
        }
    }
}