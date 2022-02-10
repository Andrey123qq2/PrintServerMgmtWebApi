using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.ApiModels.Printers
{
    public class DeletePrinterRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
