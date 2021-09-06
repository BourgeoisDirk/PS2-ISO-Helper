using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OPL
{
    public partial class Helper : Window
    {
        public Dictionary<string, List<string>> GameMapping;
        public ObservableCollection<Record> RecordSet { get; set; }

        public Helper()
        {
            InitializeComponent();
        }

        public Helper(string sourcedir)
        {
            InitializeComponent();
            SourceDirectory_Textbox.Text = sourcedir;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GameMapping == null)
                PS2_Codex.Functions.LoadGameMapping(ref GameMapping);
        }        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GameMapping == null)
                return;

            RecordSetToMapping();
            PS2_Codex.Functions.SaveGameMapping(GameMapping);
        }

        private void RecordSetToMapping() {
            foreach (Record record in RecordSet.Where(x => !x.Invalid))
            {
                if (GameMapping.ContainsKey(record.GameID))
                    GameMapping[record.GameID][1] = record.ReName;
                else
                    GameMapping.Add(record.GameID, new List<string>() { record.Name, record.ReName });
            }
        }

        private void MappingToRecordSet()
        {
            if (GameMapping == null)
                return;

            foreach (var record in RecordSet.Where(x => !string.IsNullOrEmpty(x.GameID)))
            {
                if (GameMapping.ContainsKey(record.GameID))
                {
                    var value = GameMapping[record.GameID][1];
                    if (!string.IsNullOrEmpty(value))
                        record.ReName = value;
                }
            }
        }

        private void Scan_Button_Click(object sender, RoutedEventArgs e)
        {
            RecordSet = new ObservableCollection<Record>();

            foreach (var filepath in Directory.GetFiles(SourceDirectory_Textbox.Text, "*.iso", SearchOption.TopDirectoryOnly))
                RecordSet.Add(new Record(filepath));

            MappingToRecordSet();

            DataGrid.ItemsSource = RecordSet;
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            Error_TextBox.Text = string.Empty;
            DataGrid.SelectedIndex = -1;

            if (RecordSet.Where(x => x.Invalid).Count() > 0)
                if (MessageBox.Show("There are Unresolved Errors. Would you like to continue and Rename the Valid ones anyway ?", "Unresolved Errors", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            
            foreach (Record item in RecordSet.Where(x => String.IsNullOrEmpty(x.ErrorContent) && (!x.Name.Equals(x.ReName))))
            {
                var errortext = item.ApplyRename();
                if (!string.IsNullOrEmpty(errortext))
                {
                    if (Error_TextBox.Text != string.Empty)
                        Error_TextBox.Text += Environment.NewLine;
                    Error_TextBox.Text += item.Name + ": "+ errortext;
                }
                   
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
            {
                Error_TextBox.Text = "";
                return;
            }

            Record selected = DataGrid.SelectedItem as Record;
            Error_TextBox.Text = selected.ErrorContent;
        }

        private void ResetToOriginalName_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            foreach (Record record in DataGrid.SelectedItems)
                record.ReName = record.Name;
        }

        private void LimitToAllowedChars_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            foreach (Record record in DataGrid.SelectedItems)
                record.ReName = PS2_Codex.Functions.LimitToAllowedCharacters(record.ReName);
        }

        private void RemoveBracketContent_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            foreach (Record record in DataGrid.SelectedItems)
                record.ReName = PS2_Codex.Functions.RemoveBracketContentFromName(record.ReName);
        }

        private void Force32Chars_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            foreach (Record record in DataGrid.SelectedItems)
                record.ReName = PS2_Codex.Functions.ForceShortenNameTo32Characters(record.ReName);
        }
        
    }
}
