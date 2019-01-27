using System;
using Native;

namespace rpi_ws281x
{
    public class WS281xException : Exception
    {
        public int ErrorNumber { get; private set; }

        public string ErrorCode { get; private set; }
        
        public string ErrorMessage { get; private set; }

        internal WS281xException(ws2811_return_t return_code, string return_error, string message) : base(message)
        {
             ErrorNumber = (int)return_code;
             ErrorCode = Enum.GetName(typeof(ws2811_return_t), ErrorNumber);
             ErrorMessage = return_error;
        }

        internal static WS281xException Create(ws2811_return_t return_code, string status)
        {
            var errorMessage = GetErrorMessage(return_code);
            var message = $"An Error occurred while {status} - {errorMessage} ({(int)return_code})";

            return new WS281xException(return_code, errorMessage, message);
        }

        private static string GetErrorMessage(ws2811_return_t return_code)
        {
            var result = string.Empty;

            switch (return_code)
            {
                case ws2811_return_t.WS2811_ERROR_GENERIC:
                    result = "Generic failure";
                    break;

                case ws2811_return_t.WS2811_ERROR_OUT_OF_MEMORY:
                    result = "Out of memory";
                    break;

                case ws2811_return_t.WS2811_ERROR_HW_NOT_SUPPORTED:
                    result = "Hardware revision is not supported";
                    break;

                case ws2811_return_t.WS2811_ERROR_MEM_LOCK:
                    result = "Memory lock failed";
                    break;

                case ws2811_return_t.WS2811_ERROR_MMAP:
                    result = "nmap() failed";
                    break;

                case ws2811_return_t.WS2811_ERROR_MAP_REGISTERS:
                    result = "Unable to map registers to userspace";
                    break;

                case ws2811_return_t.WS2811_ERROR_GPIO_INIT:
                    result = "Unable to initialize GPIO";
                    break;

                case ws2811_return_t.WS2811_ERROR_PWM_SETUP:
                    result = "Unable to initialize PWM";
                    break;

                case ws2811_return_t.WS2811_ERROR_MAILBOX_DEVICE:
                    result = "Failed to create mailbox device";
                    break;

                case ws2811_return_t.WS2811_ERROR_DMA:
                    result = "DMA Error";
                    break;

                case ws2811_return_t.WS2811_ERROR_ILLEGAL_GPIO:
                    result = "Selected GPIO not supported";
                    break;

                case ws2811_return_t.WS2811_ERROR_PCM_SETUP:
                    result = "Unable to initialize PCM";
                    break;

                case ws2811_return_t.WS2811_ERROR_SPI_SETUP:
                    result = "Unable to initialize SPI";
                    break;

                case ws2811_return_t.WS2811_ERROR_SPI_TRANSFER:
                    result = "SPI transfer error";
                    break;
            }
            return result;
        }
    }
}