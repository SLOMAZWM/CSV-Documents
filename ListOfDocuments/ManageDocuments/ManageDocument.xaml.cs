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
using System.Windows.Shapes;
using Profisys_Zadanie.Class;

namespace Profisys_Zadanie.ListOfDocuments.ManageDocuments
{
    /// <summary>
    /// Interaction logic for ManageDocument.xaml
    /// </summary>
    public partial class ManageDocument : Window
    {
        private Dictionary<string, Page> pagesDictionary;
        Button activeNavigationButton;
        public Document ActuallyManagedDocument {  get; set; }
        public bool IsEditDocument = false;

        public ManageDocument()
        {
            InitializeComponent();
            pagesDictionary = InitializePages();
            ActuallyManagedDocument = new Document();
        }

        public ManageDocument(Document editDocument, bool isEdit)
        {
            IsEditDocument = isEdit;
            ActuallyManagedDocument = editDocument;
            InitializeComponent();
            pagesDictionary = InitializePages();
        }

        private void NavigationBtn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is string pageKey)
            {
                Page pageToNavigate;
                if (pagesDictionary.TryGetValue(pageKey, out pageToNavigate))
                {
                    DocumentContentFrame.Navigate(pageToNavigate);
                    ChangeStyleToNavigateButton(button);
                }
            }
        }

        private void ChangeStyleToNavigateButton(Button button)
        {
            if (activeNavigationButton != null)
            {
                activeNavigationButton.Style = FindResource("NavigationButtonTop") as Style;
            }

            button.Style = FindResource("ActivatedNavigationButtonTop") as Style;
            activeNavigationButton = button;
        }

        private Dictionary<string, Page> InitializePages()
        {
            pagesDictionary = new Dictionary<string, Page>()
            {
                {"Information", new InformationDocumentPage(this) },
                {"Positions", new PositionsDocumentPage(this) }
            };

            return pagesDictionary;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
