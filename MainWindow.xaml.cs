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
using Profisys_Zadanie.ImportExport;
using Profisys_Zadanie.ListOfDocuments;
using Profisys_Zadanie.Information;

namespace Profisys_Zadanie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, Page> pagesDictionary;

        public MainWindow()
        {
            InitializeComponent();
            pagesDictionary = InitializePage();
        }

        private Dictionary<string, Page> InitializePage()
        {
            Dictionary<string, Page> pages = new Dictionary<string, Page>();
            pages.Add("Import", new ImportPage());
            pages.Add("Export", new ExportPage());
            pages.Add("DocumentsTable", new DocumentsTablePage());
            pages.Add("Information", new InformationPage());
            return pages;
        }

        private void NavigationBtn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is string pageKey)
            {
                Page pageToNavigate;
                if (pagesDictionary.TryGetValue(pageKey, out pageToNavigate))
                {
                    ContentFrame.Navigate(pageToNavigate);
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimalizeAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
