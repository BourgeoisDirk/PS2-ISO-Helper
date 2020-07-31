using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS2_Codex
{
    public class Identifier
    {
        public enum Actions
        {
            None = 0,
            Unzip = 1,
            Convert = 2,
            Identify = 3
        }

        public delegate void ActionStarted(Actions action, int fileCount);
        public delegate void ActionStopped(Actions action);

        public delegate void FileStarted(string filename);
        public delegate void FileSuccess(string filename);
        public delegate void FileRenamed(string oldname, string newname);
        public delegate void FileFailed(string filename);
        public delegate void FileStopped(string filename);

        public delegate void Progress(int fileNumber, int fileCount, string fileName);
        public delegate void ErrorOccurred(string exception);


        public event ActionStarted ActionStart;
        public event ActionStopped ActionStop;

        public event FileStarted FileStart;        
        public event FileSuccess FileOK;
        public event FileRenamed FileRename;
        public event FileFailed FileNOK;
        public event FileStopped FileStop;

        public event Progress Update;
        public event ErrorOccurred Error;

        public Identifier()
        {
            LimitCharacters = true;
            RemoveBracketContent = true;
            ShortenTo32Characters = true;
        }

        public bool Initialized { get; private set; }
        public string SourceDirectory { get; private set; }
        public string TargetFailureDirectory { get; private set; }
        public string TargetSuccessDirectory { get; private set; }

        public bool LimitCharacters { get; set; }
        public bool RemoveBracketContent { get; set; }
        public bool ShortenTo32Characters { get; set; }


        public bool Initialize(string sourceDirectory, string targetFailureDirectory, string targetSuccessDirectory, bool autoCreateMissingDirectories = true)
        {
            SourceDirectory = sourceDirectory;
            TargetFailureDirectory = targetFailureDirectory;
            TargetSuccessDirectory = targetSuccessDirectory;

            if (!HandleDirectories(sourceDirectory, targetFailureDirectory, targetSuccessDirectory, autoCreateMissingDirectories))
                return false;

            Initialized = true;
            return Initialized;
        }

        private bool HandleDirectories(string sourceIsoDirectory, string targetBadIsoDirectory, string targetGoodisoDirectory, bool autoCreateMissingDir = true)
        {
            if (string.IsNullOrEmpty(sourceIsoDirectory))
            {
                Error?.Invoke("Source Directory is undefined");
                return false;
            }

            if (string.IsNullOrEmpty(targetBadIsoDirectory))
            {
                Error?.Invoke("Target Failure Directory is undefined");
                return false;
            }

            if (string.IsNullOrEmpty(targetGoodisoDirectory))
            {
                Error?.Invoke("Target Success ISO Directory is undefined");
                return false;
            }

            if (autoCreateMissingDir)
            {
                if (!Directory.Exists(targetBadIsoDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(targetBadIsoDirectory);
                    }
                    catch (Exception ex)
                    {
                        Error?.Invoke("Target Failure Directory could not be created: " + ex.Message);
                        return false;
                    }
                }

                if (!Directory.Exists(targetGoodisoDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(targetGoodisoDirectory);
                    }
                    catch (Exception ex)
                    {
                        Error?.Invoke("Target Success Directory could not be created: " + ex.Message);
                        return false;
                    }
                }
            }
            else
            {
                if (!Directory.Exists(targetBadIsoDirectory))
                {
                    Error?.Invoke("Target Failure Directory does not exist");
                    return false;
                }

                if (!Directory.Exists(targetGoodisoDirectory))
                {
                    Error?.Invoke("Target Success Directory does not exist");
                    return false;
                }
            }

            return true;
        }


        public bool Execute()
        {
            if (!Initialized)
            {
                Error?.Invoke("Code not Initialized");
                return false;
            }

            ExtractZipFiles();

            ConvertBinFiles();

            if (!IdentifyIsoFiles())
                return false;

            return true;
        }


        #region "Zip Extraction"

        public void ExtractZipFiles()
        {
            if (!Initialized)
            {
                Error?.Invoke("Code not Initialized");
                return;
            }            

            int counter = 0;
            var zipList = new DirectoryInfo(SourceDirectory).GetFiles("*.*", SearchOption.TopDirectoryOnly).Where(x => "*.7z,*.zip,*.rar".Contains(x.Extension.ToLower()));
            ActionStart?.Invoke(Actions.Unzip, zipList.Count());

            foreach (var zipFile in zipList)
            {
                counter++;                

                FileStart?.Invoke(zipFile.Name);
                Update?.Invoke(counter, zipList.Count(), zipFile.Name);

                if (ExtractZip(zipFile.FullName))
                {
                    FileOK?.Invoke(zipFile.Name);
                    File.Delete(zipFile.FullName);
                }
                else
                {
                    FileNOK?.Invoke(zipFile.Name);
                    File.Move(zipFile.FullName, Path.Combine(TargetFailureDirectory, zipFile.Name));
                }

                FileStop?.Invoke(zipFile.Name);
            }

            ActionStop?.Invoke(Actions.Unzip);
        }

        private bool ExtractZip(string path)
        {
            try
            {
                switch (Path.GetExtension(path).ToLower())
                {
                    case ".7z":
                        using (var archive = SevenZipArchive.Open(path))
                        {
                            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                            {
                                entry.WriteToDirectory(Path.GetDirectoryName(path), new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                        break;
                    case ".zip":
                        using (var archive = ZipArchive.Open(path))
                        {
                            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                            {
                                entry.WriteToDirectory(Path.GetDirectoryName(path), new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                        break;
                    case ".rar":
                        using (var archive = RarArchive.Open(path))
                        {
                            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                            {
                                entry.WriteToDirectory(Path.GetDirectoryName(path), new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke("Zip extraction failed: " + ex.Message);
                return false;
            }

            return true;
        }

        #endregion


        #region "Convert Bin to Iso"

        public void ConvertBinFiles()
        {
            if (!Initialized)
            {
                Error?.Invoke("Code not Initialized");
                return;
            }            

            int counter = 0;
            var binList = new DirectoryInfo(SourceDirectory).GetFiles("*.bin", SearchOption.TopDirectoryOnly);
            ActionStart?.Invoke(Actions.Convert, binList.Count());

            foreach (var binFile in binList)
            {
                counter++;

                FileStart?.Invoke(binFile.Name);
                Update?.Invoke(counter, binList.Count(), binFile.Name);

                if (ConvertBinToIso(binFile.FullName))
                    FileOK?.Invoke(binFile.Name);
                else
                {
                    FileNOK?.Invoke(binFile.Name);
                    File.Move(binFile.FullName, Path.Combine(TargetFailureDirectory, binFile.Name));
                    if (File.Exists(Path.ChangeExtension(binFile.FullName, ".cue")))
                        File.Move(Path.ChangeExtension(binFile.FullName, ".cue"), Path.Combine(TargetFailureDirectory, Path.ChangeExtension(binFile.Name, ".cue")));
                }

                FileStop?.Invoke(binFile.Name);
            }

            ActionStop?.Invoke(Actions.Convert);
        }

        private bool ConvertBinToIso(string path)
        {
            try
            {
                string pathCap = "\"{0}\"";
                string[] arguments = { string.Format(pathCap, path),
                                       string.Format(pathCap, Path.ChangeExtension(path, ".cue")),
                                       string.Format(pathCap, Path.ChangeExtension(path, null)) };

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "bchunk.exe"),
                        Arguments = String.Join(" ", arguments),
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();

                File.Move(Path.ChangeExtension(path, null) + "01.iso", Path.ChangeExtension(path, ".iso"));
                File.Delete(path);
                File.Delete(Path.ChangeExtension(path, ".cue"));
            }
            catch (Exception ex)
            {
                Error?.Invoke("Bin to Iso Conversion failed: "+ ex.Message);
                return false;
            }

            return true;
        }

        #endregion


        #region "Identification"

        private bool IdentifyIsoFiles()
        {
            if (!Initialized)
            {
                Error?.Invoke("Code not Initialized");
                return false;
            }            

            int counter = 0;
            var isoList = new DirectoryInfo(SourceDirectory).GetFiles("*.iso", SearchOption.TopDirectoryOnly);
            ActionStart?.Invoke(Actions.Identify, isoList.Count());

            if (isoList.Count() == 0)
            {
                Error?.Invoke("No ISO files found in the Source Directory");
                return false;
            }

            LoadGameMapping(out Dictionary<string, string> mapping);

            foreach (var isoFile in isoList)
            {
                counter++;

                FileStart?.Invoke(isoFile.Name);
                Update?.Invoke(counter, isoList.Count(), isoFile.Name);

                string exception = IdentifyIso(isoFile, out string id);
                if (string.IsNullOrEmpty(exception))
                {
                    string newPath = Path.Combine(TargetSuccessDirectory, GetNewFilename(mapping, id, isoFile.Name));
                    if (isoFile.FullName == newPath)
                    {
                        FileOK?.Invoke(isoFile.Name);
                    }
                    else
                    {                        
                        if (File.Exists(newPath))
                        {
                            Error?.Invoke("Duplicate File");
                            FileNOK?.Invoke(isoFile.Name);
                            MoveFailedFile(isoFile.FullName, Path.Combine(TargetFailureDirectory, isoFile.Name));
                        }
                        else
                        {
                            FileRename?.Invoke(isoFile.Name, Path.GetFileName(newPath));
                            FileOK?.Invoke(isoFile.Name);
                            File.Move(isoFile.FullName, newPath);
                        }
                    }
                }
                else
                {
                    Error?.Invoke(exception);
                    FileNOK?.Invoke(isoFile.Name);
                    MoveFailedFile(isoFile.FullName, Path.Combine(TargetFailureDirectory, isoFile.Name));
                }

                FileStop?.Invoke(isoFile.Name);
            }

            SaveGameMapping(mapping);

            ActionStop?.Invoke(Actions.Identify);

            return true;
        }

        private void MoveFailedFile(string source, string target)
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

        private string IdentifyIso(FileInfo isoFile, out string id)
        {
            id = null;
            var identified = false;

            try
            {
                using (FileStream isoStream = File.OpenRead(isoFile.FullName))
                {
                    var cd = new DiscUtils.Iso9660.CDReader(isoStream, true);
                    var fileList = cd.Root.GetFiles();
                    foreach (var file in fileList)
                    {
                        var fileName = file.Name.Substring(0, file.Name.IndexOf(';'));
                        if ((fileName.Length == 11) && (fileName.IndexOf('_') == 4) && (fileName.IndexOf('.') == 8))
                        {
                            id = fileName;
                            identified = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (!identified)
            {
                return "No Game Identifier found within the ISO File";
            }

            return null;
        }

        private bool LoadGameMapping(out Dictionary<string, string> mapping)
        {
            mapping = new Dictionary<string, string>();

            string gamesListFilepath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "GameList.txt");
            if (!File.Exists(gamesListFilepath))
                return false;

            using (StreamReader sr = new StreamReader(gamesListFilepath))
            {
                while (!sr.EndOfStream)
                {
                    string splitMe = sr.ReadLine();
                    string[] splits = splitMe.Split(';');
                    mapping.Add(splits[0], splits[1]);
                }
            }

            return true;
        }

        private void SaveGameMapping(Dictionary<string, string> mapping)
        {
            string gamesListFilepath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "GameList.txt");
            if (File.Exists(gamesListFilepath))
            {
                string oldFilePath = Path.Combine(Path.GetDirectoryName(gamesListFilepath), "Old" + Path.GetFileName(gamesListFilepath));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);

                File.Move(gamesListFilepath, oldFilePath);
            }

            using (StreamWriter file = new StreamWriter(gamesListFilepath))
                foreach (var pair in mapping)
                    file.WriteLine(string.Format("{0};{1}", pair.Key, pair.Value));
        }

        private string GetNewFilename(Dictionary<string, string> mapping, string gameId, string fileName)
        {            
            string name = Path.ChangeExtension(fileName, null);
            string oldName = name;

            if (name.Length >= 11) { 
                string currentId = name.Substring(0, 11);
                if ((currentId.IndexOf('_') == 4) || (currentId.IndexOf('.') == 8) || (currentId.LastIndexOf('.') == 11))
                    name = name.Substring(12);
            }

            if (mapping.ContainsKey(gameId))
                name = mapping[gameId].Split(';')[0];
            else
            {
                if (name.Contains(", The"))
                    name = "The " + name.Replace(", The", "");

                if (LimitCharacters) { 
                    string allowedChars = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_()[]";
                    Regex regex = new Regex(@"^[ abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\-_()[\]]+$");
                    if (!regex.IsMatch(name))
                        name = string.Concat(name.Where(x => allowedChars.Contains(x)));
                }

                if (RemoveBracketContent)
                {
                    for (int i = 1; i < 4; i++)
                        if (name.Contains(string.Format("(Disc {0})", i)))
                            name = name.Replace(string.Format("(Disc {0})", i), string.Format("- Disc {0}", i));
                    
                    name = Regex.Replace(name, @" \((.*?)\)", "");  // Remove everything between ( and ), including the Brackets and leading Space
                }

                if (ShortenTo32Characters) { 
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

                        for (int i = start; i < parts.Length; i++) { 
                            parts[i] = string.Concat(parts[i].Where(x => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Contains(x)));

                            if (!string.IsNullOrEmpty(name))
                                name = name + " - ";

                            name = name + parts[i];
                        }

                        if (counter == 5)
                            break;
                    }
                }

                name = name.Replace("  ", " ");
                mapping.Add(gameId, string.Format("{0};{1}", name, oldName));
            }

            return string.Format("{0}.{1}", gameId, Path.ChangeExtension(name, Path.GetExtension(fileName)));
        }        

        #endregion

    }
}
