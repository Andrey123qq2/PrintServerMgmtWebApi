using System.Collections.Generic;

namespace Domain.Printers
{
    public class ChangeDACLResult
    {
        static readonly Dictionary<uint, string> _resultMap = new Dictionary<uint, string>
        {
            { 0,  "Successful completion"},
            { 2,  "The user does not have access to the requested information"},
            { 8,  "Unknown failure"},
            { 9,  "The user does not have adequate privileges to execute the method"},
            { 21,  "A parameter specified in the method call is not valid"},
            { 10000,  "No such entity"},
            { 10001,  "Entity already exists"},
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
