# Description
WebAPI for Windows PrintServer management, including swagger

### Available methods
| Method | Description | EventLog ID |
| --------- | ----------- |
|`Get`| gets printer by name | 20 |
|`Delete`| removes printer by name | 30 |
|`Create`| creates printer by properties | 40 |
|`Patch`| changes printer properties and name | 50 |
|`GetQueue`| gets printer queue | 60 |
|`GetQueueCount`| gets printer queue jobs count | 70 |
|`GetDrivers`| gets names of available drivers on print server| 80 |
|`ClearQueue`| clears jobs queue for printer | 90 |
|`RemoveFromACL`| removes account/group from printer's ACL (on W2019 requires admin priveleges and doesn't work) | 100 |
|`AddPrintPermission`| adds print permission by account's/group's SID (on W2019 requires admin priveleges and doesn't work)| 110 |
|`RestartSpooler`| restarts print server spooler service| 120 |

### Logging
Windows log name: PrintServerMgmtWebApi

## Installation
1. Install aspnetcore-runtime-3.X, dotnet-hosting-3.X-win, dotnet-runtime-3.X-win-x64
2. Add Windows IIS role with Windows auth feature
3. Copy files to C:\inetpub\PrintServerMgmtWebApi
4. Create service account and give it admin privileges
5. Create IIS web site PrintServerMgmtWebApi with windows auth enabled and set service account to app pool
6. Create local group PrintServer_Admins and add required users to it