using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Profisys_Zadanie.Class
{
    public class Document
    {
        public int OriginalId { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public List<DocumentItems> Items { get; set; } = new List<DocumentItems>();

        public bool IsDocumentNotEmpty()
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                MessageBox.Show("Uzupełnij typ dokumentu!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(Date))
            {
                MessageBox.Show("Uzupełnij datę dokumentu!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("Uzupełnij imię!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("Uzupełnij nazwisko!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(City))
            {
                MessageBox.Show("Uzupełnij miasto!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (Items.Count == 0)
            {
                MessageBox.Show("Dokument musi zawierać przynajmniej jedną pozycję!", "Błąd wypełnienia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
