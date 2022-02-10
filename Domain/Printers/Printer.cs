using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Printers
{
    public class Printer
    {
        public string Name { get; set; }
        public string ShareName { get; set; }
        public string DriverName { get; set; }
        public string PrinterStatus { get; set; }
        public string Location { get; set; }
    }
}
