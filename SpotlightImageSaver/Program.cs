/*
 * Copyright 2016 Ebben Feagan.
 *
 * This file is part of SpotlightImageSaver.
 *
 *  SpotlightImageSaver is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  SpotlightImageSaver is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with SpotlightImageSaver.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SpotlightImageSaver
{
    internal class Program
    {
        private static List<JpgInfo> newImages;
        private static List<JpgInfo> existingImages;
        public static bool beVerbose = false;
        private static string path;
        private static bool onlyMobile = false;
        private static bool onlyDesktop = false;
        private static bool dryrun = false;

        private static int Main(string[] args)
        {
            beVerbose = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("VERBOSE"));
            dryrun = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("DRYRUN"));

            if (args.Length < 1)
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
            if (beVerbose)
            {
                Console.WriteLine("Existing files are in: {0}", path);
            }

            string user_profile = System.Environment.GetEnvironmentVariable("userprofile");
            string path_part2 = "AppData\\Local\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets";
            string spotlight_path = Path.Combine(user_profile, path_part2);

            if (args.Length == 2)
            {
                if (args[1].Equals("--only-desktop"))
                {
                    Console.WriteLine("Only processing Desktop images");
                    onlyDesktop = true;
                }
                else
                {
                    if (args[1].Equals("--only-mobile"))
                    {
                        Console.WriteLine("Only processing Mobile images");
                        onlyMobile = true;
                    }
                    else
                    {
                        DisplayMessage(string.Format("Invalid parameter: {0}", args[1]));
                        return 2;
                    }
                }
            }

            Console.WriteLine("Looking for files...");
            var texistingImages = new List<string>(Directory.GetFiles(path));
            var tnewImages = new List<string>(Directory.GetFiles(spotlight_path));

            var tempExisting = new List<string>(texistingImages);
            foreach (string item in tempExisting)
            {
                if (!item.EndsWith(".spotlight.jpg"))
                {
                    texistingImages.Remove(item);
                }
            }

            if (beVerbose)
            {
                Console.WriteLine("Existing Images: {0}", texistingImages.Count);
                Console.WriteLine("To Filter: {0}", tnewImages.Count);
                Console.WriteLine("Assets dir: {0}", spotlight_path);
            }

            existingImages = new List<JpgInfo>();
            Parallel.ForEach(texistingImages, (item) =>
            {
                try
                {
                    existingImages.Add(new JpgInfo(item));
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Not a JPEG File")
                    {
                        throw;
                    }
                }
            });

            newImages = new List<JpgInfo>();
            Parallel.ForEach(tnewImages, (item) =>
            {
                try
                {
                    newImages.Add(new JpgInfo(item));
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Not a JPEG File")
                    {
                        throw;
                    }
                }
            });

            if (beVerbose)
            {
                Console.WriteLine("After initial filtering, {0} images left to process.", newImages.Count);
            }

            var tempNewImages = new List<JpgInfo>(newImages);
            foreach (JpgInfo item in tempNewImages)
            {
                if (onlyDesktop && (item.height > item.width))
                {
                    newImages.Remove(item);
                }
                if (onlyMobile && (item.width > item.height))
                {
                    newImages.Remove(item);
                }
                if (item.width < 1080 || item.height < 1080 || item.width > 1920 || item.height > 1920)
                {
                    newImages.Remove(item);
                }
            }

            if (beVerbose)
            {
                Console.WriteLine("After appropriateness filtering, {0} images remain to process.", newImages.Count);
            }

            List<string> filesToCopy = new List<string>();
            foreach (JpgInfo newf in newImages)
            {
                var existing = false;
                foreach (JpgInfo oldf in existingImages)
                {
                    if (newf.hash.Equals(oldf.hash))
                    {
                        existing = true;
                        break;
                    }
                }
                if (!existing)
                {
                    if (beVerbose)
                    {
                        Console.WriteLine(newf.ToString());
                    }
                    filesToCopy.Add(newf.filepath);
                }
            }

            if (beVerbose)
            {
                Console.WriteLine("Matching Images: {0}", newImages.Count - filesToCopy.Count);
            }

            if (filesToCopy.Count > 0)
            {
                Console.WriteLine("Copying {0} new files...", filesToCopy.Count);
                foreach (string item in filesToCopy)
                {
                    string destFileName = Path.Combine(path, Path.GetFileNameWithoutExtension(item) + ".spotlight.jpg");
                    if (!dryrun)
                    {
                        File.Copy(item, destFileName);
                        File.SetLastWriteTime(destFileName, DateTime.Now);
                    }
                    else
                    {
                        Console.WriteLine("Would've created: {0}", destFileName);
                    }
                }
                Console.WriteLine("Done.");
            }
            else
            {
                Console.WriteLine("No New Files");
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
            DisplayMessage("Usage: spotimgsave path/to/save/to [--only-desktop|--only-mobile]");
        }
    }
}