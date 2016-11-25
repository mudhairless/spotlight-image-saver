/*
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * */

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