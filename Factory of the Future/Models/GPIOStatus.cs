using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class GPIOStatus
    {
        public GPIOStatus(string machineId, int gpioStatus)
        {
            MachineId = machineId;
            GpioStatus = gpioStatus;
        }
        public string MachineId { get; set; }
        public int GpioStatus { get; set; }
    }
}