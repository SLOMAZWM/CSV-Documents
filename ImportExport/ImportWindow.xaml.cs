using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Profisys_Zadanie.Class;
using Microsoft.Win32;

namespace Profisys_Zadanie.ImportExport
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        public List<Document> Documents = new List<Document>();
        private string selectedDocumentCsvPath;
        private string selectedPositionsCsvPath;

        public ImportWindow()
        {
            InitializeComponent();
        }

        private void SelectDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedDocumentCsvPath = openFileDialog.FileName;
                // Aktualizuj Label z wybraną ścieżką
                
                if (ValidateCsvFile(selectedDocumentCsvPath, isDocument: true))
                {
                    MessageBox.Show("Plik został poprawnie załadowany!", "Poprawny format", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Niepoprawnie wczytany plik!", "Błąd formatu", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
        }

        private void SelectPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedPositionsCsvPath = openFileDialog.FileName;
                // Aktualizuj Label z wybraną ścieżką
                // Walidacja struktury pliku
                if (ValidateCsvFile(selectedPositionsCsvPath, isDocument: false))
                {
                    MessageBox.Show("Plik został poprawnie załadowany!", "Poprawny format", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Niepoprawnie wczytany plik!", "Błąd formatu", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
        }

        public bool ValidateCsvFile(string filePath, bool isDocument)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();
                string[] headerRow = csv.HeaderRecord;

                bool isValid;
                if (isDocument)
                {
                    isValid = IsValidHeaderForDocument(headerRow);
                }
                else
                {
                    isValid = IsValidHeaderForDocumentItems(headerRow);
                }

                if (!isValid)
                {
                    var missingHeaders = isDocument ?
                        GetMissingHeaders(headerRow, new[] { "Id", "Type", "Date", "FirstName", "LastName", "City" }) :
                        GetMissingHeaders(headerRow, new[] { "DocumentID", "Ordinal", "Name", "Quantity", "Price", "TaxRate" });

                    MessageBox.Show("Nagłówki nieprawidłowe. Brakujące nagłówki: " + string.Join(", ", missingHeaders), "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return isValid;
            }
        }

        private IEnumerable<string> GetMissingHeaders(string[] headers, string[] requiredHeaders)
        {
            return requiredHeaders.Where(requiredHeader => !headers.Contains(requiredHeader, StringComparer.OrdinalIgnoreCase));
        }


        private bool IsValidHeaderForDocument(string[] headers)
        {
            var requiredHeaders = new[] { "Id", "Type", "Date", "FirstName", "LastName", "City" };
            return requiredHeaders.All(requiredHeader => headers.Contains(requiredHeader, StringComparer.OrdinalIgnoreCase));
        }

        private bool IsValidHeaderForDocumentItems(string[] headers)
        {
            var requiredHeaders = new[] { "DocumentID", "Ordinal", "Name", "Quantity", "Price", "TaxRate" };
            return requiredHeaders.All(requiredHeader => headers.Contains(requiredHeader, StringComparer.OrdinalIgnoreCase));
        }

        private void ExitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
