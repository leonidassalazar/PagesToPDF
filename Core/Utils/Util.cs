using System;

namespace Core.Utils
{
    public class Util
    {
        public static string ConcatUrlEndpoint(string urlBase, string endpoint)
        {
            var uriFinal = new Uri(new Uri(urlBase), endpoint);

            return uriFinal.AbsoluteUri;
        }

        public void WriteLineOnTop(string text)
        {
            var currentPosition = new
            {
                left = Console.CursorLeft,
                top = Console.CursorTop
            };
            Console.WriteLine();
            Console.WriteLine($"{text} {new string(' ', 1000)}");
            Console.SetCursorPosition(currentPosition.left, currentPosition.top);
        }
    }
}
