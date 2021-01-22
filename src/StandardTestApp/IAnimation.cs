namespace CoreTestApp
{
    public interface IAnimation
    {
         void Execute(AbortRequest request, int gpioPin);
    }
}