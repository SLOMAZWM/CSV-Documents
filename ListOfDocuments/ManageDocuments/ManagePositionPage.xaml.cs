using Profisys_Zadanie.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ManagePositionPage.xaml
    /// </summary>
    public partial class ManagePositionPage : Page
    {
        private bool edit = false;
        PositionsDocumentPage positionsDocumentPage;
        public ManagePositionPage(PositionsDocumentPage positionsDocument)
        {
            positionsDocumentPage = positionsDocument;
            InitializeComponent();
            InitializeAdd();
            edit = false;
        }

        public ManagePositionPage(PositionsDocumentPage positionsDocument, bool edited)
        {
            positionsDocumentPage = positionsDocument;
            InitializeComponent();
            edit = edited;
            InitializeEdit();
        }

        private bool IsNotEmpty()
        {
            if (string.IsNullOrEmpty(ProductNameTxtBox.Text))
            {
                MessageBox.Show("Wpisz nazwę produktu!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(QuantityTxtBox.Text))
            {
                MessageBox.Show("Wpisz ilość!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(PriceForOneTxtBox.Text))
            {
                MessageBox.Show("Wpisz cenę za sztukę!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(TaxRateTxtBox.Text))
            {
                MessageBox.Show("Wpisz procent podatku!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void AddEditProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNotEmpty())
            {
                DocumentItems newItemInDocument = new DocumentItems()
                {
                    Ordinal = (short)(positionsDocumentPage.PositionsDataGrid.Items.Count + 1),
                    Product = ProductNameTxtBox.Text,
                    Quantity = short.Parse(QuantityTxtBox.Text),
                    Price = decimal.Parse(PriceForOneTxtBox.Text),
                    TaxRate = byte.Parse(TaxRateTxtBox.Text)
                };

                List<DocumentItems> itemsSource = positionsDocumentPage.PositionsDataGrid.ItemsSource as List<DocumentItems>;

                if (itemsSource == null)
                {
                    itemsSource = new List<DocumentItems>();
                }

                if (!edit)
                {
                    itemsSource.Add(newItemInDocument);
                }
                else
                {
                    if (positionsDocumentPage.PositionsDataGrid.SelectedItem is DocumentItems selectedItem)
                    {
                        int index = itemsSource.IndexOf(selectedItem);
                        if (index != -1)
                        {
                            itemsSource[index] = newItemInDocument;
                        }
                        else
                        {
                            MessageBox.Show("Nie można zaktualizować wybranego elementu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }

                positionsDocumentPage.PositionsDataGrid.ItemsSource = null;
                positionsDocumentPage.PositionsDataGrid.ItemsSource = itemsSource;

                positionsDocumentPage.ManageDocument.DocumentContentFrame.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Proszę wypełnić wszystkie pola.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void QuantityTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void QuantityTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                bool isValid = short.TryParse(textBox.Text, out short result) && result >= 0;

                if (!isValid)
                {
                    MessageBox.Show("Wprowadzona wartość jest poza zakresem dla ilości! Maksymalna dozwolona wartość to 32 767.", "Błąd wartości", MessageBoxButton.OK, MessageBoxImage.Error);

                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        private void PriceForOneTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var isDigitOrComma = e.Text.All(c => char.IsDigit(c) || c == ',');

            bool isCommaAlreadyPresent = textBox.Text.Contains(",");
            bool isInputComma = e.Text.Equals(",");

            if (isInputComma && isCommaAlreadyPresent)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = !isDigitOrComma;
            }
        }

        private void PriceForOneTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string input = textBox.Text.Replace(',', '.');
                bool isValid = decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);

                int maxDigitsBeforeDecimalPoint = 20;
                int maxDigitsAfterDecimalPoint = 2;
                int commaIndex = textBox.Text.IndexOf(',');

                if (isValid)
                {
                    if (result >= (decimal)Math.Pow(10, maxDigitsBeforeDecimalPoint) ||
                        (commaIndex != -1 && textBox.Text.Length - commaIndex - 1 > maxDigitsAfterDecimalPoint))
                    {
                        MessageBox.Show($"Wprowadzona wartość jest za duża. Maksymalna liczba cyfr przed przecinkiem: {maxDigitsBeforeDecimalPoint}, po przecinku: {maxDigitsAfterDecimalPoint}.", "Błąd wartości", MessageBoxButton.OK, MessageBoxImage.Warning);

                        textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                }
                else
                {
                    MessageBox.Show("Wprowadzona wartość nie jest prawidłową liczbą.", "Błąd formatu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }


        private void TaxRateTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void TaxRateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (int.TryParse(textBox.Text, out int value) && value > 100)
            {
                textBox.Text = "100";
            }
        }

        private void LocationExitBtn_Click(object sender, EventArgs e)
        {
            positionsDocumentPage.ManageDocument.DocumentContentFrame.NavigationService.GoBack();
        }

        private void InitializeAdd()
        {
            PositionManageLbl.Content = "Dodaj pozycję";
        }

        private void InitializeEdit()
        {
            PositionManageLbl.Content = "Edytuj pozycję";

            // Sprawdzenie, czy SelectedItem nie jest null
            if (positionsDocumentPage.PositionsDataGrid.SelectedItem != null)
            {
                DocumentItems editItem = (DocumentItems)positionsDocumentPage.PositionsDataGrid.SelectedItem;

                ProductNameTxtBox.Text = editItem.Product;
                QuantityTxtBox.Text = editItem.Quantity.ToString();
                PriceForOneTxtBox.Text = editItem.Price.ToString();
                TaxRateTxtBox.Text = editItem.TaxRate.ToString();
            }
            else
            {
                // Obsługa sytuacji, gdy SelectedItem jest null
                MessageBox.Show("Nie wybrano elementu do edycji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
