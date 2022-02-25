using Domain.Printers;
using System;
using System.Management;

namespace Domain.Interfaces
{
    public interface IPrintersRepository
    {
        ManagementObject Get(string printerName, string propertyFilter);
        ManagementPath Create(string printerName, string shareName, string printerHostName, string driverName, string location, string propertyFilter, string comment);
        void Delete(string printerName);
        uint RenamePrinter(string printerHostName, string newName, string propertyFilter);
        uint RenamePrinter(ManagementObject printer, string newName);
        ManagementPath ChangeProperty(string printerHostName, string property, string newValue, string propertyFilter);
        ManagementPath ChangeProperty(ManagementObject printerManagementObject, string property, string newValue);
        ManagementObjectCollection GetPrinterQueue(string printerHostName, string printerName, string propertyFilter);
        uint ClearPrinterQueue(string printerHostName, string propertyFilter);
        uint RemoveFromACL(string printerHostName, string entityName, string propertyFilter);
        uint AddPrintPermission(string printerHostName, string sid, string propertyFilter);
        ManagementObjectCollection GetDrivers();
        ManagementObject GetPrinterManagementObject(string propertyValue, string propertyName);
    }
}