using PrintServerMgmtWebApi.Repository;
using Domain.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using PrintServerMgmtWebApi.PrintServer;

namespace PrintServerMgmtWebApi.Repository.Tests
{
    [TestFixture()]
    public class PrintersRepositoryWMITests
    {
        private IPrintersRepository _repository;
        [SetUp]
        public void Setup()
        {
            var wmiScope = new PrintServerManagementScope();
            _repository = new PrintersRepositoryWMI(wmiScope);
        }

        [Test()]
        public void CreateTest()
        {
            string printerName = "PRN-TEST-API";
            string shareName = "PRN-TEST-API-SHARE";
            string printerHostName = "PPRN-TEST-API";
            string driverName = "HP Universal Printing PCL 6";
            string location = "Test Office";
            string propertyFilter = "PortName";
            string comment = "123";
            var result = _repository.Create(printerName, shareName, printerHostName, driverName, location, propertyFilter, comment);
            StringAssert.Contains("DeviceID=\"" + printerName, result.ToString());
        }

        [Test()]
        public void DeleteTest()
        {
            string printerName = "PRN-TEST-API-DELETE";
            //_repository.Create(printerName, shareName, driverName, location);
            Assert.DoesNotThrow(() => _repository.Delete(printerName));
        }

        [Test()]
        public void GetTest()
        {
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            var result = _repository.Get(printerHostName, propertyFilter);
            if (result == null)
                Assert.Fail("Expected printer {0} has not been found", printerHostName);
            else
                StringAssert.Contains("DeviceID=\"" + printerHostName, result.ToString());
        }

        [Test()]
        public void RenamePrinterTest()
        {
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            string printerName = "PRN-TEST-API";
            string printerNameNew = "PRN-TEST-API-2";
            var result = _repository.RenamePrinter(printerHostName, printerNameNew, propertyFilter);
            if (result == 10000u)
                Assert.Fail("Expected printer {0} has not been found", printerName);
            else
            {
                Assert.AreEqual(0, result);
                _repository.RenamePrinter(printerHostName, printerName, propertyFilter);
            }
        }

        [Test()]
        public void ChangePropertyTest()
        {
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            string propertyName = "Location";
            string newValue = "Test Office-2";
            string initValue = "Test Office";
            ManagementObject mgmtObj = new ManagementObject();

            var result = _repository.ChangeProperty(printerHostName, propertyName, newValue, propertyFilter);
            if (String.IsNullOrEmpty(result.ToString()))
                Assert.Fail("Printer's {0} property {1} has already expected value", printerHostName, propertyName);
            mgmtObj.Path = result;
            string resultPropertyValue = mgmtObj[propertyName].ToString();

            StringAssert.AreEqualIgnoringCase(resultPropertyValue, newValue);

            _repository.ChangeProperty(printerHostName, propertyName, initValue, propertyFilter);
        }

        [Test()]
        public void GetDriversTest()
        {
            string expectedDriverName = "HP Universal Printing PCL 6";
            ManagementObjectCollection driversCollection = _repository.GetDrivers();
            List<string> driversNames = driversCollection
                .Cast<ManagementObject>()
                .ToList()
                .Select(m => m.Properties["Name"].Value?.ToString().Split(",")[0])
                .ToList();

            Assert.Contains(expectedDriverName, driversNames);
        }

        [Test()]
        public void GetPrinterQueueTest()
        {
            string printerName = "PRN-TEST-API";
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            string expectedJobName = "TestDocument.txt - Notepad";
            ManagementObjectCollection jobCollection = _repository.GetPrinterQueue(printerHostName, printerName, propertyFilter);
            List<string> jobNames = jobCollection
                .Cast<ManagementObject>()
                .ToList()
                .Select(m => m.Properties["Document"].Value?.ToString().Split(",")[0])
                .ToList();

            Assert.Contains(expectedJobName, jobNames);
        }

        [Test()]
        public void ClearPrinterQueueTest()
        {
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            uint result = _repository.ClearPrinterQueue(printerHostName, propertyFilter);
            Assert.AreEqual(0, result);
        }

        [Test()]
        public void AddPrintPermissionTest()
        {
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            string testSid = "S-1-5-32-550";
            string testName = "Print Operators";
            var result = _repository.AddPrintPermission(printerHostName, testSid, propertyFilter);
            if (result == 10001u)
                Assert.Fail("Expected entity {0} already exists", testSid);
            else
            {
                Assert.AreEqual(0, result);
                _repository.RemoveFromACL(printerHostName, testName, propertyFilter);
            }
        }

        [Test()]
        public void RemoveFromACLTest()
        {
            string printerHostName = "PRN-TEST-API";
            string propertyFilter = "PortName";
            string testSid = "S-1-1-0";
            string testName = "Everyone";
            var result = _repository.RemoveFromACL(printerHostName, testName, propertyFilter);
            if (result == 10000u)
                Assert.Fail("No such entity {0}", testSid);
            else
            {
                Assert.AreEqual(0, result);
                _repository.AddPrintPermission(printerHostName, testName, propertyFilter);
            }
        }
    }
}