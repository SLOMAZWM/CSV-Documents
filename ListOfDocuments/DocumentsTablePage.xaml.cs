using iTextSharp.text;
using Profisys_Zadanie.Class;
using Profisys_Zadanie.ListOfDocuments.ManageDocuments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Profisys_Zadanie.ListOfDocuments
{
    /// <summary>
    /// Interaction logic for DocumentsTablePage.xaml
    /// </summary>
    public partial class DocumentsTablePage : Page
    {
        public ObservableCollection<Profisys_Zadanie.Class.Document> AllDocuments {  get; set; }
        private bool editDocument = false;
        private int currentPage = 1;
        private int totalPages = 0;
        private int itemsPerPage = 20;

        public DocumentsTablePage()
        {
            InitializeComponent();
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            AllDocuments = ServiceDocumentDataBase.GetAllInformationFromDocuments();
            RefreshDataInDocumentsDataGrid();
            SetupPagination();
        }

        private void SetupPagination()
        {
            totalPages = (int)Math.Ceiling(AllDocuments.Count / (double)itemsPerPage);
            UpdatePaginationButtons();
        }

        private void NewDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageDocument newDocumentW = new ManageDocument();
            newDocumentW.ShowDialog();
            RefreshDataInDocumentsDataGrid();
        }

        private async void EditDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsDataGrid.SelectedItem is Profisys_Zadanie.Class.Document editedDocument)
            {
                List<DocumentItems> items = await ServiceDocumentDataBase.GetDocumentItemsByDocumentIdAsync(editedDocument.Id);

                editedDocument.Items = items;
                editDocument = true;

                ManageDocument editWindow = new ManageDocument(editedDocument, editDocument);
                editWindow.ShowDialog();

                RefreshDataInDocumentsDataGrid();
            }
            else
            {
                MessageBox.Show("Wybierz dokument do edycji", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            if(DocumentsDataGrid.SelectedItem != null)
            {
                Profisys_Zadanie.Class.Document deleteDocument = (Profisys_Zadanie.Class.Document)DocumentsDataGrid.SelectedItem;
                var result = MessageBox.Show($"Czy aby na pewno chcesz usunąć dokument o ID: {deleteDocument.Id}?", "Potwierdź wybór", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    await ServiceDocumentDataBase.DeleteDocumentAndDocumentItemsAsync(deleteDocument.Id);
                    RefreshDataInDocumentsDataGrid();
                    MessageBox.Show("Poprawnie usunięto dokument i powiązane z nim pozycje!", "Usunięto prawidłowo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Usuwanie zostało wycofane");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Wybierz dokument do usunięcia!", "Błąd zaznaczenia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private async void DetailsDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            if(DocumentsDataGrid.SelectedItem != null)
            {
                Profisys_Zadanie.Class.Document document = (Profisys_Zadanie.Class.Document)DocumentsDataGrid.SelectedItem;
                document.Items = await ServiceDocumentDataBase.GetDocumentItemsByDocumentIdAsync(document.Id);
                DetailDocumentWindow showDetailDocument = new DetailDocumentWindow(document);
                showDetailDocument.ShowDialog();
            }
            else
            {
                MessageBox.Show("Zaznacz dokument, by zobaczyć jego szczegóły!", "Błąd zaznaczenia", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void RefreshDataInDocumentsDataGrid()
        {
            var paginatedDocuments = AllDocuments.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);
            DocumentsDataGrid.ItemsSource = new ObservableCollection<Profisys_Zadanie.Class.Document>(paginatedDocuments);
            CalculateDocuments();
        }

        private void UpdatePaginationButtons()
        {
            int maxPage = (int)Math.Ceiling((double)AllDocuments.Count / itemsPerPage);
            PaginationItemsControl.Items.Clear();

            int halfRange = 3;
            int startPage = Math.Max(1, currentPage - halfRange);
            int endPage = Math.Min(maxPage, currentPage + halfRange);

            if (endPage - startPage < 5)
            {
                if (startPage > 1) startPage = Math.Max(1, endPage - 5);
                else endPage = Math.Min(maxPage, startPage + 5);
            }

            for (int pageNumber = startPage; pageNumber <= endPage; pageNumber++)
            {
                var button = new Button
                {
                    Content = pageNumber.ToString(),
                    Style = FindResource("pagingButton") as Style
                };
                button.Click += PageButton_Click;

                PaginationItemsControl.Items.Add(button);
            }

            UpdatePaginationButtonStyles();
        }


        private void UpdateDisplayedDocuments()
        {
            int startIndex = (currentPage - 1) * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, AllDocuments.Count);

            var displayedDocuments = new ObservableCollection<Profisys_Zadanie.Class.Document>(AllDocuments.Skip(startIndex).Take(itemsPerPage));

            DocumentsDataGrid.ItemsSource = displayedDocuments;

            UpdatePaginationButtonStyles();
            UpdatePaginationButtons();
            CalculateDocuments();
        }


        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Content?.ToString(), out int pageNumber))
            {
                currentPage = pageNumber;
                UpdateDisplayedDocuments();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int maxPage = (int)Math.Ceiling((double)AllDocuments.Count / itemsPerPage);
            if (currentPage < maxPage)
            {
                currentPage++;
                UpdateDisplayedDocuments();
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDisplayedDocuments();
            }
        }

        private void FirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            UpdateDisplayedDocuments();
        }

        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            int totalItems = AllDocuments.Count;
            int maxPage = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            currentPage = maxPage;
            UpdateDisplayedDocuments();
        }

        private void UpdatePaginationButtonStyles()
        {
            foreach (UIElement element in PaginationItemsControl.Items)
            {
                if (element is Button button)
                {
                    if (int.TryParse(button.Content?.ToString(), out int pageNumber))
                    {
                        Brush currentPageBackground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC107")); // Żółte tło
                        Brush currentPageForeground = Brushes.White; 

                       
                        Brush otherPageBackground = Brushes.Transparent; 
                        Brush otherPageForeground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC107")); // Żółty tekst

                        if (pageNumber == currentPage)
                        {
                            button.Background = currentPageBackground;
                            button.Foreground = currentPageForeground;
                        }
                        else
                        {
                            button.Background = otherPageBackground;
                            button.Foreground = otherPageForeground;
                        }
                    }
                }
            }
        }


        private void CalculateDocuments()
        {
            AmountOfDocumentsTxtBlock.Text = DocumentsDataGrid.Items.Count.ToString();
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DocumentsDataGrid.SelectedItem = null;
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllDocuments == null) return;

            var filteredDocuments = string.IsNullOrEmpty(txtFilter.Text)
                ? AllDocuments
                : new ObservableCollection<Profisys_Zadanie.Class.Document>(AllDocuments.Where(doc =>
                    (doc.Date != null && doc.Date.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (doc.FirstName != null && doc.FirstName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (doc.LastName != null && doc.LastName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (doc.City != null && doc.City.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                ));

            DocumentsDataGrid.ItemsSource = filteredDocuments;
            CalculateDocuments();
        }

    }
}
