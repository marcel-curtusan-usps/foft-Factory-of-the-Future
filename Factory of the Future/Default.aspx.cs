using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
                    ["Facility_NASS_CODE"] = Session[SessionKey.Facility_NASS_CODE].ToString(),
                    ["Facility_Id"] = Session[SessionKey.Facility_Id].ToString(),
                    ["Facility_Name"] = Session[SessionKey.Facility_Name].ToString(),
                    ["Environment"] = Session[SessionKey.Environment].ToString(),
                    ["FacilityTimeZone"] = AppParameters.AppSettings.FACILITY_TIMEZONE,
                    ["SoftwareVersion"] = Session[SessionKey.SoftwareVersion].ToString(),
                    ["ApplicationFullName"] = Session[SessionKey.ApplicationFullName].ToString(),
                    ["ApplicationAbbr"] = Session[SessionKey.ApplicationAbbr].ToString(),
                    ["ConnectionList"] = JsonConvert.SerializeObject(AppParameters.RunningConnection.DataConnection.Select(y => y.ConnectionInfo).ToList(), Formatting.Indented),
                    ["AppSetting"] = GetAppSetting()

                };
                return JsonConvert.SerializeObject(SessionInfo, Formatting.Indented);

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

        private string GetAppSetting()
        {
            AppSetting TempAppSettings = AppParameters.AppSettings.ShallowCopy();
            try
            {
                foreach (PropertyInfo prop in TempAppSettings.GetType().GetProperties())
                {
                    if (new Regex("^(ORACONN)", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                    {
                        prop.SetValue(TempAppSettings, AppParameters.Decrypt(prop.GetValue(AppParameters.AppSettings, null).ToString()));
                    }
                }
                return JsonConvert.SerializeObject(TempAppSettings);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return JsonConvert.SerializeObject(TempAppSettings);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //
        }
    }
}