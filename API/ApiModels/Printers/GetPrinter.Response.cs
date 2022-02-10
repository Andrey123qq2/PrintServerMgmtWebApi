namespace API.ApiModels.Printers
{
    public class GetPrinterResponse
    {
        public string Name { get; set; }
        public string ShareName { get; set; }
        public string DriverName { get; set; }
        public string Location { get; set; }
        public string PrinterStatus { get; set; }        
    }
}
