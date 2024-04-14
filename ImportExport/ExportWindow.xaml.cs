using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using CsvHelper;
using Profisys_Zadanie.Class;
using System.Globalization;
using System.IO;
using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using System.Text;


namespace Profisys_Zadanie.ImportExport
{
    public partial class ExportWindow : Window
    {
        private string selectedFolderPath;

        public ExportWindow()
        {
            InitializeComponent();
        }

        private void SelectDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;

                var wasActive = this.IsActive;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    selectedFolderPath = dialog.FileName;
                    MessageBox.Show($"Folder został wybrany: {selectedFolderPath}", "Wybrany Folder", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedFolderLabel.Content = selectedFolderPath;
                }

                if (wasActive)
                {
                    this.Activate();
                }
            }
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var documents = ServiceDocumentDataBase.GetAllInformationFromDocuments();
                var allDocumentItems = new List<DocumentItems>();

                
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    Encoding = Encoding.UTF8, 
                    HasHeaderRecord = true,
                };

                using (var writer = new StreamWriter(Path.Combine(selectedFolderPath, "Documents.csv"), false, new UTF8Encoding(true)))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(documents);
                }

                foreach (var document in documents)
                {
                    var items = await ServiceDocumentDataBase.GetDocumentItemsByDocumentIdAsync(document.Id);
                    allDocumentItems.AddRange(items);
                }

                using (var writer = new StreamWriter(Path.Combine(selectedFolderPath, "DocumentItems.csv"), false, new UTF8Encoding(true)))
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(allDocumentItems);
                }

                MessageBox.Show("Dane zostały pomyślnie zapisane do plików CSV.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas eksportowania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            this.DragMove();
        }
    }
}
