// transmission-remote-dotnet
// http://code.google.com/p/transmission-remote-dotnet/
// Copyright (C) 2009 Alan F
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Jayrock.Json;
using Microsoft.Win32;
using TransmissionRemoteDotnet.Forms;
using TransmissionRemoteDotnet.Localization;
using TransmissionRemoteDotnet.Properties;

namespace TransmissionRemoteDotnet
{
    public static class Toolbox
    {

        #region Fields
        // TODO: Move same checks to save settings.
        private const string UnixPathDelimiter = "/";
        private const string WinPathDelimiter = "\\";
        private const int StripeOffset = 15;

        public static readonly NumberFormatInfo NumberFormat;
        #endregion

        public enum MaxSize
        {
            MsByte = 1,
            MsKilo,
            MsMega,
            MsGiga,
            MsTera
        }
        static readonly byte[] TrueBitCount = {
            0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8
            /*
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8,
            4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8, 5, 6, 6, 7, 6, 7, 7, 8, 6, 7, 7, 8, 7, 8, 8, 9*/
        };

        static Toolbox()
        {
            var xml = new XmlSerializer(typeof(NumberFormatInfo));
            var xmldata = new MemoryStream(Resources.EngNumberFormatInfo);
            NumberFormat = (NumberFormatInfo)xml.Deserialize(xmldata);
        }

        public static decimal ToProgress(object o)
        {
            decimal result = Math.Round(ToDecimal(o), 2);
            decimal resultMultiplied = result * 100;
            return resultMultiplied <= 100 ? resultMultiplied : result;
        }

        public static string TrimPath(string s)
        {
            int fwdSlashPos = s.IndexOf('/');
            if (fwdSlashPos > 0)
            {
                return s.Remove(0, fwdSlashPos + 1);
            }
            int bckSlashPos = s.IndexOf('\\');
            if (bckSlashPos > 0)
            {
                return s.Remove(0, bckSlashPos + 1);
            }
            return s;
        }

        public static double ToDouble(object o)
        {
            return ToDouble(o, 0.0);
        }

        private static double ToDouble(object o, double Default)
        {
            switch (o)
            {
                case string s: return double.Parse(s, NumberFormat);
                case JsonNumber number: return number.ToDouble();
                default: return Default;
            }
        }

        public static long ToLong(object o)
        {
            return ToLong(o, 0);
        }

        public static long ToLong(object o, long Default)
        {
            switch (o)
            {
                case long l: return l;
                case bool b: return b ? 1 : 0;
                case JsonNumber number: return number.ToInt64();
                default: return Default;
            }
        }

        private static int ToInt(bool b)
        {
            return b ? 1 : 0;
        }
        public static int ToInt(object o)
        {
            return ToInt(o, 0);
        }
        public static int ToInt(object o, int Default)
        {
            switch (o)
            {
                case int i: return i;
                case bool b: return b ? 1 : 0;
                case JsonNumber number: return number.ToInt32();
                default: return Default;
            }
        }

        public static decimal ToDecimal(object o)
        {
            return ToDecimal(o, 0);
        }
        public static decimal ToDecimal(object o, decimal Default)
        {
            switch (o)
            {
                case string s: return decimal.Parse(s, NumberFormat);
                case JsonNumber number: return number.ToDecimal();
                default: return Default;
            }
        }

        public static JsonArray ListViewSelectionToIdArray(ListView.SelectedListViewItemCollection selections)
        {
            var ids = new JsonArray();
            foreach (Torrent item in selections)
                ids.Put(item.Id);
            return ids;
        }

        public static void CopyListViewToClipboard(ListView listView)
        {
            StringBuilder sb = new StringBuilder();
            if (listView.SelectedItems.Count > 1)
            {
                for (int i = 0; i < listView.Columns.Count; i++)
                {
                    sb.Append(listView.Columns[i].Text);
                    if (i != listView.Columns.Count - 1)
                        sb.Append(',');
                    else
                        sb.Append(Environment.NewLine);
                }
            }
            lock (listView)
            {
                foreach (ListViewItem item in listView.SelectedItems)
                    for (int i = 0; i < item.SubItems.Count; i++)
                    {
                        ListViewItem.ListViewSubItem si = item.SubItems[i];
                        sb.Append(si.Text.Contains(",") ? "\"" + si.Text + "\"" : si.Text);
                        if (i != item.SubItems.Count - 1)
                            sb.Append(',');
                        else
                            sb.Append(Environment.NewLine);
                    }
            }
            Clipboard.SetText(sb.ToString());
        }

        public static void StripeListView(ListView list)
        {
            Color window = SystemColors.Window;
            lock (list)
            {
                list.BeginUpdate();
                foreach (ListViewItem item in list.Items)
                {
                    item.BackColor = item.Index % 2 == 1 ?
                        Color.FromArgb(window.R - StripeOffset,
                            window.G - StripeOffset,
                            window.B - StripeOffset)
                        : window;
                    if (!item.UseItemStyleForSubItems)
                        foreach (ListViewItem.ListViewSubItem lvs in item.SubItems)
                            lvs.BackColor = item.BackColor;
                }
                list.EndUpdate();
            }
        }

        public static void JsonGet(ref bool d, object o)
        {
            if (o != null) d = ToBool(o, d);
        }

        public static void JsonGet(ref int d, object o)
        {
            if (o != null) d = ToInt(o, d);
        }

        public static void JsonGet(ref string d, object o)
        {
            if (o != null) d = o as string;
        }

        public static void JsonPut(JsonObject dest, string key, bool value)
        {
            JsonPut(dest, key, ToInt(value));
        }

        public static void JsonPut(JsonObject dest, string key, object value)
        {
            dest.Put(key, value);
        }

        public static short ToShort(object o)
        {
            return ToShort(o, 0);
        }

        public static short ToShort(object o, short Default)
        {
            return ((JsonNumber?)o)?.ToInt16() ?? Default;
        }

        public static bool ToBool(object o)
        {
            return ToBool(o, true);
        }

        public static bool ToBool(object o, bool Default)
        {
            switch (o)
            {
                case bool b: return b;
                case int i: return i != 0;
                case JsonNumber number: return number.ToBoolean();
                default: return Default;
            }
        }

        public static DateTime DateFromEpoch(double e)
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            return epoch.Add(TimeSpan.FromSeconds(e));
        }

        public static decimal CalcPercentage(long x, long total)
        {
            if (total > 0)
            {
                return Math.Floor((x / (decimal)total) * 100 * 100) / 100;
            }
            return 100;
        }

        public static double CalcRatio(long uploadTotal, long downloadTotal)
        {
            if (downloadTotal <= 0 || uploadTotal < 0)
            {
                return -1;
            }
            return Math.Round((double)uploadTotal / downloadTotal, 3);
        }

        public static string KbpsString(int rate)
        {
            return $"{rate} {OtherStrings.KilobyteShort}/{OtherStrings.Second.ToLower()[0]}";
        }

        public static string FormatTimespanLong(TimeSpan span)
        {
            return
                $"{span.Days}{OtherStrings.Day.ToLower()[0]} {span.Hours}{OtherStrings.Hour.ToLower()[0]} {span.Minutes}{OtherStrings.Minute.ToLower()[0]} {span.Seconds}{OtherStrings.Second.ToLower()[0]}";
        }

        public static string FormatPriority(int n)
        {
            if (n < 0)
                return OtherStrings.Low;
            if (n > 0)
                return OtherStrings.High;
            return OtherStrings.Normal;
        }

        public static string GetSpeed(long bytes)
        {
            return $"{GetFileSize(bytes)}/{OtherStrings.Second.ToLower()[0]}";
        }

        public static string GetSpeed(object o)
        {
            return GetSpeed(ToLong(o));
        }

        public static string GetFileSize(long bytes)
        {
            return GetFileSize(bytes, MaxSize.MsGiga);
        }

        public static string GetFileSize(long bytes, MaxSize maxsize)
        {
            if (bytes >= 1099511627776 && maxsize >= MaxSize.MsTera)
            {
                decimal size = decimal.Divide(bytes, 1099511627776);
                return $"{size:##.##} {OtherStrings.TerabyteShort}";
            }
            if (bytes >= 1073741824 && maxsize >= MaxSize.MsGiga)
            {
                decimal size = decimal.Divide(bytes, 1073741824);
                return $"{size:N} {OtherStrings.GigabyteShort}";
            }
            if (bytes >= 1048576 && maxsize >= MaxSize.MsMega)
            {
                decimal size = decimal.Divide(bytes, 1048576);
                return $"{size:##.##} {OtherStrings.MegabyteShort}";
            }
            if (bytes >= 1024 && maxsize >= MaxSize.MsKilo)
            {
                decimal size = decimal.Divide(bytes, 1024);
                return $"{size:##.##} {OtherStrings.KilobyteShort}";
            }
            if (bytes > 0 && maxsize >= MaxSize.MsByte)
            {
                decimal size = bytes;
                return $"{size:##.##} {OtherStrings.Byte[0]}";
            }
            return "0.00 " + OtherStrings.Byte[0];
        }

        public static string GetExecuteLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string GetExecuteDirectory()
        {
            return Path.GetDirectoryName(GetExecuteLocation());
        }

        public static string GetApplicationData()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AboutDialog.AssemblyTitle);
        }

        public static string LocateFile(string file)
        {
            return LocateFile(file, true);
        }

        public static string LocateFile(string file, bool require)
        {
            return LocateFile(file, require, GetExecuteDirectory());
        }

        public static string LocateFile(string file, params string[] paths)
        {
            return LocateFile(file, true, paths);
        }

        public static string LocateFile(string file, bool require, params string[] paths)
        {
            foreach (string path in paths)
            {
                string fullpath = Path.Combine(path, file);
                if (!require || File.Exists(fullpath))
                    return fullpath;
            }
            throw new FileNotFoundException(file);
        }

        public static string[] Split(string str, int chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentOutOfRangeException("chunkSize");
            int stringLength = str.Length;
            int n = (stringLength + chunkSize - 1) / chunkSize;
            string[] strings = new string[n];
            for (int i = 0, s = 0; i < n; i++, s += chunkSize)
            {
                if (s + chunkSize > stringLength) chunkSize = stringLength - s;
                strings[i] = str.Substring(s, chunkSize);
            }
            return strings;
        }

        public static int BitCount(byte[] bitmap)
        {
            int bits = 0;
            foreach (byte szam in bitmap)
            {
                bits += TrueBitCount[szam];
            }
            return bits;
        }

        public static void SelectAll(ListView lv)
        {
            lock (lv)
            {
                lv.BeginUpdate();
                foreach (ListViewItem item in lv.Items)
                    item.Checked = true;
                lv.EndUpdate();
            }
        }

        public static void SelectNone(ListView lv)
        {
            lock (lv)
            {
                lv.BeginUpdate();
                foreach (ListViewItem item in lv.Items)
                    item.Checked = false;
                lv.EndUpdate();
            }
        }

        public static void SelectInvert(ListView lv)
        {
            lock (lv)
            {
                lv.BeginUpdate();
                foreach (ListViewItem item in lv.Items)
                    item.Checked ^= true;
                lv.EndUpdate();
            }
        }

        public static string GetDomainName(string domain)
        {
            string[] sectons = domain.Split('.');
            if (sectons.Length >= 3)
                domain = string.Join(".", sectons, sectons.Length - 2, 2);
            return domain;
        }

        public static string Md5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(input);
            data = md5.ComputeHash(data);
            StringBuilder s = new StringBuilder();
            foreach (byte b in data)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static Bitmap LoadSkinImage(string fileName, int minHeight, int maxHeight, int imageNumber)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    Bitmap b = new Bitmap(fileName);
                    if (minHeight <= b.Height && b.Height <= maxHeight && b.Width == b.Height * imageNumber)
                    {
                        return b;
                    }
                    b.Dispose();
                }
            }
            catch { }
            return null;
        }

        public static void LoadSkinToImagelist(string fileName, int minHeight, int maxHeight, ImageList imageList, List<Bitmap> Default)
        {
            StringCollection keys = imageList.Images.Keys;
            imageList.Images.Clear();
            Bitmap skin = LoadSkinImage(fileName, minHeight, maxHeight, Default.Count);
            if (skin != null)
            {
                imageList.ImageSize = new Size(skin.Height, skin.Height);
                imageList.Images.AddStrip(skin);
            }
            else
            {
                if (Default.Count > 0)
                    imageList.ImageSize = new Size(Default[0].Height, Default[0].Height);
                imageList.Images.AddRange(Default.ToArray());
            }
            Trace.Assert(imageList.Images.Count <= keys.Count, "Imagelist has more image than image keys!");
            for (int i = 0; i < imageList.Images.Count; i++)
                imageList.Images.SetKeyName(i, keys[i]);
        }

        public static bool ScreenExists(Point p)
        {
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.Bounds.Contains(p))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Renames a subkey of the passed in registry key since 
        /// the Framework totally forgot to include such a handy feature.
        /// </summary>
        /// <param name="parentKey">The RegistryKey that contains the subkey 
        /// you want to rename (must be writeable)</param>
        /// <param name="subKeyName">The name of the subkey that you want to rename
        /// </param>
        /// <param name="newSubKeyName">The new name of the RegistryKey</param>
        /// <returns>True if succeeds</returns>
        public static bool RenameSubKey(RegistryKey parentKey,
        string subKeyName, string newSubKeyName)
        {
            CopyKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
            return true;
        }

        /// <summary>
        /// Copy a registry key. The parentKey must be writeable.
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="keyNameToCopy"></param>
        /// <param name="newKeyName"></param>
        /// <returns></returns>
        public static bool CopyKey(RegistryKey parentKey,
        string keyNameToCopy, string newKeyName)
        {
            //Create new key
            using (RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName))
            {
                //Open the sourceKey we are copying from
                using (RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy))
                {
                    RecurseCopyKey(sourceKey, destinationKey);
                }
            }
            return true;
        }

        private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                using (RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName))
                {
                    using (RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName))
                    {
                        RecurseCopyKey(sourceSubKey, destSubKey);
                    }
                }
            }
        }

        static readonly byte[] Bytes = Encoding.ASCII.GetBytes("TransmissionCool");

        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="originalString">The original string.</param>
        /// <returns>The encrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be 
        /// thrown when the original string is null or empty.</exception>
        public static string Encrypt(string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                       ("The string which needs to be encrypted can not be null.");
            }
            TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(Bytes, Bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// Decrypt a crypted string.
        /// </summary>
        /// <param name="cryptedString">The crypted string.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be thrown 
        /// when the crypted string is null or empty.</exception>
        public static string Decrypt(string cryptedString)
        {
            if (string.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }
            TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream
                    (Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(Bytes, Bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        #region ConvertSambaPaths
        public static string ConvertUnixPathToWinPath(string path)
        {

            var mappings = Program.Settings.Current.SambaShareMappings;

            foreach (var row in mappings)
            {
                string key = row.Key;
                string value = row.Value;

                if (!key.EndsWith(UnixPathDelimiter))
                    key += UnixPathDelimiter;
                if (!value.EndsWith(WinPathDelimiter))
                    value += WinPathDelimiter;

                path = path.Replace(key, value);
            }

            path = path.Replace("/", "\\");

            return path;
        }

        public static string ConvertWinPathToUnixPath(string path)
        {
            var mappings = Program.Settings.Current.SambaShareMappings;

            foreach (var row in mappings)
            {
                string key = row.Key;
                string value = row.Value;

                if (!key.EndsWith(UnixPathDelimiter))
                    key += UnixPathDelimiter;
                if (!value.EndsWith(WinPathDelimiter))
                    value += WinPathDelimiter;

                path = path.Replace(value, key);
            }

            path = path.Replace("\\", "/");

            return path;
        }
        #endregion

    }
}