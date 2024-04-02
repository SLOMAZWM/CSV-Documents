using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profisys_Zadanie.Class
{
    public class DocumentItems
    {
        public long DocumentId { get; set; }
        public short Ordinal {  get; set; }
        public string Name { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public byte TaxRate { get; set; }
    }
}
