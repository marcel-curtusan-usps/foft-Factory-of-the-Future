using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class UpdateParam
    {
        public string Version { get; set; } = "";
        public string AppName { get; set; } = "CF";
        public string DrivePath { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Token { get; set; } = "";
    }
}