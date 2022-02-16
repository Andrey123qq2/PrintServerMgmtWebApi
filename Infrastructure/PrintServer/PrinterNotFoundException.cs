using System;

namespace Infrastructure.PrintServer
{
    public class PrinterNotFoundException : Exception
    {
        public PrinterNotFoundException()
        {
        }
        public PrinterNotFoundException(string message)
            : base(message)
        {
        }
        public PrinterNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
