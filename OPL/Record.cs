using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OPL
{
    public class Record : INotifyPropertyChanged
    {
        private Dictionary<string, List<string>> GameMapping;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Initializing;

        public Record(Dictionary<string, List<string>> gamemapping, string filepath)
        {
            Initializing = true;

            GameMapping = gamemapping;
            Filepath = filepath;

            Initializing = false;
        }

        private string _Filepath;
        public string Filepath {
            get => _Filepath;
            private set
            {
                _Filepath = value;
                Directory = Path.GetDirectoryName(value);
                Filename = Path.GetFileName(value);
                OnPropertyChanged("Filepath");
            }
        }

        private string _Directory;
        public string Directory {
            get => _Directory;
            private set
            {
                _Directory = value;
                OnPropertyChanged("Directory");
            }
        }

        private string _Filename;
        public string Filename {
            get => _Filename;
            private set
            {
                _Filename = value;
                OnPropertyChanged("Filename");

                GameID = value.Substring(0, 11);
                Name = Path.GetFileNameWithoutExtension((String.IsNullOrEmpty(GameID) ? value : value.Substring(12)));
                Extension = Path.GetExtension(value);
            }
        }

        private string _GameID;
        public string GameID {
            get => _GameID;
            private set
            {
                if ((value.IndexOf('_') == 4) && (value.IndexOf('.') == 8)) {
                    if (GameMapping.ContainsKey(value))
                        RegisteredName = GameMapping[value][0];

                    _GameID = value;
                }
                else
                {
                    _GameID = String.Empty;
                    AddError("Missing a valid GameID like \"XXXX_00.00\"");
                }
                OnPropertyChanged("GameID");
            }
        }

        private string _RegisteredName;
        public string RegisteredName
        {
            get => _RegisteredName;
            set
            {
                if (Initializing)
                {
                    _RegisteredName = value;
                    OnPropertyChanged("RegisteredName");
                }
                else if (GameMapping.ContainsKey(GameID)) { 
                    GameMapping[GameID][0] = value;
                    _RegisteredName = value;
                    OnPropertyChanged("RegisteredName");
                }
            }
        }

        private string _Name;
        public string Name { 
            get => _Name;
            private set {
                _Name = value;
                OnPropertyChanged("GameID");

                ReName = value;
            } 
        }

        private string _Extension;
        public string Extension {
            get => _Extension;
            private set
            {
                _Extension = value;
                OnPropertyChanged("Extension");
            }
        }

        private string _ReName;        
        public string ReName
        {
            get => _ReName;
            set
            {
                _ReName = value;
                ReNameLength = value.Length;
                ValidateName(value);
                OnPropertyChanged("ReName");
            }
        }

        private int _ReNameLength;
        public int ReNameLength
        {
            get => _ReNameLength;
            private set
            {
                _ReNameLength = value;
                OnPropertyChanged("ReNameLength");
            }
        }

        private string _ErrorContent;
        public string ErrorContent {
            get => _ErrorContent;
            private set
            {
                _ErrorContent = value;
                OnPropertyChanged("ReName");
                OnPropertyChanged("Invalid");
            }
        }

        public bool Invalid => !String.IsNullOrEmpty(ErrorContent);

        public string GetNewFilepath()
            => Path.Combine(Directory, string.Format("{0}.{1}{2}", GameID, ReName, Extension));

        public string ApplyRename()
        {
            if (!String.IsNullOrEmpty(ErrorContent))
                return "Has Errors";
            if (Name == ReName)
                return "Name remains unchanged";

            if (File.Exists(GetNewFilepath()))
            {
                ErrorContent = "Duplicate Filename detected, cannot Rename this Item";
                return ErrorContent;
            }

            try
            {
                File.Move(Filepath, GetNewFilepath());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private void ValidateName(string name)
        {
            if (!Initializing)
                ErrorContent = String.Empty;

            // Limit Characters
            string allowedChars = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_()[]";
            Regex regex = new Regex(@"^[ abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\-_()[\]]+$");
            if (!regex.IsMatch(name))
                AddError("The Name contains characters that are not Allowed. Allowed Characters are: "+ allowedChars);

            // Min Character Length
            if (name.Length == 0)
                AddError("The New Name must have a Value, it cannot be left Empty.");

            // Max 32 Character Length
            if (name.Length > 32)
                AddError("The Name is longer than 32 Characters, you must Shorten It.");
        }

        private void AddError(string errortext)
        {
            if (!String.IsNullOrEmpty(ErrorContent))
                ErrorContent += Environment.NewLine;

            ErrorContent += errortext;
        }
    }
}
