using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
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
                JObject SessionInfo = new JObject
                {
                    ["IsAuthenticated"] = Session[SessionKey.IsAuthenticated].ToString(),
                    ["UserId"] = Session[SessionKey.AceId].ToString(),
                    ["Role"] = Session[SessionKey.UserRole].ToString(),
                    ["UserFirstName"] = Session[SessionKey.UserFirstName].ToString(),
                    ["UserLastName"] = Session[SessionKey.UserLastName].ToString(),
                    ["Facility_NASS_CODE"] = AppParameters.AppSettings.FACILITY_NASS_CODE,
                    ["Facility_Id"] = AppParameters.AppSettings.FACILITY_ID,
                    ["Facility_Name"] = AppParameters.AppSettings.FACILITY_NAME,
                    ["Environment"] = Session[SessionKey.Environment].ToString(),
                    ["FacilityTimeZone"] = AppParameters.AppSettings.FACILITY_TIMEZONE,
                    ["SoftwareVersion"] = Session[SessionKey.SoftwareVersion].ToString(),
                    ["ApplicationFullName"] = Session[SessionKey.ApplicationFullName].ToString(),
                    ["ApplicationAbbr"] = Session[SessionKey.ApplicationAbbr].ToString(),
                    ["ConnectionList"] = JsonConvert.SerializeObject(AppParameters.RunningConnection.Connection.Select(y => y.ConnectionInfo).ToList(), Formatting.Indented),
                    ["AppSetting"] = JsonConvert.SerializeObject(AppParameters.AppSettings, Formatting.Indented)

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
            //
        }
    }
}