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

namespace Profisys_Zadanie.ImportExport
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        public List<Document> Documents = new List<Document>();

        public ImportWindow()
        {
            InitializeComponent();
        }

        private void ExitAppBtn_Click(object sender,  RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) 
        {
            this.DragMove();
        }

        public void ValidateCsvFile(string filePath)
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

                if (!IsValidHeaderForDocument(headerRow) && !IsValidHeaderForDocumentItems(headerRow))
                {
                    MessageBox.Show("Niewłaściwy format pliku CSV", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }

                while (csv.Read())
                {
                    Document record = new Document();

                    try
                    {
                        record.Id = uint.Parse(csv.GetField("Id"));
                        record.Type = csv.GetField("Type");
                        record.Date = DateTime.Parse(csv.GetField("Date"));
                        record.FirstName = csv.GetField("FirstName");
                        record.LastName = csv.GetField("LastName");
                        record.City = csv.GetField("City");

                        if (string.IsNullOrEmpty(record.Type) || string.IsNullOrEmpty(record.FirstName) || string.IsNullOrEmpty(record.LastName) || string.IsNullOrEmpty(record.City))
                        {
                            MessageBox.Show("Błąd wiersza CSV: niektóre pola są puste.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            continue;
                        }

                        Documents.Add(record);
                    }
                    catch (FormatException ex)
                    {
                        
                        MessageBox.Show($"Błąd formatu danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue; 
                    }
                    catch (Exception ex)
                    {
                        
                        MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue; 
                    }

                    // Jeżeli wiersz przeszedł walidację, możesz go teraz użyć do dalszego przetwarzania
                }

            }
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

    }
}
