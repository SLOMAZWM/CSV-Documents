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
    /// Interaction logic for InformationDocumentPage.xaml
    /// </summary>
    public partial class InformationDocumentPage : Page
    {
        private ManageDocument manageDocument;

        public InformationDocumentPage(ManageDocument Document)
        {
            InitializeComponent();
            manageDocument = Document;
            InitializeEditInformationDocument();
        }

        private void InitializeEditInformationDocument()
        {
            if (manageDocument.IsEditDocument == true)
            {
                TypeDocumentCMB.Text = manageDocument.ActuallyManagedDocument.Type;
                SelectDatePicker.Text = manageDocument.ActuallyManagedDocument.Date;
                FirstNameTxtBox.Text = manageDocument.ActuallyManagedDocument.FirstName;
                LastNameTxtBox.Text = manageDocument.ActuallyManagedDocument.LastName;
                CityTxtBox.Text = manageDocument.ActuallyManagedDocument.City;
            }
            else
            {
                return;
            }
        }

        private void LettersOnlyPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) || c == ' ');
        }

        private void TypeDocumentCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TypeDocumentCMB.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)TypeDocumentCMB.SelectedItem;
                manageDocument.ActuallyManagedDocument.Type = selectedItem.Content.ToString();
            }
            else
            {
                return;
            }
        }

        private void TextUserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            manageDocument.ActuallyManagedDocument.Date = SelectDatePicker.Text;
            manageDocument.ActuallyManagedDocument.FirstName = FirstNameTxtBox.Text;
            manageDocument.ActuallyManagedDocument.LastName = LastNameTxtBox.Text;
            manageDocument.ActuallyManagedDocument.City = CityTxtBox.Text;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            TypeDocumentCMB.SelectedItem = null;
            SelectDatePicker.Text = string.Empty;
            FirstNameTxtBox.Text = string.Empty;
            LastNameTxtBox.Text = string.Empty;
            CityTxtBox.Text = string.Empty;
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (manageDocument.PagesDictionary.TryGetValue("Positions", out Page positionsPage))
            {
                manageDocument.DocumentContentFrame.Navigate(positionsPage);

                Button PositionsButton = manageDocument.PositionsButton;

                manageDocument.ChangeStyleToNavigateButton(PositionsButton);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            manageDocument.Close();
        }
    }
}
