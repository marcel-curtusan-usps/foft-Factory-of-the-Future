using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Factory_of_the_Future.AGV.Models
{
    public class EITS_Messages
    {
  
        public class MessageResponse
        {
            public string OBJECT_TYPE { get; set; } = "COMMAND_RESPONSE";
            public string ACTION { get; set; } = "";
            public string REQUESTID { get; set; } = "";
            public string RESPONSE_CODE { get; set; } = "";
            public string RESPONSE_MSG { get; set; } = "";
            public MessageResponse(CancelMission cancelMission)
            {
                ACTION = cancelMission.ACTION;
                REQUESTID = cancelMission.REQUESTID;
                RESPONSE_CODE = new ProcessMessage().CancelRequet(cancelMission);
                RESPONSE_MSG = "0";
            }

            public MessageResponse(RequestMission requestMission)
            {
                ACTION = requestMission.ACTION;
                RESPONSE_CODE = "0";
                RESPONSE_MSG = "0";
            }

            public MessageResponse(LocationStatus locationStatus)
            {
                ACTION = locationStatus.ACTION;
                RESPONSE_CODE = new ProcessMessage().LocationStatus(locationStatus); 
                RESPONSE_MSG = "0";
            }
        }
        public class HeartbeatResponse
        {
            public string OBJECT_TYPE { get; set; } = "STATUS_UPDATE";
            public string ACTION { get; set; } = "HEARTBEATSTATUS";
            public string SERVER_IP { get; set; } = AppParameters.ServerIpAddress;
            public DateTime SERVER_DATE_TIME { get; set; } = DateTime.Now;
            public int STATUS_CODE { get; set; } = Check_AGV_Connection();

            private static int Check_AGV_Connection()
            {
                try
                {
                    //if (AppParameters.ConnectionList.Count > 0)
                    //{
                    //    return 0;
                    //}
                    //else
                    //{
                        return -1;
                   // }
                }
                catch (Exception)
                {

                    return -1;
                }
            }
        }
        public class LocationStatus
        {
            public string OBJECT_TYPE { get; set; } = "";
            public string ACTION { get; set; } = "";
            public string CLIENT_IP { get; set; } = "";
            public string USERNAME { get; set; } = "";
            public string VENDOR_NAME { get; set; } = "";
            public string LOCATION { get; set; } = "";
            public string POSITION { get; set; } = "";
            public string STATUS { get; set; } = "";
        }
        public class RequestMission
        {
            public string OBJECT_TYPE { get; set; } = "COMMAND_REQUEST";
            public string ACTION { get; set; } = "MOVEREQUEST";
            public string CLIENT_IP { get; set; } = "";
            public string USERNAME { get; set; } = "";
            public string VENDOR_NAME { get; set; } = "";
            public string LOAD_TYPE { get; set; } = "";
            public string NASS_CODE { get; set; } = "";
            public string POSITION { get; set; } = "";
            public string CONTAINER_COUNT { get; set; } = "";
            public string PICKNAME { get; set; } = "";
            public string DROPNAME { get; set; } = "";
            public string ENDNAME { get; set; } = "";
        }
        public class CancelMission
        {
            public string OBJECT_TYPE { get; set; } = "";
            public string ACTION { get; set; } = "";
            public string VENDOR_NAME { get; set; } = "";
            public string USERNAME { get; set; } = "";
            public string CLIENT_IP { get; set; } = "";
            public string REQUESTID { get; set; } = "";
        }
        public class HeartbeatRequest
        {
            public string OBJECT_TYPE { get; set; } = "";
            public string ACTION { get; set; } = "";
            public string CLIENT_IP { get; set; } = "";
            public string USERNAME { get; set; } = "";
            public string VENDOR_NAME { get; set; } = "";
        }
    }
}