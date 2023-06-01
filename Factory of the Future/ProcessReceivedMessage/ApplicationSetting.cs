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
            bool fileUpdate = false;
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    dynamic appSettingObj = JToken.Parse(data);
                    if (((Newtonsoft.Json.Linq.JContainer)appSettingObj).HasValues)
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
                                    AppParameters.AppSettings.FACILITY_ID = SV_Site_Info.FdbId; 
                                    AppParameters.AppSettings.FACILITY_ZIP = SV_Site_Info.ZipCode; 
                                    AppParameters.AppSettings.FACILITY_LKEY = SV_Site_Info.LocaleKey;
                                    Task.Run(() => AppParameters.LoglocationSetup());
                                    FOTFManager.Instance.CoordinateSystem.Clear();
                                    Task.Run(() => AppParameters.ResetParameters()).ConfigureAwait(true);
                                }
                                else
                                {
                                    kv.Value = "";
                                    AppParameters.AppSettings.FACILITY_NAME = "Site Not Found";
                                    AppParameters.AppSettings.FACILITY_ID = "";
                                    AppParameters.AppSettings.FACILITY_ZIP = "";
                                    AppParameters.AppSettings.FACILITY_LKEY = "";
                                }

                            }
                            else if (kv.Name == "LOG_LOCATION")
                            {
                                if (!string.IsNullOrEmpty(kv.Value.ToString()))
                                {
                                    AppParameters.AppSettings.LOG_LOCATION = kv.Value;
                                    Task.Run(() => AppParameters.LoglocationSetup());
                                }
                            }
                            foreach (PropertyInfo prop in AppParameters.AppSettings.GetType().GetProperties())
                            {
                                if (new Regex("^(" + kv.Name + ")$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                {
                                    if (prop.Name.StartsWith("ORACONN"))
                                    {
                                        kv.Value = AppParameters.Encrypt((string)kv.Value);
                                    }
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