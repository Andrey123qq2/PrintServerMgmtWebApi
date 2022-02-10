using Domain.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ApiModels.Printers
{
    public class GetPrinterQueueResponse
    {
        public List<PrintJob> PrintJobsCollection { get; set; }
    }
}
