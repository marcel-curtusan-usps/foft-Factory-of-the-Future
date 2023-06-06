using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class MPEGroupList
    {
        public MPEGroupList(string mpeName, string status, int operationNumber)
        {
            MPEName = mpeName;
            Status = status;
            OperationNumber = operationNumber;
        }
        public string MPEName { get; set; }
        public string Status { get; set; }
        public int OperationNumber { get; set; }
    }
}