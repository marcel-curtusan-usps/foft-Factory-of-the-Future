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
                    if (AppParameters.AppSettings.ContainsKey("SV_SITE_URL"))
                    {
                        if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("SV_SITE_URL").Value))
                        {
                            Uri parURL = new Uri((string)AppParameters.AppSettings.Property("SV_SITE_URL").Value + site_id_format);
                            string SV_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
                            if (!string.IsNullOrEmpty(SV_Response))
                            {
                                if (AppParameters.IsValidJson(SV_Response))
                                {
                                    JArray sitearray = JArray.Parse(SV_Response);
                                    if (sitearray.HasValues)
                                    {
                                        siteInfo = (JObject)sitearray.Children().FirstOrDefault();
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