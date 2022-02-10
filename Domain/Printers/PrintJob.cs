using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Printers
{
    public class PrintJob
    {
        public string Document { get; set; }
        public string TimeSubmitted { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }
        public string TotalPages { get; set; }
        public string Size { get; set; }
    }
}

//'Caption','Color','DataType','Description','Document','DriverName','ElapsedTime',
//'HostPrintQueue','InstallDate','JobId','JobStatus','Name','Notify','Owner','PagesPrinted','PaperLength',
//'PaperSize','PaperWidth','Parameters','PrintProcessor','Priority','Size','StartTime','Status',
//'StatusMask','TimeSubmitted','TotalPages','UntilTime'

//StatusMask

//Bitmap of the possible statuses that relate to this print job.

//1 (0x1)

//Paused

//2(0x2)

//Error

//4(0x4)

//Deleting

//8(0x8)

//Spooling

//16(0x10)

//Printing

//32(0x20)

//Offline

//64(0x40)

//Paperout

//128(0x80)

//Printed

//256(0x100)

//Deleted

//512(0x200)

//Blocked_DevQ

//1024(0x400)

//User_Intervention_Req

//2048(0x800)

//Restart