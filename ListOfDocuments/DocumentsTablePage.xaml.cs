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

        public DocumentsTablePage()
        {
            InitializeComponent();
            Documents = ServiceDocumentDataBase.GetAllInformationFromDocuments();
            DocumentsDataGrid.ItemsSource = Documents;
        }

        private void NewDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageDocument newDocument = new ManageDocument();
            newDocument.ShowDialog();
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DocumentsDataGrid.SelectedItem = null;
        }
    }
}
