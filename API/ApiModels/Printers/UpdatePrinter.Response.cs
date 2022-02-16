using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ApiModels.Printers
{
    public class UpdatePrinterResponse
    {
        public List<string> UpdateResultsCollection { get; set; }
        public string Description { get; set; }
    }
}
