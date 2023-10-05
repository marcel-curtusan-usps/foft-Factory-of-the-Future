using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Factory_of_the_Future.Models;

namespace Factory_of_the_Future
{
    internal class SiteInfo
    {
        public string Data { get; set; }

        internal object Edit(string data)
        {
            Data = data;
            bool NewValue = false;
            bool SiteFileUpdate = false;

            try
            {
                if (!string.IsNullOrEmpty(Data))
                {
                    dynamic appSettingObj = JToken.Parse(Data);
                    if (((JContainer)appSettingObj).HasValues)
                    {
                        foreach (var kv in appSettingObj)
                        {
                            if (kv.Name == "siteId")
                            {
                                SV_Site_Info SV_Site_Info = new Get_Site_Info().Get_Info((string)kv.Value);
                                if (SV_Site_Info != null)
                                {
                                    AppParameters.SiteInfo = SV_Site_Info;
                                    Task.Run(() => AppParameters.LoglocationSetup()).ConfigureAwait(false);
                                    Task.Run(() => AppParameters.ResetParameters()).ConfigureAwait(false);
                                    SiteFileUpdate = true;
                                }
                                else
                                {
                                    kv.Value = "";
                                    AppParameters.SiteInfo = new SV_Site_Info();
                                    SiteFileUpdate = true;
                                }

                            }
                            else 
                            {
                                foreach (PropertyInfo prop in AppParameters.SiteInfo.GetType().GetProperties())
                                {
                                    if (new Regex("^(" + kv.Name + ")$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                    {
                                        if (prop.GetValue(AppParameters.SiteInfo, null).ToString() != (string)kv.Value)
                                        {
                                            prop.SetValue(AppParameters.SiteInfo, (string)kv.Value);
                                            SiteFileUpdate = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (SiteFileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "SiteInformation.json", JsonConvert.SerializeObject(AppParameters.AppSettings, Formatting.Indented));
                }
                return GetSiteInfo();
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
        internal object GetSiteInfo()
        {
            return AppParameters.SiteInfo;
        }
    }
}