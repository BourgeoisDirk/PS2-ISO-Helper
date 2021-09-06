using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PS2_Codex
{
    public class Identifier
    {
        public enum Actions
        {
            None = 0,
            Unzip = 1,
            Convert = 2,
            Identify = 3,
            LoadMapping = 4,
            SaveMapping = 5
        }


        public delegate void ProcessStarted();
        public delegate void ProcessStopped(Dictionary<string, List<string>> gamemapping, TimeSpan duration);

        public delegate void ActionStarted(Actions action, int fileCount);
        public delegate void ActionStopped(Actions action, TimeSpan duration);

        public delegate void FileStarted(string filename, long bytesize);
        public delegate void FileSuccess(string filename, TimeSpan duration);
        public delegate void FileFailed(string filename, TimeSpan duration);
        public delegate void FileRenamed(string oldname, string newname);
        public delegate void FileStopped(string filename, TimeSpan duration);

        public delegate void Progress(Actions action, int fileNumber, int fileCount, string fileName);
        public delegate void ErrorOccurred(string exception);


        public event ProcessStarted ProcessStart;
        public event ProcessStopped ProcessStop;

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

            var processstarted = DateTime.Now;
            ProcessStart?.Invoke();

            ExtractZipFiles();

            ConvertBinFiles();

            if (!IdentifyIsoFiles())
                return false;
             
            ProcessStop?.Invoke(GameMapping, DateTime.Now - processstarted);

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
            var actionstarted = DateTime.Now;
            ActionStart?.Invoke(Actions.Unzip, zipList.Count());            

            foreach (var zipFile in zipList)
            {
                counter++;

                var filestarted = DateTime.Now;
                FileStart?.Invoke(zipFile.Name, zipFile.Length);
                Update?.Invoke(Actions.Unzip, counter, zipList.Count(), zipFile.Name);

                if (ExtractZip(zipFile.FullName))
                {
                    FileOK?.Invoke(zipFile.Name, DateTime.Now - filestarted);
                    File.Delete(zipFile.FullName);
                }
                else
                {
                    FileNOK?.Invoke(zipFile.Name, DateTime.Now - filestarted);
                    File.Move(zipFile.FullName, Path.Combine(TargetFailureDirectory, zipFile.Name));
                }

                FileStop?.Invoke(zipFile.Name, DateTime.Now - filestarted);
            }

            ActionStop?.Invoke(Actions.Unzip, DateTime.Now - actionstarted);
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
            var actionstarted = DateTime.Now;
            ActionStart?.Invoke(Actions.Convert, binList.Count());

            foreach (var binFile in binList)
            {
                counter++;

                var filestarted = DateTime.Now;
                FileStart?.Invoke(binFile.Name, binFile.Length);
                Update?.Invoke(Actions.Convert, counter, binList.Count(), binFile.Name);

                if (ConvertBinToIso(binFile.FullName))
                    FileOK?.Invoke(binFile.Name, DateTime.Now - filestarted);
                else
                {
                    FileNOK?.Invoke(binFile.Name, DateTime.Now - filestarted);
                    File.Move(binFile.FullName, Path.Combine(TargetFailureDirectory, binFile.Name));
                    if (File.Exists(Path.ChangeExtension(binFile.FullName, ".cue")))
                        File.Move(Path.ChangeExtension(binFile.FullName, ".cue"), Path.Combine(TargetFailureDirectory, Path.ChangeExtension(binFile.Name, ".cue")));
                }

                FileStop?.Invoke(binFile.Name, DateTime.Now - filestarted);
            }

            ActionStop?.Invoke(Actions.Convert, DateTime.Now - actionstarted);
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

        private Dictionary<string, List<string>> GameMapping;

        private bool IdentifyIsoFiles()
        {
            if (!Initialized)
            {
                Error?.Invoke("Code not Initialized");
                return false;
            }            

            int counter = 0;
            var isoList = new DirectoryInfo(SourceDirectory).GetFiles("*.iso", SearchOption.TopDirectoryOnly);
            var actionstarted = DateTime.Now;
            ActionStart?.Invoke(Actions.Identify, isoList.Count());

            if (isoList.Count() == 0)
            {
                Error?.Invoke("No ISO files found in the Source Directory");
                return false;
            }

            LoadGameMapping(ref GameMapping);

            foreach (var isoFile in isoList)
            {
                counter++;

                var filestarted = DateTime.Now;
                FileStart?.Invoke(isoFile.Name, isoFile.Length);
                Update?.Invoke(Actions.Identify, counter, isoList.Count(), isoFile.Name);

                string exception = IdentifyIso(isoFile, out string id);
                if (string.IsNullOrEmpty(exception))
                {
                    string newPath = Path.Combine(TargetSuccessDirectory, GetNewFilename(GameMapping, id, isoFile.Name));
                    if (isoFile.FullName == newPath)
                    {
                        FileOK?.Invoke(isoFile.Name, DateTime.Now - filestarted);
                    }
                    else
                    {                        
                        if (File.Exists(newPath))
                        {
                            Error?.Invoke("Duplicate File");
                            FileNOK?.Invoke(isoFile.Name, DateTime.Now - filestarted);
                            Functions.MoveFailedFile(isoFile.FullName, Path.Combine(TargetFailureDirectory, isoFile.Name));
                        }
                        else
                        {
                            FileRename?.Invoke(isoFile.Name, Path.GetFileName(newPath));
                            FileOK?.Invoke(isoFile.Name, DateTime.Now - filestarted);
                            File.Move(isoFile.FullName, newPath);
                        }
                    }
                }
                else
                {
                    Error?.Invoke(exception);
                    FileNOK?.Invoke(isoFile.Name, DateTime.Now - filestarted);
                    Functions.MoveFailedFile(isoFile.FullName, Path.Combine(TargetFailureDirectory, isoFile.Name));
                }

                FileStop?.Invoke(isoFile.Name, DateTime.Now - filestarted);
            }

            SaveGameMapping(GameMapping);

            ActionStop?.Invoke(Actions.Identify, DateTime.Now - actionstarted);

            return true;
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

        private bool LoadGameMapping(ref Dictionary<string, List<string>> mapping)
        {
            var actionstarted = DateTime.Now;
            ActionStart?.Invoke(Actions.LoadMapping, 1);

            bool output = Functions.LoadGameMapping(ref mapping);

            ActionStop?.Invoke(Actions.LoadMapping, DateTime.Now - actionstarted);

            return output;
        }

        public void SaveGameMapping(Dictionary<string, List<string>> mapping)
        {
            var actionstarted = DateTime.Now;
            ActionStart?.Invoke(Actions.SaveMapping, 1);

            Functions.SaveGameMapping(mapping);

            ActionStop?.Invoke(Actions.SaveMapping, DateTime.Now - actionstarted);
        }

        private string GetNewFilename(Dictionary<string, List<string>> mapping, string gameId, string fileName)
        {            
            string name = Path.ChangeExtension(fileName, null);
            string oldName = name;

            if (name.Length >= 11) { 
                string currentId = name.Substring(0, 11);
                if ((currentId.IndexOf('_') == 4) && (currentId.IndexOf('.') == 8) && (currentId.LastIndexOf('.') == 11))
                    name = name.Substring(12);
            }

            if (mapping.ContainsKey(gameId)) { 
                string mapped = mapping[gameId][1];
                if (!string.IsNullOrEmpty(mapped))
                    name = mapped;
            }
            else
            {
                if (LimitCharacters)
                    name = Functions.LimitToAllowedCharacters(name);

                if (RemoveBracketContent)
                    name = Functions.RemoveBracketContentFromName(name);

                if (ShortenTo32Characters)
                    name = Functions.ForceShortenNameTo32Characters(name);

                if (mapping.ContainsKey(gameId))
                    mapping[gameId][1] = name; 
                else
                    mapping.Add(gameId, new List<string>() { oldName, name });
            }

            return string.Format("{0}.{1}{2}", gameId, name, Path.GetExtension(fileName));
        }

        #endregion

    }
}
