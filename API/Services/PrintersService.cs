using API.ApiModels.Printers;
using Domain.Converters;
using Domain.Interfaces;
using Domain.Printers;
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
            ManagementObject printerManagementObject = _repository.Get(model.Name);
            GetPrinterResponse response = ManagementObjectConverter.Convert<GetPrinterResponse>(printerManagementObject);
            response.PrinterStatus = PrinterStatus.GetStatus(int.Parse(response.PrinterStatus));
            return response;
        }

        public CreatePrinterResponse Create(CreatePrinterRequest model)
        {
            _repository.Create(model.Name, model.ShareName, model.DriverName, model.Location);
            var response = new CreatePrinterResponse()
            {
                Name = model.Name
            };
            return response;
        }

        public DeletePrinterResponse Delete(DeletePrinterRequest model)
        {
            _repository.Delete(model.Name);
            var response = new DeletePrinterResponse()
            {
                Name = model.Name
            };
            return response;
        }

        public UpdatePrinterResponse Update(string printerName, UpdatePrinterRequest model)
        {
            ManagementObject printerManagementObject = _repository.Get(printerName);
            UpdatePrinterName(printerManagementObject, model.Name);
            UpdatePrinterProperties(printerManagementObject, model);

            var response = new UpdatePrinterResponse()
            {
                Name = model.Name
            };
            return response;
        }

        public RemoveFromPrinterACLResponse RemoveFromACL(RemoveFromPrinterACLRequest model)
        {
            uint result = _repository.RemoveFromACL(model.Name, model.EntityName);

            var response = new RemoveFromPrinterACLResponse()
            {
                Description = ChangeDACLResult.GetDescription(result)
            };
            return response;
        }

        public AddPrintPermissionResponse AddPrintPermission(AddPrintPermissionRequest model)
        {
            uint result = _repository.AddPrintPermission(model.Name, model.Sid);

            var response = new AddPrintPermissionResponse()
            {
                Description = ChangeDACLResult.GetDescription(result)
            };
            return response;
        }

        public GetPrinterQueueResponse GetQueue(GetPrinterQueueRequest model)
        {
            ManagementObjectCollection printerQueue = _repository.GetPrinterQueue(model.Name);
            List<PrintJob> printJobs = printerQueue
                .Cast<ManagementObject>()
                .ToList()
                .Select(m => ManagementObjectConverter.Convert<PrintJob>(m))
                .ToList();

            var response = new GetPrinterQueueResponse()
            {
                PrintJobsCollection = printJobs
            };
            return response;
        }

        public GetDriversResponse GetDrivers()
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
            return response;
        }

        public ClearPrinterQueueResponse ClearQueue(ClearPrinterQueueRequest model)
        {
            _repository.ClearPrinterQueue(model.Name);

            var response = new ClearPrinterQueueResponse()
            {
                Success = true
            };
            return response;
        }

        private void UpdatePrinterName(ManagementObject printerManagementObject, string nameInModel)
        {
            string currentPrinterName = printerManagementObject.Properties["Name"].Value.ToString();
            if (currentPrinterName != nameInModel)
                _repository.RenamePrinter(printerManagementObject, nameInModel);
        }

        private void UpdatePrinterProperties(ManagementObject printerManagementObject, UpdatePrinterRequest model)
        {
            Type modelType = typeof(UpdatePrinterRequest);
            modelType.GetProperties()
                .Where(p => p.Name != "Name")
                .ToList()
                .ForEach(p =>
                {
                    string propName = p.Name;
                    string printerCurrentValue = printerManagementObject.Properties[propName].Value.ToString();
                    string printerNewValue = modelType.GetProperty(propName).GetValue(model)?.ToString();
                    if (printerNewValue != null && printerCurrentValue != printerNewValue)
                        _repository.ChangeProperty(printerManagementObject, propName, printerNewValue);
                });
        }
    }
}
