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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SpotlightImageSaver
{
    public class JpgInfo
    {
        public int width = 0;
        public int height = 0;
        public bool isPortrait = false;
        public string hash = "";
        public string filepath;
        public string newFileName;
        public bool isJFIF = false;

        public override string ToString()
        {
            return string.Format("File: {0}\nJFIF?:{4}\nDimensions: Width({1}) Height({2})\nHash: {3}\n", filepath, width, height, hash, isJFIF);
        }

        public JpgInfo(string path)
        {
            byte[] finfo = File.ReadAllBytes(path);
            filepath = path;

            ushort sig = BitConverter.ToUInt16(finfo.Take<byte>(2).Reverse().ToArray<byte>(), 0);
            if (sig != 0xFFD8)
            {
                throw new Exception("Not a JPEG File");
            }

            var hasher = new SHA256Managed();
            var chash = hasher.ComputeHash(finfo);
            foreach (byte item in chash)
            {
                hash += item.ToString("x2");
            }

            if (BitConverter.ToUInt16(finfo.Skip<byte>(2).Take<byte>(2).Reverse().ToArray<byte>(), 0) == 0xFFE0)
            {
                isJFIF = true;
            }

            if (!isJFIF)
            {
                width = Convert.ToInt32(BitConverter.ToUInt16(finfo.Skip<byte>(2).Take<byte>(2).Reverse().ToArray<byte>(), 0));
                height = Convert.ToInt32(BitConverter.ToUInt16(finfo.Skip<byte>(4).Take<byte>(2).Reverse().ToArray<byte>(), 0));
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(finfo))
                {
                    if (!getJpgDimensions(ms, out width, out height))
                    {
                        using (Image img = Image.FromStream(ms))
                        {
                            width = img.Size.Width;
                            height = img.Size.Height;
                        }
                    }
                }
            }

            if (height > width) isPortrait = true;

            newFileName = Path.GetFileNameWithoutExtension(filepath) + ".spotlight.jpg";
        }

        private bool getJpgDimensions(Stream buffer, out int width, out int height)
        {
            width = height = 0;
            bool found = false;
            bool eof = false;
            using (BinaryReader reader = new BinaryReader(buffer))
            {
                while (!found || eof)
                {
                    // read 0xFF and the type
                    reader.ReadByte();
                    byte type = reader.ReadByte();

                    // get length
                    int len = 0;
                    switch (type)
                    {
                        // start and end of the image
                        case 0xD8:
                        case 0xD9:
                            len = 0;
                            break;

                        // restart interval
                        case 0xDD:
                            len = 2;
                            break;

                        // the next two bytes is the length
                        default:
                            int lenHi = reader.ReadByte();
                            int lenLo = reader.ReadByte();
                            len = (lenHi << 8 | lenLo) - 2;
                            break;
                    }

                    // EOF?
                    if (type == 0xD9)
                        eof = true;

                    // process the data
                    if (len > 0)
                    {
                        // read the data
                        byte[] data = reader.ReadBytes(len);

                        // this is what we are looking for
                        if (type == 0xC0)
                        {
                            width = data[1] << 8 | data[2];
                            height = data[3] << 8 | data[4];
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
    }
}