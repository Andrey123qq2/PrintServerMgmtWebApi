using API.ApiModels.Printers;
using Domain.Converters;
using Domain.Interfaces;
using Domain.Printers;
using Infrastructure.WindowsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace API.Services
{
    public class PrintersService
    {
        private readonly IPrintersRepository _repository;

        public PrintersService(IPrintersRepository repository)
        {
            _repository = repository;
        }

        public GetPrinterResponse Get(GetPrinterRequest model)
        {
            ManagementObject printerManagementObject = _repository.Get(model.Name, model.PropertyFilter);
            GetPrinterResponse response = ManagementObjectConverter.Convert<GetPrinterResponse>(printerManagementObject);
            response.PrinterStatus = PrinterStatus.GetStatus(int.Parse(response.PrinterStatus));
            return response;
        }

        public GeneralResponse Create(CreatePrinterRequest model)
        {
            ManagementPath result = _repository
                .Create(model.Name, model.ShareName, model.PrinterHostName, model.DriverName, model.Location, model.PropertyFilter, model.Comment);
            var response = new GeneralResponse()
            {
                Description = result.ToString()
            };
            return response;
        }

        public DeletePrinterResponse Delete(DeletePrinterRequest model)
        {
            _repository.Delete(model.Name);
            var response = new DeletePrinterResponse()
            {
                Description = "Successful completion"
            };
            return response;
        }

        public UpdatePrinterResponse Update(string printerName, string propertyFilter, UpdatePrinterRequest model)
        {
            var response = new UpdatePrinterResponse
            {
                UpdateResultsCollection = new List<string>()
            };
            ManagementObject printerManagementObject = _repository.Get(printerName, propertyFilter);
            if (model.Name != null)
            {
                uint resultRename = UpdatePrinterName(printerManagementObject, model.Name);
                string resultRenameString = WmiInvokeResult.GetDescription(resultRename);
                response.Description = resultRenameString;
                response.UpdateResultsCollection.Add(resultRenameString);
                if (resultRename == 5 || resultRename == 2)
                    throw new UnauthorizedAccessException(resultRenameString);
                printerManagementObject = _repository.Get(printerName, propertyFilter);
            };

            List<string> resultUpdateProperties = UpdatePrinterProperties(printerManagementObject, model);
            response.UpdateResultsCollection.AddRange(resultUpdateProperties);
            if (resultUpdateProperties.Count > 0)
                response.Description = resultUpdateProperties.FirstOrDefault();

            return response;
        }

        public RemoveFromPrinterACLResponse RemoveFromACL(RemoveFromPrinterACLRequest model)
        {
            uint result = _repository.RemoveFromACL(model.Name, model.EntityName, model.PropertyFilter);

            var response = new RemoveFromPrinterACLResponse()
            {
                Description = ChangeDACLResult.GetDescription(result)
            };
            return response;
        }

        public AddPrintPermissionResponse AddPrintPermission(AddPrintPermissionRequest model)
        {
            uint result = _repository.AddPrintPermission(model.Name, model.Sid, model.PropertyFilter);

            var response = new AddPrintPermissionResponse()
            {
                Description = ChangeDACLResult.GetDescription(result)
            };
            return response;
        }

        public GetPrinterQueueResponse GetQueue(GetPrinterQueueRequest model)
        {
            List<PrintJob> printJobs = GetPrinterQueue(model.PrinterHostName, model.Name,  model.PropertyFilter);
            var response = new GetPrinterQueueResponse()
            {
                PrintJobsCollection = printJobs
            };
            return response;
        }

        public GetPrinterQueueCountResponse GetQueueCount(GetPrinterQueueCountRequest model)
        {
            List<PrintJob> printJobs = GetPrinterQueue(model.PrinterHostName, model.Name, model.PropertyFilter);
            var response = new GetPrinterQueueCountResponse()
            {
                Count = printJobs.Count
            };
            return response;
        }

        public GeneralResponse RestartSpooler()
        {
            var serviceController = new WindowsServiceController("spooler");
            serviceController.RestartService();
            return new GeneralResponse { Description = "Spooler has been restarted" };
        }

        private List<PrintJob> GetPrinterQueue(string printerHostName, string printerName, string propertyFilter)
        {
            ManagementObjectCollection printerQueue = _repository.GetPrinterQueue(printerHostName, printerName, propertyFilter);
            List<PrintJob> printJobs = printerQueue
                .Cast<ManagementObject>()
                .ToList()
                .Select(m => ManagementObjectConverter.Convert<PrintJob>(m))
                .ToList();
            return printJobs;
        }

        public IEnumerable<PrinterDriver> GetDrivers()
        {
            ManagementObjectCollection driversCollection = _repository.GetDrivers();
            List<PrinterDriver> drivers = driversCollection
                .Cast<ManagementObject>()
                .ToList()
                .Select(m =>
                {
                    var driver = ManagementObjectConverter.Convert<PrinterDriver>(m);
                    driver.Name = driver.Name.Split(",")[0];
                    return driver;
                })
                .ToList();

            var response = new GetDriversResponse()
            {
                DriversCollection = drivers
            };
            return response.DriversCollection;
        }

        public ClearPrinterQueueResponse ClearQueue(ClearPrinterQueueRequest model)
        {
            _repository.ClearPrinterQueue(model.Name, model.PropertyFilter);

            var response = new ClearPrinterQueueResponse()
            {
                Success = true
            };
            return response;
        }

        private uint UpdatePrinterName(ManagementObject printerManagementObject, string newName)
        {
            uint result;
            string currentPrinterName = printerManagementObject.Properties["Name"].Value.ToString();
            if (currentPrinterName != newName)
                result = _repository.RenamePrinter(printerManagementObject, newName);
            else
                result = 10001u;
            return result;
        }

        private List<string> UpdatePrinterProperties(ManagementObject printerManagementObject, UpdatePrinterRequest model)
        {
            Type modelType = typeof(UpdatePrinterRequest);
            List<string> results = modelType.GetProperties()
                .Where(p => p.Name != "Name")
                .ToList()
                .Select(p =>
                {
                    ManagementPath result = new ManagementPath();
                    string propName = p.Name;
                    string printerCurrentValue = printerManagementObject.Properties[propName].Value.ToString();
                    string printerNewValue = modelType.GetProperty(propName).GetValue(model)?.ToString();
                    if (printerNewValue != null && printerCurrentValue != printerNewValue)
                        result = _repository.ChangeProperty(printerManagementObject, propName, printerNewValue);
                    return result.ToString();
                })
                .Where(r => !string.IsNullOrEmpty(r))
                .ToList();
            return results;
        }
    }
}
