using Domain.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

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
            string driverName = "HP Universal Printing PCL 6";
            string location = "Test Office";
            var result = _repository.Create(printerName, shareName, driverName, location);
            StringAssert.Contains("DeviceID=\"" + printerName, result.ToString());
        }

        [Test()]
        public void GetTest()
        {
            string printerName = "PRN-TEST-API";
            var result = _repository.Get(printerName);
            if (result == null)
                Assert.Fail("Expected printer {0} has not been found", printerName);
            else
                StringAssert.Contains("DeviceID=\"" + printerName, result.ToString());
        }

        [Test()]
        public void RenamePrinterTest()
        {
            string printerName = "PRN-TEST-API";
            string printerNameNew = "PRN-TEST-API-2";
            var result = _repository.RenamePrinter(printerName, printerNameNew);
            if (result == 10000u)
                Assert.Fail("Expected printer {0} has not been found", printerName);
            else
            {
                Assert.AreEqual(0, result);
                _repository.RenamePrinter(printerNameNew, printerName);
            }
        }

        [Test()]
        public void ChangePropertyTest()
        {
            string printerName = "PRN-TEST-API";
            string propertyName = "Location";
            string newValue = "Test Office-2";
            string initValue = "Test Office";
            ManagementObject mgmtObj = new ManagementObject();

            var result = _repository.ChangeProperty(printerName, propertyName, newValue);
            if (String.IsNullOrEmpty(result.ToString()))
                Assert.Fail("Printer's property {1} has already expected value", printerName, propertyName);
            mgmtObj.Path = result;
            string resultPropertyValue = mgmtObj[propertyName].ToString();

            StringAssert.AreEqualIgnoringCase(resultPropertyValue, newValue);

            _repository.ChangeProperty(printerName, propertyName, initValue);
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
            string expectedJobName = "TestDocument.txt - Notepad";
            ManagementObjectCollection jobCollection = _repository.GetPrinterQueue(printerName);
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
            string printerName = "PRN-TEST-API";
            uint result = _repository.ClearPrinterQueue(printerName);
            Assert.AreEqual(0, result);
        }

        [Test()]
        public void AddPrintPermissionTest()
        {
            string printerName = "PRN-TEST-API";
            string testSid = "S-1-5-32-550";
            string testName = "Print Operators";
            var result = _repository.AddPrintPermission(printerName, testSid);
            if (result == 10001u)
                Assert.Fail("Expected entity {0} already exists", testSid);
            else
            {
                Assert.AreEqual(0, result);
                _repository.RemoveFromACL(printerName, testName);
            }
        }

        [Test()]
        public void RemoveFromACLTest()
        {
            string printerName = "PRN-TEST-API";
            string testSid = "S-1-1-0";
            string testName = "Everyone";
            var result = _repository.RemoveFromACL(printerName, testName);
            if (result == 10000u)
                Assert.Fail("No such entity {0}", testSid);
            else
            {
                Assert.AreEqual(0, result);
                _repository.AddPrintPermission(printerName, testSid);
            }
        }
    }
}