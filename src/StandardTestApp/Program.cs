using System;
using System.Collections.Generic;

namespace CoreTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var abort = new AbortRequest();
            var animations = GetAnimations();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                abort.IsAbortRequested = true;
            };

            var input = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("What do you want to test:" + Environment.NewLine);
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Color wipe animation");
                Console.WriteLine("2 - Rainbow color animation" + Environment.NewLine);
                Console.WriteLine("Press CTRL+C to abort current test." + Environment.NewLine);
                Console.Write("What is your choice: ");
                input = Int32.Parse(Console.ReadLine());

                if (animations.ContainsKey(input))
                {
                    abort.IsAbortRequested = false;
                    animations[input].Execute(abort);
                }
                
            } while (input != 0);
        }

        private static Dictionary<int, IAnimation> GetAnimations()
        {
            var result = new Dictionary<int, IAnimation>();

            result[1] = new ColorWipe();
            result[2] = new RainbowColorAnimation();

            return result;
        }
    }
}
