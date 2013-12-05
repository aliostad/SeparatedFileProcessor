using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SeparatedFileProcessor
{
    class Program
    {
        static void Main(string[] args)
        {

            var foregroundColor = Console.ForegroundColor;
            var options = new CommandLineOptions();
            try
            {
                if (args.Length == 0)
                {
                    ConsoleWriteLine(ConsoleColor.White, options.GetTheHelp());
                    return;
                }
                var successful = Parser.Default.ParseArguments(args, options);
                if (!successful)
                {
                    ConsoleWriteLine(ConsoleColor.Red, "unsuccessful parsing the arguments");
                    return;
                }

                var reader = new StreamReader(options.InputFileName);
                string line = null;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    var strings = line.Split(options.Separator[0]);
                    if (options.Count > 0 && options.Count != strings.Length)
                    {
                        ConsoleWriteLine(ConsoleColor.Red , "Line {0} has {1} fields.", lineNumber, strings.Length);
                        continue;
                    }


                }
            }
            catch (Exception exception)
            {
                ConsoleWriteLine(ConsoleColor.Red, exception.ToString());
                return;
            }

            Console.ForegroundColor = foregroundColor;
            ConsoleWriteLine(ConsoleColor.DarkYellow, "");
            ConsoleWriteLine(ConsoleColor.DarkYellow, "Press <ENTER> to exit ...");
            Console.Read();
        }

        private static void ConsoleWrite(ConsoleColor color, string value, params object[] args)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value, args);
            Console.ForegroundColor = foregroundColor;
        }

        private static void ConsoleWriteLine(ConsoleColor color, string value, params object[] args)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value, args);
            Console.ForegroundColor = foregroundColor;
        }
    }
}
