﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Factory_of_the_Future
{
    public partial class Main : System.Web.UI.Page
    {
        public string Session_Info { get { return GetSessionInfo(); } }

        private string GetSessionInfo()
        {
            try
            {
                JObject SessionInfo = new JObject {
                    ["IsAuthenticated"] = Session[SessionKey.IsAuthenticated].ToString(),
                    ["UserId"] = Session[SessionKey.AceId].ToString(),
                    ["Role"] = Session[SessionKey.UserRole].ToString(),
                    ["UserFirstName"] = Session[SessionKey.UserFirstName].ToString(),
                    ["UserLastName"] = Session[SessionKey.UserLastName].ToString(),
                    ["Facility_NASS_CODE"] = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(),
                    ["Facility_Id"] = AppParameters.AppSettings["FACILITY_ID"].ToString(),
                    ["Facility_Name"] = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                    ["Environment"] = Session[SessionKey.Environment].ToString(),
                    ["FacilityTimeZone"] = AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString(),
                    ["SoftwareVersion"] = Session[SessionKey.SoftwareVersion].ToString(),
                    ["ApplicationFullName"] = Session[SessionKey.ApplicationFullName].ToString(),
                    ["ApplicationAbbr"] = Session[SessionKey.ApplicationAbbr].ToString(),
                    ["ConnectionList"] = JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented)

            };
                return JsonConvert.SerializeObject(SessionInfo, Formatting.Indented);

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}