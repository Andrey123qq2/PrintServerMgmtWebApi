using System.ComponentModel.DataAnnotations;

namespace API.ApiModels.Printers
{
    public class CreatePrinterRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShareName { get; set; }
        [Required]
        public string PrinterHostName { get; set; }
        [Required]
        public string DriverName { get; set; }
        [Required]
        public string PropertyFilter { get; set; }
        public string Location { get; set; }
        public string Comment { get; set; }
        
    }
}