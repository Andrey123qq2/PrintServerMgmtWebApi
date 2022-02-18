using System;
using System.ServiceProcess;

namespace Infrastructure.WindowsServices
{
    public class WindowsServiceController
    {
        private string _serviceName;

        public WindowsServiceController(string _serviceName)
        {
            this._serviceName = _serviceName;
        }

        public void RestartService()
        {
            using (ServiceController service = new ServiceController(_serviceName))
            {
                try
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Can not restart the Windows Service {_serviceName}", ex);
                }
            }
        }

        public void StopService()
        {
            using (ServiceController service = new ServiceController(_serviceName))
            {
                try
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Can not Stop the Windows Service [{_serviceName}]", ex);
                }
            }
        }

        public void StartService()
        {
            using (ServiceController service = new ServiceController(_serviceName))
            {
                try
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Can not Start the Windows Service [{_serviceName}]", ex);
                }
            }
        }

        public void StartOrRestart()
        {
            if (IsRunningStatus)
                RestartService();
            else if (IsStoppedStatus)
                StartService();
        }

        public void StopServiceIfRunning()
        {
            using (ServiceController service = new ServiceController(_serviceName))
            {
                try
                {
                    if (!IsRunningStatus)
                        return;

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Can not Stop the Windows Service [{_serviceName}]", ex);
                }
            }
        }

        public bool IsRunningStatus => Status == ServiceControllerStatus.Running;

        public bool IsStoppedStatus => Status == ServiceControllerStatus.Stopped;

        public ServiceControllerStatus Status
        {
            get
            {
                using (ServiceController service = new ServiceController(_serviceName))
                {
                    return service.Status;
                }
            }
        }
    }
}