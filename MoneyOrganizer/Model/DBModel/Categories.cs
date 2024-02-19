using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyOrganizer.Model.DBModel
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ImageUrl { get; set; }
        public int Priority { get; set; }
    }
}
