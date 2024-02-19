using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyOrganizer.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public string CategoryImage { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; }

    }
}
