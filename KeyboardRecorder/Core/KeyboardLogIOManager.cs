using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADOFAI_Auto.Core.Model;

namespace ADOFAI_Auto.Core
{
    public static class KeyboardLogIOManager
    {
        public const string MAPDATA_FILE_EXTENSION = ".apmd"; /* A dance of fire and ice Play Map Data*/

        public static bool Save(string path, KeyList keyList)
        {
            if (string.IsNullOrWhiteSpace(path) || keyList == null)
                return false;

            if (Path.GetExtension(path).ToLower() != MAPDATA_FILE_EXTENSION)
                path += MAPDATA_FILE_EXTENSION;

            StringBuilder result = new StringBuilder();

            result.AppendLine("# \"A Dance of Fire and Ice\" Map Data");
            result.AppendLine();
            result.AppendLine($"# int Range: Max = {int.MaxValue} ~ Min = {int.MinValue}");
            result.AppendLine();
            result.AppendLine();

            result.AppendLine("# =============== Data ===============");
            result.AppendLine($"FirstOffset={keyList.InputOffset}");
            result.AppendLine($"GlobalOffset={keyList.GlobalOffset}");

            var record = keyList.GetRecord();

            result.AppendLine();
            for (int i = 0; i < record.Count; i++)
                result.AppendLine(record[i].FrameDelay.ToString());
            result.AppendLine("# =============== End ===============");

            try
            {
                File.WriteAllText(path, result.ToString(), Encoding.UTF8);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool Load(string path, KeyList keyList, bool isClear = true)
        {
            if (string.IsNullOrWhiteSpace(path) || keyList == null || !File.Exists(path))
                return false;

            string[] contents;
            try
            {
                contents = File.ReadAllLines(path, Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            if (contents == null || contents.Length == 0)
                return false;

            if (isClear)
                keyList.Clear(false);

            for (int i = 0; i < contents.Length; i++)
                contents[i] = contents[i].Trim();

            for (int i = 0; i < contents.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(contents[i]) || contents[i].StartsWith("#"))
                    continue;

                if (contents[i].Contains("="))
                {
                    var offset = contents[i].Split('=');
                    if (offset.Length == 2 && int.TryParse(offset[1], out int offsetData))
                    {
                        switch (offset[0])
                        {
                            case "FirstOffset":
                                keyList.InputOffset = offsetData;
                                break;
                            case "GlobalOffset":
                                keyList.GlobalOffset = offsetData;
                                break;
                        }
                    }
                    else
                        continue;
                }
                else if (int.TryParse(contents[i], out int delayData))
                    keyList.Add(new KeyboardLog(delayData));
            }
            return true;
        }
    }
}
