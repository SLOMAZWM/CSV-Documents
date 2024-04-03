using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
