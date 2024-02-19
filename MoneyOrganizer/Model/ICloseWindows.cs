using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyOrganizer.Model
{
    public interface ICloseWindows
    {
        Action Close { get; set; }

        bool CanClose();
    }
}
