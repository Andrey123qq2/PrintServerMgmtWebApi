using System.Management;

namespace PrintServerMgmtWebApi.PrintServer
{
    public class PrintServerManagementScope
    {
        public ManagementScope ManagementScope;
        public PrintServerManagementScope(string computerName = "localhost")
        {
            var wmiConnectionOptions = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Default,
                EnablePrivileges = true // required to load/install the driver.
            };
            // Supposed equivalent to VBScript objWMIService.Security_.Privileges.AddAsString "SeLoadDriverPrivilege", True 
            string path = $"\\\\{computerName}\\root\\cimv2";
            ManagementScope = new ManagementScope(path, wmiConnectionOptions);
        }
    }
}
