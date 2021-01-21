using System;
using Native;

namespace rpi_ws281x
{
    public class WS281xException : Exception
    {
        public ws2811_return_t ErrorNumber { get; private set; }

        public string ErrorCode { get; private set; }
        
        internal WS281xException(ws2811_return_t return_code, string message) : base(message)
        {
             ErrorNumber = return_code;
             ErrorCode = Enum.GetName(typeof(ws2811_return_t), return_code);
        }

        internal static WS281xException Create(ws2811_return_t return_code, string status)
        {
            var errorMessage = GetErrorMessage(return_code);
            var message = $"An Error occurred while {status} - {errorMessage} ({(int)return_code})";

            return new WS281xException(return_code, message);
        }

        /// <summary>
        /// Return a user friendly message based on return code
        /// </summary>
        public static string GetErrorMessage(ws2811_return_t returnCode) {
            var result = returnCode switch {
                ws2811_return_t.WS2811_SUCCESS => "Operation Successful",
                ws2811_return_t.WS2811_ERROR_GENERIC => "Generic failure",
                ws2811_return_t.WS2811_ERROR_OUT_OF_MEMORY => "Out of memory",
                ws2811_return_t.WS2811_ERROR_HW_NOT_SUPPORTED => "Hardware revision is not supported",
                ws2811_return_t.WS2811_ERROR_MEM_LOCK => "Memory lock failed",
                ws2811_return_t.WS2811_ERROR_MMAP => "nmap() failed",
                ws2811_return_t.WS2811_ERROR_MAP_REGISTERS => "Unable to map registers to userspace",
                ws2811_return_t.WS2811_ERROR_GPIO_INIT => "Unable to initialize GPIO",
                ws2811_return_t.WS2811_ERROR_PWM_SETUP => "Unable to initialize PWM",
                ws2811_return_t.WS2811_ERROR_MAILBOX_DEVICE => "Failed to create mailbox device",
                ws2811_return_t.WS2811_ERROR_DMA => "DMA Error",
                ws2811_return_t.WS2811_ERROR_ILLEGAL_GPIO => "Selected GPIO not supported",
                ws2811_return_t.WS2811_ERROR_PCM_SETUP => "Unable to initialize PCM",
                ws2811_return_t.WS2811_ERROR_SPI_SETUP => "Unable to initialize SPI",
                ws2811_return_t.WS2811_ERROR_SPI_TRANSFER => "SPI transfer error",
                _ => "Unknown Error Occurred"
            };
            return result;
        }
    }
}