using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;

namespace PS2_Image_Reader
{
    public partial class MainWindow : Window
    {
        private string LogFilePath { get; set; }
        private Dictionary<string, List<string>> GameMapping;

        public MainWindow()
        {
            InitializeComponent();
            LogFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), String.Format("Log-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")));
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

                File.Create(LogFilePath).Close();

                var identifier = new PS2_Codex.Identifier();
                identifier.ProcessStart += Identifier_ProcessStart;
                identifier.ProcessStop += Identifier_ProcessStop;
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

        private void Identifier_ProcessStart()
        {
            SetWindowLocked(true);
            AddOutput("Process Started");
        }

        private void Identifier_ProcessStop(Dictionary<string, List<string>> gamemapping, TimeSpan duration)
        {
            this.Dispatcher.Invoke(() => {
                GameMapping = gamemapping;
                SetWindowLocked(false);
                AddOutput(string.Format("Process Finished ({0})", duration.ToString("c")));

                if (RunOplHelper_CheckBox.IsChecked.Value)
                    RunOplHelperWindow();
            });            
        }        

        private void SetWindowLocked(bool locked)
        {
            this.Dispatcher.Invoke(() => {
                SourceDirectory_Textbox.IsEnabled = !locked;
                TargetBadISO_Textbox.IsEnabled = !locked;
                OPLFriendly_CheckBox.IsEnabled = !locked;
                LimitCharacters_CheckBox.IsEnabled = !locked && OPLFriendly_CheckBox.IsChecked.Value;
                RemoveBracketContent_CheckBox.IsEnabled = !locked && OPLFriendly_CheckBox.IsChecked.Value;
                ShortenTo32Characters_CheckBox.IsEnabled = !locked && OPLFriendly_CheckBox.IsChecked.Value;
                RunOplHelper_CheckBox.IsEnabled = !locked;
                Go_Button.IsEnabled = !locked;
                OPL_Button.IsEnabled = !locked;
            });            
        }

        private void Identifier_FileRename(string oldname, string newname)
        {
            AddOutput(string.Format("File Renamed from '{0}' to '{1}'", oldname, newname));
        }

        private void Identifier_FileStart(string filename, long bytesize)
        {
            AddOutput(string.Format("File Started: {0} ({1})", filename, PS2_Codex.Functions.GetReadableByteSize(bytesize)));
        }        

        private void Identifier_FileNOK(string filename, TimeSpan duration)
        {
            AddOutput(string.Format("File Failed: {0} ({1})", filename, duration.ToString("c")));
        }

        private void Identifier_FileOK(string filename, TimeSpan duration)
        {
            AddOutput(string.Format("File Succeeded: {0} ({1})", filename, duration.ToString("c")));
        }        

        private void Identifier_ActionStart(PS2_Codex.Identifier.Actions action, int fileCount)
        {
            AddOutput("Action Started: " + action.ToString() + (fileCount == 0 ? "" : " (" + fileCount + ")"));

            this.Dispatcher.Invoke(() => { 
                Status_TextBlock.Text = "Executing: " + action.ToString();

                StatusBar.Value = 0;
                StatusBar.Minimum = 0;            
                StatusBar.Maximum = fileCount;
            });
        }

        private void Identifier_ActionStop(PS2_Codex.Identifier.Actions action, TimeSpan duration)
        {
            AddOutput(string.Format("Action Completed: {0} ({1})", action.ToString(), duration.ToString("c")));

            this.Dispatcher.Invoke(() => {
                Status_TextBlock.Text = "Completed: " + action.ToString();                
            });
        }

        private void Identifier_Update(PS2_Codex.Identifier.Actions action, int fileNumber, int fileCount, string fileName)
        {
            this.Dispatcher.Invoke(() => {
                Status_TextBlock.Text = "Executing: " + action.ToString() + " " + string.Format("({0}/{1})", fileNumber, fileCount);
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

        private void OPL_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                RunOplHelperWindow();
            });
        }

        private void RunOplHelperWindow()
        {
            var OplHelper = new OPL.Helper(SourceDirectory_Textbox.Text);
            OplHelper.GameMapping = GameMapping;
            OplHelper.ShowDialog();
        }
    }
}
