using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace PS2_Image_Reader
{
    public partial class MainWindow : Window
    {
        private string LogFilePath => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Log.txt");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go_Button_Click(object sender, RoutedEventArgs e)
        {
            Beta();
        }

        private void SourceDirectory_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TargetBadISO_Textbox.Text = SourceDirectory_Textbox.Text + @"\FAILED";
        }

        private void OPLFriendly_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                LimitCharacters_CheckBox.IsEnabled = OPLFriendly_CheckBox.IsChecked.Value;
                RemoveBracketContent_CheckBox.IsEnabled = OPLFriendly_CheckBox.IsChecked.Value;
                ShortenTo32Characters_CheckBox.IsEnabled = OPLFriendly_CheckBox.IsChecked.Value;
            }      
        }

        private void OPLFriendly_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded) 
            { 
                LimitCharacters_CheckBox.IsEnabled = OPLFriendly_CheckBox.IsChecked.Value;
                RemoveBracketContent_CheckBox.IsEnabled = OPLFriendly_CheckBox.IsChecked.Value;
                ShortenTo32Characters_CheckBox.IsEnabled = OPLFriendly_CheckBox.IsChecked.Value;
            }
        }

        private void Beta()
        {
            string source = SourceDirectory_Textbox.Text;
            string targetOK = SourceDirectory_Textbox.Text;
            string targetNOK = TargetBadISO_Textbox.Text;

            bool opl = OPLFriendly_CheckBox.IsChecked.Value;
            bool limit = LimitCharacters_CheckBox.IsChecked.Value;
            bool brackets = RemoveBracketContent_CheckBox.IsChecked.Value;
            bool shorten = ShortenTo32Characters_CheckBox.IsChecked.Value;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                File.Create(LogFilePath);

                var identifier = new PS2_Codex.Identifier();
                identifier.Error += Identifier_Error;
                identifier.Update += Identifier_Update;
                identifier.ActionStart += Identifier_ActionStart;
                identifier.ActionStop += Identifier_ActionStop;
                identifier.FileStart += Identifier_FileStart;
                identifier.FileOK += Identifier_FileOK;
                identifier.FileNOK += Identifier_FileNOK;
                identifier.FileRename += Identifier_FileRename;

                if (opl)
                {
                    identifier.LimitCharacters = limit;
                    identifier.RemoveBracketContent = brackets; 
                    identifier.ShortenTo32Characters = shorten;
                }
                
                identifier.Initialize(source, targetNOK, targetOK, true);
                identifier.Execute();

            }).Start();            
        }

        private void Identifier_FileRename(string oldname, string newname)
        {
            AddOutput(string.Format("File Renamed from '{0}' to '{1}'", oldname, newname));
        }

        private void Identifier_FileStart(string filename)
        {
            AddOutput("File Started: " + filename);
        }

        private void Identifier_FileNOK(string filename)
        {
            AddOutput("File Failed: " + filename);
        }

        private void Identifier_FileOK(string filename)
        {
            AddOutput("File Succeeded: "+ filename);
        }

        private void Identifier_ActionStop(PS2_Codex.Identifier.Actions action)
        {
            AddOutput("Action Completed: " + action.ToString());

            this.Dispatcher.Invoke(() => { 
                Status_TextBlock.Text = "Completed: " + action.ToString();
            });
        }

        private void Identifier_ActionStart(PS2_Codex.Identifier.Actions action, int fileCount)
        {
            AddOutput("Action Started: " + action.ToString());

            this.Dispatcher.Invoke(() => { 
                Status_TextBlock.Text = "Executing: " + action.ToString();

                StatusBar.Value = 0;
                StatusBar.Minimum = 0;            
                StatusBar.Maximum = fileCount;
            });
        }

        private void Identifier_Update(int fileNumber, int fileCount, string fileName)
        {
            this.Dispatcher.Invoke(() => { 
                StatusBar.Value = fileNumber;
            });
        }

        private void Identifier_Error(string exception)
        {
            this.Dispatcher.Invoke(() => {  
                AddOutput("Error: " + exception);
            });
        }

        private void AddOutput(string line)
        {
            string output = DateTime.Now.ToString("HH:mm:ss") + "    " + line;

            File.AppendAllText(LogFilePath, Environment.NewLine + output);

            this.Dispatcher.Invoke(() => { 
                Output_Textbox.Text = output + Environment.NewLine + Output_Textbox.Text; 
            });
        }

    }
}
