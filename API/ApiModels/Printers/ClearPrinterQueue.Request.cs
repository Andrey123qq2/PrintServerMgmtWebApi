using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.ApiModels.Printers
{
    public class ClearPrinterQueueRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PropertyFilter { get; set; }
    }
}
