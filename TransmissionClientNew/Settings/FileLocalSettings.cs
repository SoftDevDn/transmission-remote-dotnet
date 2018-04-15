using System;
using System.Text;
using System.IO;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet.Settings
{
    public class FileLocalSettingsStore : ILocalSettingsStore
    {
        private const string ConfFile = @"settings.json";
        public override JsonObject Load()
        {
            return Load(Toolbox.LocateFile(ConfFile));
        }

        public JsonObject Load(string filename)
        {
            JsonObject jo;
            using (var inFile = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] binaryData = new byte[inFile.Length];
                if (inFile.Read(binaryData, 0, (int)inFile.Length) < 1)
                    throw new Exception(OtherStrings.EmptyFile);
                jo = (JsonObject)JsonConvert.Import(Encoding.UTF8.GetString(binaryData));
            }
            return jo;
        }

        public override bool Save(JsonObject s)
        {
            return Save(Toolbox.LocateFile(ConfFile, false), s);
        }

        public bool Save(string filename, JsonObject s)
        {
            try
            {
                using (var outFile = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(outFile))
                    {
                        writer.Write(s.ToString());
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
