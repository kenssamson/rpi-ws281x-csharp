namespace rpi_ws281x
{
    public enum Pin : int
    {
        Gpio10 = 10,
        
        Gpio12 = 12,
        
        Gpio13 = 13,
        
        Gpio18 = 18,
        
        Gpio19 = 19,
        
        Gpio21 = 21,
        
        Gpio31 = 31
    }

    public enum PinController {
        Gpio10 = ControllerType.SPI,
        Gpio12 = ControllerType.SPI,
        Gpio13 = ControllerType.PWM1,
        Gpio18 = ControllerType.PWM0,
        Gpio19 = ControllerType.PWM1,
        Gpio21 = ControllerType.PCM,
        Gpio31 = ControllerType.PCM
    }
}