using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class AppSetting
    {
        public string APPLICATION_NAME { get; set; } = "";
        public string APPLICATION_FULLNAME { get; set; } = "";
        public string FACILITY_NAME { get; set; } = "";
        public string FACILITY_ZIP { get; set; } = "";
        public string FACILITY_ID { get; set; } = "";
        public string FACILITY_NASS_CODE { get; set; }  
        public string FACILITY_LKEY { get; set; } = "";
        public string FACILITY_P2P_SITEID { get; set; } = "";
        public string FACILITY_TIMEZONE { get; set; }
        public bool LOG_API_DATA { get; set; } = false;
        public string RETENTION_DAYS { get; set; } = "";
        public string RETENTION_MAX_FILE_SIZE { get; set; } = "";
        public bool? SERVER_ACTIVE { get; set; }    
        public string SERVER_ACTIVE_HOSTNAME { get; set; } = "";
        public bool? LOCAL_PROJECT_DATA { get; set; }
        public string DOCKDOOR_ZONE { get; set; } = "";
        public string AGV_ZONE { get; set; } = "";
        public string MANUAL_ZONE { get; set; } = "";
        public string TAG_PERSON { get; set; } = "";
        public string TAG_AGV { get; set; } = "";
        public string TAG_HVI { get; set; } = "";
        public string TAG_LOCATOR { get; set; } = "";
        public string VIEWPORTS { get; set; } = "";
        public string TAG_PIV { get; set; } = "";
        public string TAG_TIMEOUTMILS { get; set; } = "";
        public string Domain { get; set; } = "";
        public string SV_SITE_URL { get; set; } = "";
        public string SV_ITINERARY { get; set; } = "";
        public string SV_CODETYPE { get; set; } = "";
        public string SV_TRIPCONTAINER { get; set; } = "";
        public string ADUSAContainer { get; set; } = "";
        public string API_KEY { get; set; } = "";
        public string LOG_LOCATION { get; set; } = "";
        public string ADMINOVERRIDEUSER { get; set; } = "";
        public string DEV_SVRP_IP { get; set; } = "";
        public string DEV_SVRS_IP { get; set; } = "";
        public string SIT_SVRP_IP { get; set; } = "";
        public string SIT_SVRS_IP { get; set; } = "";
        public string CAT_SVRP_IP { get; set; } = "";
        public string CAT_SVRS_IP { get; set; } = "";
        public bool? REMOTEDB { get; set; } 
        public string ROLES_ADMIN { get; set; } = "";
        public string ROLES_OPERATOR { get; set; } = "";
        public string ROLES_MAINTENANCE { get; set; } = "";
        public string ROLES_OIE { get; set; } = "";
        public string ORACONNSTRING { get; set; } = "";
        public string ORACONNASSTRING { get; set; } = "";
        public string ORACONNSVSTRING { get; set; } = "";
        public string MPE_WATCH_ID { get; set; } = "";
        public double POSITION_MAX_AGE { get; set; } = 0.0;
        public string CONNECTIONNAME { get; set; } = "";
        public string SV_TRIP_STATUS { get; set; } = "";

        public AppSetting ShallowCopy()
        {
            return (AppSetting) this.MemberwiseClone();
        }
    }
}