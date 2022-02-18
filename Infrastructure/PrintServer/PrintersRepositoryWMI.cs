using Domain.Interfaces;
using Infrastructure.PrintServer;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.AccessControl;

namespace PrintServerMgmtWebApi.PrintServer
{
    public class PrintersRepositoryWMI : IPrintersRepository
    {
        private readonly ManagementScope _managementScope;

        public PrintersRepositoryWMI(PrintServerManagementScope printServerManagementScope)
        {
            _managementScope = printServerManagementScope.ManagementScope;
            _managementScope.Connect();
        }

        public ManagementObject Get(string printerName)
        {
            ManagementObject printerManagementObject = GetPrinterManagementObject(printerName);
            if (printerManagementObject == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            return printerManagementObject;
        }

        public ManagementPath Create(string printerName, string shareName, string driverName, string location)
        {
            CreatePrinterPort(printerName);
            ManagementPath result = CreatePrinter(printerName, shareName, driverName, location);
            return result;
        }

        public void Delete(string printerName)
        {
            ManagementObject printerManagementObject = GetPrinterManagementObject(printerName);
            if (printerManagementObject == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            printerManagementObject.Delete();
        }

        public uint RenamePrinter(string printerName, string newName)
        {
            uint result = 10001u;
            ManagementObject printerManagementObject = GetPrinterManagementObject(printerName);
            if (printerManagementObject == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            string currentPrinterName = printerManagementObject.Properties["Name"].Value.ToString();
            if (currentPrinterName == newName)
                return result;
            result = RenamePrinter(printerManagementObject, newName);
            return result;
        }

        public uint RenamePrinter(ManagementObject printerManagementObject, string newName)
        {
            uint result = (uint)printerManagementObject.InvokeMethod("RenamePrinter", new object[] { newName });
            return result;
        }

        public ManagementPath ChangeProperty(string printerName, string property, string newValue)
        {
            ManagementObject printerManagementObject = GetPrinterManagementObject(printerName);
            if (printerManagementObject == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            ManagementPath result = new ManagementPath();
            string currentPropertyValue = printerManagementObject.Properties[property].Value.ToString();
            if (currentPropertyValue != newValue)
                result = ChangeProperty(printerManagementObject, property, newValue);
            return result;
        }

        public ManagementPath ChangeProperty(ManagementObject printerManagementObject, string property, string newValue)
        {
            printerManagementObject[property] = newValue;
            var result = printerManagementObject.Put();
            return result;
        }

        public ManagementObjectCollection GetPrinterQueue(string printerName)
        {
            ManagementObject printerManagementObject = GetPrinterManagementObject(printerName);
            if (printerManagementObject == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            string query = $"SELECT * FROM Win32_PrintJob WHERE Name LIKE \"%{printerName}%\"";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            return collection;
        }

        public uint ClearPrinterQueue(string printerName)
        {
            ManagementObject printerManagementObject = GetPrinterManagementObject(printerName);
            if (printerManagementObject == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            uint result = (uint)printerManagementObject.InvokeMethod("CancelAllJobs", null);
            return result;
        }

        public ManagementObjectCollection GetDrivers()
        {
            string query = $"Select * from Win32_PrinterDriver";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            return collection;
        }

        public uint RemoveFromACL(string printerName, string entityName)
        {
            uint resultCode = 10000;
            var printer = GetPrinterManagementObject(printerName);
            if (printer == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            var result = printer.InvokeMethod("GetSecurityDescriptor", null, null);
            var descriptor = (ManagementBaseObject)result["Descriptor"];
            var flags = (uint)descriptor["ControlFlags"];
            bool entityRemoved = false;
            if ((flags & (uint)ControlFlags.DiscretionaryAclPresent) == (uint)ControlFlags.DiscretionaryAclPresent)
            {
                var dacl = (ManagementBaseObject[])descriptor["DACL"];
                var newDaclList = new List<ManagementBaseObject>();
                foreach (var ace in dacl)
                {
                    var trustee = (ManagementBaseObject)ace["Trustee"];
                    if (trustee["Name"].ToString() != entityName)
                        newDaclList.Add(ace);
                    else
                        entityRemoved = true;
                }
                if (entityRemoved)
                {
                    descriptor.SetPropertyValue("DACL", newDaclList.ToArray());
                    var inParams = printer.GetMethodParameters("SetSecurityDescriptor");
                    inParams["Descriptor"] = descriptor;
                    result = printer.InvokeMethod("SetSecurityDescriptor", inParams, null);
                    resultCode = (uint)result["ReturnValue"];
                }
            }
            return resultCode;
        }

        public uint AddPrintPermission(string printerName, string sid)
        {
            uint resultCode = 10001;
            var printer = GetPrinterManagementObject(printerName);
            printer.Scope = _managementScope;
            if (printer == null)
                throw new PrinterNotFoundException($"Printer {printerName} not found");
            var result = printer.InvokeMethod("GetSecurityDescriptor", null, null);
            var descriptor = (ManagementBaseObject)result["Descriptor"];
            var flags = (uint)descriptor["ControlFlags"];
            bool entityExists = false;
            if ((flags & (uint)ControlFlags.DiscretionaryAclPresent) == (uint)ControlFlags.DiscretionaryAclPresent)
            {
                var dacl = (ManagementBaseObject[])descriptor["DACL"];
                var newDaclList = new List<ManagementBaseObject>();
                newDaclList.AddRange(dacl.ToArray());
                foreach (var ace in dacl)
                {
                    var trustee = (ManagementBaseObject)ace["Trustee"];
                    if (trustee["SIDString"].ToString() == sid && (uint)ace.Properties["AccessMask"].Value == 131080 && (uint)ace.Properties["AceType"].Value == 0)
                        entityExists = true;
                }
                if (!entityExists)
                {
                    var newAce = GetACEForSid(sid);
                    newDaclList.Add(newAce);
                    descriptor.SetPropertyValue("DACL", newDaclList.ToArray());
                    var inParams = printer.GetMethodParameters("SetSecurityDescriptor");
                    inParams["Descriptor"] = descriptor;
                    result = printer.InvokeMethod("SetSecurityDescriptor", inParams, null);
                    resultCode = (uint)result["ReturnValue"];
                }
            }
            return resultCode;
        }


        private ManagementPath CreatePrinter(string printerName, string shareName, string driverName, string location)
        {
            var printerClass = new ManagementClass(_managementScope, new ManagementPath("Win32_Printer"), new ObjectGetOptions());
            printerClass.Get();
            var printer = printerClass.CreateInstance();
            printer.SetPropertyValue("DriverName", driverName);
            printer.SetPropertyValue("PortName", printerName);
            printer.SetPropertyValue("Name", printerName);
            printer.SetPropertyValue("ShareName", shareName);
            printer.SetPropertyValue("DeviceID", printerName);
            printer.SetPropertyValue("Location", location);
            printer.SetPropertyValue("Network", true);
            printer.SetPropertyValue("Shared", true);
            ManagementPath result = printer.Put();
            return result;
        }
        private void CreatePrinterPort(string printerName)
        {
            if (CheckPrinterPort(printerName))
                return;
            var printerPortClass = new ManagementClass(_managementScope, new ManagementPath("Win32_TCPIPPrinterPort"), new ObjectGetOptions());
            printerPortClass.Get();
            var newPrinterPort = printerPortClass.CreateInstance();
            newPrinterPort.SetPropertyValue("Name", printerName);
            newPrinterPort.SetPropertyValue("Protocol", 1);
            newPrinterPort.SetPropertyValue("HostAddress", printerName);
            newPrinterPort.SetPropertyValue("PortNumber", 9100);
            newPrinterPort.SetPropertyValue("SNMPEnabled", false);
            newPrinterPort.Put();
        }
        private bool CheckPrinterPort(string portName)
        {
            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_TCPIPPrinterPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(_managementScope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                if (m["Name"].ToString() == portName)
                    return true;
            }
            return false;
        }

        public ManagementObject GetPrinterManagementObject(string printerName)
        {
            var query = new ObjectQuery($"SELECT * FROM Win32_Printer WHERE Name='{printerName}'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(_managementScope, query);
            ManagementObjectCollection collection = searcher.Get();
            var printerManagementObject = collection.Cast<ManagementObject>().FirstOrDefault();
            return printerManagementObject;
        }

        private ManagementObject GetACEForSid(string sid)
        {
            ManagementObject ace = new ManagementClass(new ManagementScope("\\\\.\\ROOT\\CIMV2"), new ManagementPath("Win32_ACE"), null);
            ManagementObject trustee = new ManagementClass(new ManagementScope("\\\\.\\ROOT\\CIMV2"), new ManagementPath("Win32_Trustee"), null);
            trustee.Properties["SIDString"].Value = sid;
            ace.Properties["Trustee"].Value = trustee;
            ace.Properties["AccessMask"].Value = 131080;
            ace.Properties["AceType"].Value = 0;
            ace.Properties["AceFlags"].Value = 0;
            return ace;
        }

    }
}