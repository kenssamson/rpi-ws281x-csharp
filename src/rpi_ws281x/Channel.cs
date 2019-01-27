using System;

namespace rpi_ws281x
{
    public class Channel : Controller
    {
        [Obsolete("Channel class is deprecated, please use Controller class instead.")]
        public Channel(int ledCount, int pin, byte brightness, bool invert, StripType stripType)
            : base(ledCount, (Pin)pin, brightness, invert, stripType, ControllerType.Unknown)
        {
        }        
    }
}