using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class MPEGroupsNames
    {
        public MPEGroupsNames(List<string> mpeNames, List<string> groupNames)
        {
            MPENames = mpeNames;
            GroupNames = groupNames;
        }
        public IEnumerable<string> MPENames { get; set; }
        public IEnumerable<string> GroupNames { get; set; }
    }
}