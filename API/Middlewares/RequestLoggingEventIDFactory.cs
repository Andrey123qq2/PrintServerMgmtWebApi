using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class RequestLoggingEventIDFactory
    {
        private static readonly Dictionary<string, int> _mapActionToId = new Dictionary<string, int>
            {
                {  "get", 20 },
                {  "delete", 30 },
                {  "create", 40 },
                {  "patch", 50 },
                {  "getqueue", 60 },
                {  "getqueuecount", 70 },
                {  "getdrivers", 80 },
                {  "clearqueue", 90 },
                {  "removefromacl", 100 },
                {  "addprintpermission", 110 },
                {  "restartspooler", 120 },
            };
        public static int Create(string actionName = "")
        {
            if (!_mapActionToId.TryGetValue(actionName, out int code))
                return 10;
            return code;
        }
    }
}
