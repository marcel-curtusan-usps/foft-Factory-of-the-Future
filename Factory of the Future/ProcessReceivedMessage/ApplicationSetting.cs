using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class ApplicationSetting 
    {
        public string Data { get; set; }
        internal AppSetting EditAppSetting(string data)
        {
            Data = data;
            bool NewValue = false;
            bool fileUpdate = false;
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    dynamic appSettingObj = JToken.Parse(data);
                    if (((JContainer)appSettingObj).HasValues)
                    {
                        foreach (var kv in appSettingObj)
                        {
                            if (kv.Name == "FACILITY_NASS_CODE")
                            {
                                SV_Site_Info SV_Site_Info = new Get_Site_Info().Get_Info((string)kv.Value);
                                if (SV_Site_Info != null)
                                {
                                    kv.Value = SV_Site_Info.SiteId;
                                    AppParameters.AppSettings.FACILITY_NAME = SV_Site_Info.DisplayName;
                                    AppParameters.AppSettings.FACILITY_ID = !string.IsNullOrEmpty(SV_Site_Info.FdbId) ? SV_Site_Info.FdbId : ""; 
                                    AppParameters.AppSettings.FACILITY_ZIP = !string.IsNullOrEmpty(SV_Site_Info.ZipCode) ? SV_Site_Info.ZipCode : ""; 
                                    AppParameters.AppSettings.FACILITY_LKEY = !string.IsNullOrEmpty(SV_Site_Info.ZipCode) ? SV_Site_Info.ZipCode : "";
                                    Task.Run(() => AppParameters.LoglocationSetup()).ConfigureAwait(false);
                                    
                                    Task.Run(() => AppParameters.ResetParameters()).ConfigureAwait(false);
                                    fileUpdate = true;
                                }
                                else
                                {
                                    kv.Value = "";
                                    AppParameters.AppSettings.FACILITY_NAME = "Site Not Found";
                                    AppParameters.AppSettings.FACILITY_ID = "";
                                    AppParameters.AppSettings.FACILITY_ZIP = "";
                                    AppParameters.AppSettings.FACILITY_LKEY = "";
                                    fileUpdate = true;
                                }

                            }
                            else if (kv.Name == "LOG_LOCATION")
                            {
                                if (!string.IsNullOrEmpty(kv.Value.ToString()))
                                {
                                    AppParameters.AppSettings.LOG_LOCATION = kv.Value;
                                    Task.Run(() => AppParameters.LoglocationSetup());
                                    fileUpdate = true;
                                }
                            }
                            if (!new Regex("^(LOG_LOCATION|FACILITY_NASS_CODE)$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2)).IsMatch(kv.Name))
                            {
                                foreach (PropertyInfo prop in AppParameters.AppSettings.GetType().GetProperties())
                                {
                                    if (new Regex("^(" + kv.Name + ")$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                    {
                                        if (prop.Name.StartsWith("ORACONN"))
                                        {
                                            kv.Value = AppParameters.Encrypt((string)kv.Value);
                                        }
                                        if (prop.Name.StartsWith("LOG_API_DATA") || prop.Name.StartsWith("LOCAL_PROJECT_DATA") || prop.Name.StartsWith("REMOTEDB") || prop.Name.StartsWith("SERVER_ACTIVE"))
                                        {
                                            if (kv.Value.ToString() == "True")
                                            {
                                                NewValue = true;
                                            }
                                            else
                                            {
                                                NewValue = false;
                                            }
                                            if (prop.GetValue(AppParameters.AppSettings, null).ToString() != NewValue.ToString())
                                            {
                                                prop.SetValue(AppParameters.AppSettings, NewValue);
                                                fileUpdate = true;
                                            }
                                        }
                                        else
                                        {
                                            if (prop.GetValue(AppParameters.AppSettings, null).ToString() != (string)kv.Value)
                                            {
                                                prop.SetValue(AppParameters.AppSettings, (string)kv.Value);
                                                fileUpdate = true;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "AppSettings.json", JsonConvert.SerializeObject(AppParameters.AppSettings, Formatting.Indented));
                }
                return GetAppSetting();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
            finally
            {
                Data = null;
            }
        }
        internal AppSetting GetAppSetting()
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
                return TempAppSettings;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return TempAppSettings;
            }
            finally
            {
                TempAppSettings = null;
            }
        }
    }
}