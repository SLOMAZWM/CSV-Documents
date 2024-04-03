using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profisys_Zadanie.Class
{
    public static class DataHelpers
    {
        public static DataTable DocumentsToDataTable(List<Document> documents)
        {
            var table = new DataTable();
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("City", typeof(string));

            foreach (var document in documents)
            {
                var dateString = document.Date ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                table.Rows.Add(document.Type, dateString, document.FirstName, document.LastName, document.City);
            }

            return table;
        }

        public static DataTable DocumentItemsToDataTable(List<DocumentItems> items)
        {
            var table = new DataTable();
            table.Columns.Add("DocumentID", typeof(int));
            table.Columns.Add("Ordinal", typeof(short));
            table.Columns.Add("Product", typeof(string));
            table.Columns.Add("Quantity", typeof(short));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("TaxRate", typeof(byte));

            foreach (var item in items)
            {
                table.Rows.Add(item.DocumentId, item.Ordinal, item.Product, item.Quantity, item.Price, item.TaxRate);
            }

            return table;
        }
    }
}
