using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Factory_of_the_Future
{
    internal class GetData
    {
        internal static bool Get_Site_Info(string siteId, out JObject siteInfo)
        {
            try
            {
                string site_id_format = Regex.Replace(siteId.Trim(), @"--", "").Trim();
                if (!string.IsNullOrEmpty(site_id_format))
                {
                    siteInfo = new JObject();
                    if (Global.AppSettings.ContainsKey("SV_SITE_URL"))
                    {
                        if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("SV_SITE_URL").Value))
                        {
                            Uri parURL = new Uri((string)Global.AppSettings.Property("SV_SITE_URL").Value + site_id_format);
                            string SV_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
                            if (!string.IsNullOrEmpty(SV_Response))
                            {
                                if (Global.IsValidJson(SV_Response))
                                {
                                    JArray sitearray = JArray.Parse(SV_Response);
                                    if (sitearray.HasValues)
                                    {
                                        siteInfo = (JObject)sitearray.Children().FirstOrDefault();
                                        if (siteInfo.HasValues)
                                        {
                                            if (!Global.Site_Info_List.ContainsKey((string)siteInfo.Property("siteId").Value))
                                            {
                                                Global.Site_Info_List.TryAdd((string)siteInfo.Property("siteId").Value, siteInfo);
                                            }
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    siteInfo = new JObject();
                    return false;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                siteInfo = new JObject();
                return false;
            }
        }
    }
}