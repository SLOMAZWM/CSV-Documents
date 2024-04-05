using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Profisys_Zadanie.Class;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;

namespace Profisys_Zadanie.ImportExport
{
    public partial class ImportWindow : Window
    {
        public List<Document> Documents = new List<Document>();
        private string selectedDocumentCsvPath;
        private string selectedPositionsCsvPath;

        public ImportWindow()
        {
            InitializeComponent();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedDocumentCsvPath) && !string.IsNullOrEmpty(selectedPositionsCsvPath))
            {
                Documents = LoadDocuments(selectedDocumentCsvPath);
                LoadDocumentItems(selectedPositionsCsvPath);

                await SaveDataToDatabase();
            }
            else
            {
                MessageBox.Show("Proszę wybrać pliki CSV przed zatwierdzeniem.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<int>> SaveDocumentsToDatabase()
        {
            return await ServiceDocumentDataBase.InsertDocumentsAsync(Documents);
        }

        private async Task<bool> SaveDocumentItemsToDatabase(List<int> newDocumentIds)
        {
            try
            {
                UpdateDocumentItemsWithNewIds(Documents, newDocumentIds);

                List<DocumentItems> allItems = Documents.SelectMany(doc => doc.Items ?? Enumerable.Empty<DocumentItems>()).ToList();
                await ServiceDocumentDataBase.InsertDocumentItemsAsync(allItems);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisywania elementów dokumentów do bazy danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async Task<bool> SaveDataToDatabase()
        {
            try
            {
                var newDocumentIds = await SaveDocumentsToDatabase();
                if (newDocumentIds == null || !newDocumentIds.Any())
                {
                    MessageBox.Show("Nie udało się zapisać dokumentów do bazy danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                UpdateDocumentItemsWithNewIds(Documents, newDocumentIds);

                var itemsSavedSuccessfully = await SaveDocumentItemsToDatabase(newDocumentIds);
                if (!itemsSavedSuccessfully)
                {
                    MessageBox.Show("Nie udało się zapisać pozycji dokumentów do bazy danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                MessageBox.Show("Dane zostały pomyślnie zapisane do bazy danych.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                DataRefreshService.RequestDataRefresh();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisu do bazy danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private List<Document> LoadDocuments(string filePath)
        {
            var documents = new List<Document>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true,
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var document = new Document
                    {
                        Id = csv.GetField<int>("Id"),
                        Type = csv.GetField<string>("Type"),
                        Date = csv.GetField<string>("Date"),
                        FirstName = csv.GetField<string>("FirstName"),
                        LastName = csv.GetField<string>("LastName"),
                        City = csv.GetField<string>("City"),
                    };
                    documents.Add(document);
                }
            }
            return documents;
        }

        private void LoadDocumentItems(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var documentItems = csv.GetRecords<DocumentItems>().ToList();

                foreach (var item in documentItems)
                {
                    var document = Documents.FirstOrDefault(doc => doc.Id == item.DocumentId);
                    if (document != null)
                    {
                        if (document.Items == null)
                            document.Items = new List<DocumentItems>();

                        document.Items.Add(item);
                    }
                    else
                    {
                        Console.WriteLine($"Nie znaleziono dokumentu o ID {item.DocumentId} dla elementu.");
                    }
                }
            }
        }

        private void SelectDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                if (ValidateCsvFile(openFileDialog.FileName, isDocument: true))
                {
                    selectedDocumentCsvPath = openFileDialog.FileName;
                    MessageBox.Show("Plik został poprawnie załadowany!", "Poprawny format", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Aktualizuj Label z wybraną ścieżką
                }
                else
                {
                    MessageBox.Show("Niepoprawnie wczytany plik!", "Błąd formatu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                if (ValidateCsvFile(openFileDialog.FileName, isDocument: false))
                {
                    selectedPositionsCsvPath = openFileDialog.FileName;
                    MessageBox.Show("Plik został poprawnie załadowany!", "Poprawny format", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Aktualizuj Label z wybraną ścieżką
                }
                else
                {
                    MessageBox.Show("Niepoprawnie wczytany plik!", "Błąd formatu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public bool ValidateCsvFile(string filePath, bool isDocument)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ";",
            };

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
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
                        GetMissingHeaders(headerRow, new[] { "DocumentID", "Ordinal", "Product", "Quantity", "Price", "TaxRate" });

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
            var requiredHeaders = new[] { "DocumentID", "Ordinal", "Product", "Quantity", "Price", "TaxRate" };
            return requiredHeaders.All(requiredHeader => headers.Contains(requiredHeader, StringComparer.OrdinalIgnoreCase));
        }

        private void UpdateDocumentItemsWithNewIds(List<Document> documents, List<int> newDocumentIds)
        {
            for (int i = 0; i < documents.Count; i++)
            {
                var document = documents[i];
                if (document.Items != null)
                {
                    foreach (var item in document.Items)
                    {
                        item.DocumentId = newDocumentIds[i];
                    }
                }
                else
                {
                    MessageBox.Show("Błąd przypisywania pozycji do dokumentu!", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void ExitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
