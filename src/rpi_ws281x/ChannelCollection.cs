using System.Collections.Generic;
using Native;

namespace rpi_ws281x
{
    public class ChannelCollection
    {
        private Dictionary<int, Controller> controllers;

        public ChannelCollection(Dictionary<int, Controller> controllers)
        {
            this.controllers = controllers;
        }

        public Channel this[int index]
        {
            get
            {
                if (controllers.ContainsKey(index))
                {
                    return (Channel)controllers[index];
                }
                return null;
            }
            set
            {
                controllers[index] = value;
            }
        }

    }
}