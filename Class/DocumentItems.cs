using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profisys_Zadanie.Class
{
    public class DocumentItems
    {
        public int DocumentId { get; set; }
        public short Ordinal {  get; set; }
        public string Product { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public byte TaxRate { get; set; }
    }
}
