#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Native;

#endregion

namespace rpi_ws281x {
	/// <summary>
	///     Wrapper class to control WS281x LEDs
	/// </summary>
	public class WS281x : IDisposable {
		private ws2811_t _ws2811;
		private GCHandle _ws2811Handle;
		private readonly Controller _controller;
		private bool _isDisposingAllowed;

		/// <summary>
		///     Initialize the wrapper
		/// </summary>
		/// <param name="settings">Settings used for initialization</param>
		public WS281x(Settings settings) {
			if (settings.Controller.ControllerType == ControllerType.PWM1) {
				_ws2811 = new ws2811_t {
					dmanum = settings.DMAChannel,
					freq = settings.Frequency,
					channel_1 = InitChannel(settings.Controller)
				};
			} else {
				_ws2811 = new ws2811_t {
					dmanum = settings.DMAChannel,
					freq = settings.Frequency,
					channel_0 = InitChannel(settings.Controller)
				};
			}


			//Pin the object in memory. Otherwies GC will probably move the object to another memory location.
			//This would cause errors because the native library has a pointer on the memory location of the object.
			_ws2811Handle = GCHandle.Alloc(_ws2811, GCHandleType.Pinned);

			var initResult = PInvoke.ws2811_init(ref _ws2811);
			if (initResult != ws2811_return_t.WS2811_SUCCESS) {
				throw WS281xException.Create(initResult, "initializing");
			}

			// save a copy of the controllers - used to update LEDs
			_controller = settings.Controller;

			// if specified, apply gamma correction
			if (settings.GammaCorrection != null) {
				Marshal.Copy(settings.GammaCorrection.ToArray(), 0,
					settings.Controller.ControllerType == ControllerType.PWM1
						? _ws2811.channel_1.gamma
						: _ws2811.channel_0.gamma,
					settings.GammaCorrection.Count);
			}

			//Disposing is only allowed if the init was successful.
			//Otherwise the native cleanup function throws an error.
			_isDisposingAllowed = true;
		}

		/// <summary>
		///     Renders the content of the channels
		/// </summary>
		/// <param name="force">Force LEDs to updated - default only updates if when a change is done</param>
		public void Render(bool force = false) {
			var shouldRender = false;

			if (force || _controller.IsDirty) {
				var ledColor = _controller.GetColors(true);
				Marshal.Copy(ledColor, 0, _controller.ControllerType == ControllerType.PWM1
					? _ws2811.channel_1.leds
					: _ws2811.channel_0.leds, ledColor.Length);
				shouldRender = true;
			}

			if (shouldRender) {
				var result = PInvoke.ws2811_render(ref _ws2811);
				if (result != ws2811_return_t.WS2811_SUCCESS) {
					throw WS281xException.Create(result, "rendering");
				}
			}
		}

		/// <summary>
		/// Returns the controller's current brightness
		/// (0-255)
		/// </summary>
		/// <returns></returns>
		public int GetBrightness() {
			return _controller.Brightness;
		}

		/// <summary>
		/// Update the strip's brightness
		/// </summary>
		/// <param name="brightness">New brightness (0-255)</param>
		public void SetBrightness(int brightness) {
			_controller.Brightness = (byte) brightness;
			if (_controller.ControllerType == ControllerType.PWM1) {
				_ws2811.channel_1.brightness = (byte)brightness;
			} else {
				_ws2811.channel_0.brightness = (byte)brightness;
			}

			_controller.IsDirty = true;
			Render();
		}
		
		/// <summary>
		/// Update the number of LEDs in the strip
		/// </summary>
		/// <param name="ledCount">New number of leds</param>
		public void SetLedCount(int ledCount) {
			_controller.LEDCount = ledCount;
			if (_controller.ControllerType == ControllerType.PWM1) {
				_ws2811.channel_1.count = ledCount;
			} else {
				_ws2811.channel_0.count = ledCount;
			}
			Render();
		}

		/// <summary>
		///     Set all LEDs (on all controllers) to the same color.
		/// </summary>
		/// <param name="color">color to display</param>
		public void SetAll(Color color) {
			// If our strip type has a white component, adjust the color value so it renders correctly
			var cName = _controller.StripType.ToString();
			if (cName.Contains("W") && cName.Contains("SK")) {
				color = ColorClamp.ClampAlpha(color);
			}

			_controller.SetAll(color);
			_controller.IsDirty = false;
			Render(true);
		}

		/// <summary>
		/// Set the color of a particular LED directly.
		/// You will need to call Render to update the colors.
		/// </summary>
		/// <param name="ledId">THe index of the LED to update</param>
		/// <param name="color">The color to update the LED to</param>
		public void SetLed(int ledId, Color color) {
			var cName = _controller.StripType.ToString();
			if (cName.Contains("W") && cName.Contains("SK")) {
				color = ColorClamp.ClampAlpha(color);
			}

			_controller.SetLED(ledId, color);
		}

		/// <summary>
		///     Clear all LEDs
		/// </summary>
		public void Reset() {
			_controller.Reset();
			_controller.IsDirty = false;
			Render(true);
		}

		public Controller GetController() {
			return _controller;
		}

		/// <summary>
		///     Initialize the channel propierties
		/// </summary>
		/// <param name="controller">Controller Settings</param>
		private ws2811_channel_t InitChannel(Controller controller) {
			var channel = new ws2811_channel_t {
				count = controller.LEDCount,
				gpionum = controller.GPIOPin,
				brightness = controller.Brightness,
				invert = Convert.ToInt32(controller.Invert)
			};

			if (controller.StripType != StripType.Unknown) {
				//Strip type is set by the native assembly if not explicitly set.
				//This type defines the ordering of the colors e. g. RGB or GRB, ...
				channel.strip_type = (int) controller.StripType;
			}

			return channel;
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				if (_isDisposingAllowed) {
					PInvoke.ws2811_fini(ref _ws2811);
					_ws2811Handle.Free();

					_isDisposingAllowed = false;
				}

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~WS281x() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion
	}
}