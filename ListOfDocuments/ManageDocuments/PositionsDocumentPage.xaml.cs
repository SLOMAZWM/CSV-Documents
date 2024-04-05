using Profisys_Zadanie.Class;
using System;
using System.Collections.Generic;
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

namespace Profisys_Zadanie.ListOfDocuments.ManageDocuments
{
    /// <summary>
    /// Interaction logic for PositionsDocumentPage.xaml
    /// </summary>
    public partial class PositionsDocumentPage : Page
    {
        public short Ordinal = 1;
        public ManageDocument ManageDocument { get; set; }
        private bool editPosition = false;
        public PositionsDocumentPage(ManageDocument actuallyD)
        {
            InitializeComponent();
            ManageDocument = actuallyD;
            InitializeEditPositionsInDocument();
        }

        private void InitializeEditPositionsInDocument()
        {
            if(ManageDocument.IsEditDocument == true)
            {
                PositionsDataGrid.ItemsSource = ManageDocument.ActuallyManagedDocument.Items;
            }
            else
            {
                return;
            }
        }

        private async void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            List<DocumentItems> documentItemsList = new List<DocumentItems>();

            foreach(DocumentItems documentItems in PositionsDataGrid.Items)
            {
                documentItemsList.Add(documentItems);
            }

            ManageDocument.ActuallyManagedDocument.Items = documentItemsList;

            if(ManageDocument.IsEditDocument == false)
            {
                if (ManageDocument.ActuallyManagedDocument.IsDocumentNotEmpty() == true)
                {
                    await ServiceDocumentDataBase.InsertDocumentAndItemsAsync(ManageDocument.ActuallyManagedDocument);
                    MessageBox.Show("Poprawnie dodano dokument do bazy danych!", "Poprawny zapis", MessageBoxButton.OK, MessageBoxImage.Information);
                    ManageDocument.Close();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (ManageDocument.ActuallyManagedDocument.IsDocumentNotEmpty() == true)
                {
                    await ServiceDocumentDataBase.UpdateDocumentAndItemsAsync(ManageDocument.ActuallyManagedDocument);
                    MessageBox.Show("Poprawnie nadpisano dokument w bazie danych!", "Poprawny zapis", MessageBoxButton.OK, MessageBoxImage.Information);
                    ManageDocument.Close();
                }
                else
                {
                    return;
                }
            }
        }

        private void AddPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageDocument.DocumentContentFrame.Navigate(new ManagePositionPage(this));
        }

        private void EditPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            if(PositionsDataGrid.SelectedItem != null)
            {
                editPosition = true;
                ManageDocument.DocumentContentFrame.Navigate(new ManagePositionPage(this, editPosition));
            }
            else
            {
                MessageBox.Show("Wybierz produkt do edycji!", "Błąd wyboru", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeletePositionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsDataGrid.SelectedItem != null)
            {
                var result = MessageBox.Show("Czy na pewno chcesz usunąć wybraną pozycję?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    PositionsDataGrid.Items.Remove(PositionsDataGrid.SelectedItem);
                    MessageBox.Show("Poprawnie usunięto pozycję", "Usuwanie powiodło się", MessageBoxButton.OK, MessageBoxImage.Information);

                    UpdateOrdinalAfterDelete();
                }
            }
            else
            {
                MessageBox.Show("Zaznacz pozycję do usunięcia!", "Usuwanie nie powiodło się", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateOrdinalAfterDelete()
        {
            short newOrdinal = 1;
            foreach (var item in PositionsDataGrid.Items)
            {
                if (item is DocumentItems documentItem)
                {
                    documentItem.Ordinal = newOrdinal++;
                }
            }

            PositionsDataGrid.Items.Refresh();
        }


        private void ClearDataGridBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz wyczyścić całą listę pozycji?", "Potwierdzenie czyszczenia", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                PositionsDataGrid.Items.Clear();
                Ordinal = 1;
                MessageBox.Show("Lista pozycji została wyczyszczona.", "Czyszczenie powiodło się", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PositionsDataGrid.SelectedItem = null;
        }
    }
}
