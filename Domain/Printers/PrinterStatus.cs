using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Printers
{
    public class PrinterStatus
    {
        static readonly Dictionary<int, string> _stateMap = new Dictionary<int, string>
        {
            { 1,  "Other"},
            { 2,  "Unknown"},
            { 3,  "Idle"},
            { 4,  "Printing"},
            { 5,  "Warmup"},
            { 6,  "Stopped Printing"},
            { 7,  "Offline"},
        };

        public static string GetStatus(int code)
        {
            if (!_stateMap.TryGetValue(code, out string name))
            {
                // Error handling here
            };
            return name;
        }
    }
}


