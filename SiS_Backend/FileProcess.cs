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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotlightImageSaver
{
    public class FileProcess
    {
        public static List<JpgInfo> GetJpgInfo(List<string> files)
        {
            var outlist = new List<JpgInfo>();
            Parallel.ForEach(files, (item) =>
            {
                try
                {
                    outlist.Add(new JpgInfo(item));
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Not a JPEG File")
                    {
                        throw;
                    }
                }
            });
            return outlist;
        }

        public static List<JpgInfo> FilterOutInvalid(List<JpgInfo> items, bool portraitOnly = false, bool landscapeOnly = false)
        {
            if (items.Count == 0) return null;
            var outlist = new List<JpgInfo>(items);
            Parallel.ForEach(items, (item) =>
            {
                if (landscapeOnly && (item.isPortrait))
                {
                    outlist.Remove(item);
                }
                if (portraitOnly && (!item.isPortrait))
                {
                    outlist.Remove(item);
                }
                if (item.width < 1080 || item.height < 1080 || item.width > 1920 || item.height > 1920)
                {
                    outlist.Remove(item);
                }
            });
            return outlist;
        }

        public static List<JpgInfo> FilesToCopy(List<JpgInfo> oldf, List<JpgInfo> newf)
        {
            if (newf == null || oldf == null) return null;
            List<JpgInfo> outlist = new List<JpgInfo>();
            Parallel.ForEach(newf, (newfile) =>
             {
                 if (newfile == null) return;
                 var existing = false;
                 foreach (JpgInfo oldfile in oldf)
                 {
                     if (oldfile == null) break;
                     if (newfile.hash.Equals(oldfile.hash))
                     {
                         existing = true;
                         break;
                     }
                 }
                 if (!existing)
                 {
                     outlist.Add(newfile);
                 }
             });
            return outlist;
        }
    }
}