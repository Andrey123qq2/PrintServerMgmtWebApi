using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.PrintServer
{
    public class PrinterCreateConflictException : Exception
    {
        public PrinterCreateConflictException()
        {
        }
        public PrinterCreateConflictException(string message)
            : base(message)
        {
        }
        public PrinterCreateConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}