using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.ApiModels.Printers
{
    public class CreatePrinterRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShareName { get; set; }
        [Required]
        public string DriverName { get; set; }
        public string Location { get; set; }
    }
}
