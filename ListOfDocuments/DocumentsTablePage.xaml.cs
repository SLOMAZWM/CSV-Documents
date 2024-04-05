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
        public ObservableCollection<Document> Documents {  get; set; }
        private bool editDocument = false;

        public DocumentsTablePage()
        {
            InitializeComponent();
            Documents = ServiceDocumentDataBase.GetAllInformationFromDocuments();
            DocumentsDataGrid.ItemsSource = Documents;
        }

        private void NewDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageDocument newDocumentW = new ManageDocument();
            newDocumentW.ShowDialog();
            Documents = ServiceDocumentDataBase.GetAllInformationFromDocuments();
            DocumentsDataGrid.ItemsSource = Documents;
        }

        private async void EditDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsDataGrid.SelectedItem is Document editedDocument)
            {
                List<DocumentItems> items = await ServiceDocumentDataBase.GetDocumentItemsByDocumentIdAsync(editedDocument.Id);

                editedDocument.Items = items;
                editDocument = true;

                ManageDocument editWindow = new ManageDocument(editedDocument, editDocument);
                editWindow.ShowDialog();

                Documents = ServiceDocumentDataBase.GetAllInformationFromDocuments();
                DocumentsDataGrid.ItemsSource = Documents;
            }
            else
            {
                MessageBox.Show("Wybierz dokument do edycji", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DocumentsDataGrid.SelectedItem = null;
        }
    }
}
