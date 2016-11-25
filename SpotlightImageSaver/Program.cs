using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SpotlightImageSaver
{
    internal class Program
    {
        private static List<string> images;
        private static string path;

        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                HelpMessage();
                return 1;
            }

            path = args[0];
            if (!Directory.Exists(path))
            {
                DisplayMessage("Destination directory does not exist!");
                return 2;
            }

            return 0;
        }

        private static void DisplayMessage(string msg)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine("SpotlightImageSaver {0}", fvi.FileVersion);
            Console.WriteLine(":: {0}", msg);
            Console.WriteLine();
        }

        private static void HelpMessage()
        {
            DisplayMessage("Usage: spotimgsave path/to/save/to");
        }
    }
}