using Domain.Printers;
using System;
using System.Management;

namespace Domain.Interfaces
{
    public interface IPrintersRepository
    {
        ManagementObject Get(string printerName);
        ManagementPath Create(string printerName, string shareName, string driverName, string location);
        void Delete(string printerName);
        uint RenamePrinter(string printerName, string newName);
        uint RenamePrinter(ManagementObject printer, string newName);
        ManagementPath ChangeProperty(string printerName, string property, string newValue);
        ManagementPath ChangeProperty(ManagementObject printerManagementObject, string property, string newValue);
        ManagementObjectCollection GetPrinterQueue(string printerName);
        uint ClearPrinterQueue(string printerName);
        uint RemoveFromACL(string printerName, string entityName);
        uint AddPrintPermission(string printerName, string sid);
        ManagementObjectCollection GetDrivers();
    }
}