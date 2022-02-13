using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Printers
{
    public class WmiInvokeResult
    {
        static readonly Dictionary<uint, string> _resultMap = new Dictionary<uint, string>
        {
            { 0,  "Successful completion"},
            { 2,  "The user does not have access to the requested information"},
            { 5,  "Access Denied"},            
            { 1801,  "Invalid Printer Name"},
            { 10000,  "No such entity"},
            { 10001,  "Entity already in the desired state"},
        };

        public static string GetDescription(uint code)
        {
            if (!_resultMap.TryGetValue(code, out string name))
            {
                // Error handling here
            };
            return name;
        }
    }
}
