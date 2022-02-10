using Domain.Printers;
using System.Management;

namespace Domain.Interfaces
{
    public interface IPrintersRepository
    {
        ManagementObject Get(string printerName);
        void Create(string printerName, string shareName, string driverName, string location);
        void Delete(string printerName);
        void RenamePrinter(string printerName, string newName);
        void RenamePrinter(ManagementObject printer, string newName);
        void ChangeProperty(string printerName, string property, string newValue);
        void ChangeProperty(ManagementObject printerManagementObject, string property, string newValue);
        ManagementObjectCollection GetPrinterQueue(string printerName);
        void ClearPrinterQueue(string printerName);
        uint RemoveFromACL(string printerName, string entityName);
        uint AddPrintPermission(string printerName, string sid);
        ManagementObjectCollection GetDrivers();
    }
}