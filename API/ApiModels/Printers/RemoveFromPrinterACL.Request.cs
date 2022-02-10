using System.ComponentModel.DataAnnotations;

namespace API.ApiModels.Printers
{
    public class RemoveFromPrinterACLRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string EntityName { get; set; }
    }
}
