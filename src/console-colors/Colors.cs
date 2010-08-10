// Shows how to manipulate colors on the console if you think
// ConsoleColor is a bit limited.
// This doesn't work on Mac OS X with Mono.
//
// Setting the brightness was taken from this project: http://rsar.codeplex.com/

using System;

namespace Example
{
	internal class Program
	{
		static ConsoleColor BackgroundColor = Console.BackgroundColor;
		static ConsoleColor ForegroundColor = Console.ForegroundColor;

		public static void Main()
		{
			foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
			{
				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = color;
				Console.Write("{0,-11} ", color);

				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = color;
				Console.Write("{0,-11} ", color);

                int temp = (int) color;
                temp ^= 0x08; // this adjusts the brightness

				Console.BackgroundColor = (ConsoleColor) temp;
				Console.ForegroundColor = color;
				Console.Write("{0,-11} ", color);

				Console.BackgroundColor = color;
				Console.ForegroundColor = (ConsoleColor) temp;
				Console.WriteLine("{0,-11} ", color);
			}

			Console.BackgroundColor = BackgroundColor;
			Console.ForegroundColor = ForegroundColor;
		}
	}
}