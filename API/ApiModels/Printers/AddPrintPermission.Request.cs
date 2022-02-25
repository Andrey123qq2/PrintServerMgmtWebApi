using System.ComponentModel.DataAnnotations;

namespace API.ApiModels.Printers
{
    public class AddPrintPermissionRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Sid { get; set; }
        [Required]
        public string PropertyFilter { get; set; }
    }
}
