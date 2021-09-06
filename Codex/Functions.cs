using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS2_Codex
{
    public static class Functions
    {
        public static bool LoadGameMapping(ref Dictionary<string, List<string>> mapping)
        {
            mapping = new Dictionary<string, List<string>>();

            string gamesListFilepath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "GameMapping.csv");
            if (!File.Exists(gamesListFilepath))
                return false;

            using (StreamReader sr = new StreamReader(gamesListFilepath))
            {
                while (!sr.EndOfStream)
                {
                    string splitMe = sr.ReadLine();
                    string[] splits = splitMe.Split(';');
                    mapping.Add(splits[0], new List<string>() { splits[1], splits[2] });
                }
            }

            return true;
        }

        public static void SaveGameMapping(Dictionary<string, List<string>> mapping)
        {
            string gamesListFilepath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "GameMapping.csv");
            if (File.Exists(gamesListFilepath))
            {
                string oldFilePath = Path.Combine(Path.GetDirectoryName(gamesListFilepath), String.Format("Backup {0}-{1}.csv", Path.GetFileNameWithoutExtension(gamesListFilepath), DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")));
                File.Move(gamesListFilepath, oldFilePath);
            }

            using (StreamWriter file = new StreamWriter(gamesListFilepath))
                foreach (var pair in mapping)
                    file.WriteLine(string.Format("{0};{1};{2}", pair.Key, pair.Value[0], pair.Value[1]));
        }

        public static void MoveFailedFile(string source, string target)
        {
            if (File.Exists(target))
            {
                string dir = Path.GetDirectoryName(target);
                string name = Path.GetFileNameWithoutExtension(target);
                string ext = Path.GetExtension(target);

                int counter = 1;
                while (File.Exists(Path.Combine(dir, Path.ChangeExtension(name + counter.ToString(), ext))))
                    counter++;

                File.Move(source, Path.Combine(dir, Path.ChangeExtension(name + counter.ToString(), ext)));
            }
            else
            {
                File.Move(source, target);
            }
        }

        public static string LimitToAllowedCharacters(string name)
        {
            string allowedChars = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_()[]";
            Regex regex = new Regex(@"^[ abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\-_()[\]]+$");
            if (!regex.IsMatch(name))
                name = string.Concat(name.Where(x => allowedChars.Contains(x)));

            return name.Replace("  ", " ");
        }

        public static string RemoveBracketContentFromName(string name)
        {
            for (int i = 1; i < 4; i++)
                if (name.Contains(string.Format("(Disc {0})", i)))
                    name = name.Replace(string.Format("(Disc {0})", i), string.Format("- Disc {0}", i));

            name = Regex.Replace(name, @" \((.*?)\)", "");  // Remove everything between ( and ), including the Brackets and leading Space
            return name.Replace("  ", " ");
        }

        public static string ForceShortenNameTo32Characters(string name)
        {
            int counter = 0;
            while (name.Length > 32)
            {
                counter++;

                int start = 1;
                string[] parts = name.Split('-');
                if ((parts.Length == 1) || (counter > 1))
                    start = 0;

                name = "";
                if (start > 0)
                    name = parts[0];

                for (int i = start; i < parts.Length; i++)
                {
                    parts[i] = string.Concat(parts[i].Where(x => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Contains(x)));

                    if (!string.IsNullOrEmpty(name))
                        name = name + " - ";

                    name = name + parts[i];
                }

                if (counter == 5)
                    break;
            }

            return name.Replace("  ", " ");
        }

        public static string GetReadableByteSize(long bytesize)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (bytesize == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(bytesize);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(bytesize) * num).ToString() + " " + suf[place];
        }
    }
}
