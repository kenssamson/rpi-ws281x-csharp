using Native;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace rpi_ws281x
{
	/// <summary>
	/// Wrapper class to controll WS281x LEDs
	/// </summary>
	public class WS281x : IDisposable
	{
		private ws2811_t _ws2811;
		private GCHandle _ws2811Handle;
		private bool _isDisposingAllowed;

		/// <summary>
		/// Initialize the wrapper
		/// </summary>
		/// <param name="settings">Settings used for initialization</param>
		public WS281x(Settings settings)
		{
			settings.IsInitialized = true;

			_ws2811 = new ws2811_t
			{
				dmanum = settings.DMAChannel,
				freq = settings.Frequency,
				channel_0 = InitChannel(0, settings),
				channel_1 = InitChannel(1, settings)
			};
			
			//Pin the object in memory. Otherwies GC will probably move the object to another memory location.
			//This would cause errors because the native library has a pointer on the memory location of the object.
			_ws2811Handle = GCHandle.Alloc(_ws2811, GCHandleType.Pinned);

			if (settings.GammaCorrection != null)
			{
				if (settings.Controllers.ContainsKey(0))
                    Marshal.Copy(this.Settings.GammaCorrection.ToArray(), 0, _ws2811.channel_0.gamma, this.Settings.GammaCorrection.Count);
				if (settings.Controllers.ContainsKey(1))
                    Marshal.Copy(this.Settings.GammaCorrection.ToArray(), 0, _ws2811.channel_1.gamma, this.Settings.GammaCorrection.Count);
			}

			this.Settings = settings;

			var initResult = PInvoke.ws2811_init(ref _ws2811);
			if (initResult != ws2811_return_t.WS2811_SUCCESS)
			{
				throw WS281xException.Create(initResult, "initializing");
			}	

			//Disposing is only allowed if the init was successfull.
			//Otherwise the native cleanup function throws an error.
			_isDisposingAllowed = true;
		}

		/// <summary>
		/// Renders the content of the channels
		/// </summary>
		public void Render()
		{
			if (Settings.Controllers.ContainsKey(0))
			{
				var ledColor = Settings.Controllers[0].GetColors();
				Marshal.Copy(ledColor, 0, _ws2811.channel_0.leds, ledColor.Length);
			}
			if (Settings.Controllers.ContainsKey(1))
			{
				var ledColor = Settings.Controllers[1].GetColors();
				Marshal.Copy(ledColor, 0, _ws2811.channel_1.leds, ledColor.Length);
			}
			
			var result = PInvoke.ws2811_render(ref _ws2811);
			if (result != ws2811_return_t.WS2811_SUCCESS)
			{
				throw WS281xException.Create(result, "rendering");
			}
		}

		public void SetAll(Color color)
		{
			foreach (var controller in Settings.Controllers)
			{
				controller.Value.SetAll(color);
			}
			Render();
		}

		/// <summary>
		/// Clear all LEDs
		/// </summary>
		public void Reset()
		{
			foreach (var controller in Settings.Controllers)
			{
				controller.Value.Reset();
			}
			Render();
		}

		public Controller GetController(ControllerType controllerType = ControllerType.PWM0)
		{
			int channelNumber = (controllerType == ControllerType.PWM1) ? 1 : 0;
			if (Settings.Controllers.ContainsKey(channelNumber) && 
				Settings.Controllers[channelNumber].ControllerType == controllerType)
			{
				return Settings.Controllers[channelNumber];
			}
			return null;
		}

		/// <summary>
		/// Returns the settings which are used to initialize the component
		/// </summary>
		public Settings Settings;

		/// <summary>
		/// Initialize the channel propierties
		/// </summary>
		/// <param name="channelIndex">Index of the channel tu initialize</param>
		/// <param name="settings">Controller Settings</param>
		private ws2811_channel_t InitChannel(int channelIndex, Settings settings)
		{
			ws2811_channel_t channel = new ws2811_channel_t();

			if (settings.Controllers.ContainsKey(channelIndex))
			{
				channel.count		= settings.Controllers[channelIndex].LEDCount;
				channel.gpionum		= settings.Controllers[channelIndex].GPIOPin;
				channel.brightness	= settings.Controllers[channelIndex].Brightness;
				channel.invert		= Convert.ToInt32(settings.Controllers[channelIndex].Invert);

				if (settings.Controllers[channelIndex].StripType != StripType.Unknown)
				{
					//Strip type is set by the native assembly if not explicitly set.
					//This type defines the ordering of the colors e. g. RGB or GRB, ...
					channel.strip_type = (int)settings.Controllers[channelIndex].StripType;
				}
			}
			return channel;
		}

		/// <summary>
		/// Returns the error message for the given status code
		/// </summary>
		/// <param name="statusCode">Status code to resolve</param>
		/// <returns></returns>
		private string GetMessageForStatusCode(ws2811_return_t statusCode)
		{
			var strPointer = PInvoke.ws2811_get_return_t_str((int)statusCode);
			return Marshal.PtrToStringAuto(strPointer);
		}

        #region Obsolete

        [Obsolete("SetLEDColor is depreciated, please use GetController(controllerType).SetLED(ledID,color) instead")]
        public void SetLEDColor(int channelIndex, int ledID, Color color)
        {
            if (Settings.Controllers.ContainsKey(channelIndex))
            {
                Settings.Controllers[channelIndex].SetLED(ledID, color);
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				if(_isDisposingAllowed)
				{
					PInvoke.ws2811_fini(ref _ws2811);
					_ws2811Handle.Free();
										
					_isDisposingAllowed = false;
				}
				
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~WS281x()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
	#endregion
	}
}
