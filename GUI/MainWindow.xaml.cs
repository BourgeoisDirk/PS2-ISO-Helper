using System;
using System.Threading;
using System.Windows;

namespace PS2_Image_Reader
{
    public partial class MainWindow : Window
    {
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

        private void Beta()
        {
            bool opl = OPLFriendly_CheckBox.IsChecked.Value;
            string source = SourceDirectory_Textbox.Text;
            string targetOK = SourceDirectory_Textbox.Text;
            string targetNOK = TargetBadISO_Textbox.Text;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;


                var identifier = new PS2_Codex.Identifier();
                identifier.Error += Identifier_Error;
                identifier.Update += Identifier_Update;
                identifier.ActionStart += Identifier_ActionStart;
                identifier.ActionStop += Identifier_ActionStop;
                identifier.FileOK += Identifier_FileOK;
                identifier.FileNOK += Identifier_FileNOK;

                if (opl)
                {
                    identifier.LimitCharacters = true;
                    identifier.RemoveBracketContent = true;
                    identifier.ShortenTo32Characters = true;
                }
                
                identifier.Initialize(source, targetNOK, targetOK, true);
                identifier.Execute();

            }).Start();
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
            this.Dispatcher.Invoke(() => { 
                Output_Textbox.Text = DateTime.Now.ToString("HH:mm:ss") + "    " + line + Environment.NewLine + Output_Textbox.Text; 
            });
        }
    }
}
