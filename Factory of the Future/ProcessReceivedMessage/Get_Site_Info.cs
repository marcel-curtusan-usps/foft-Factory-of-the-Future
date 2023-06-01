using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Linq;

namespace Factory_of_the_Future
{
    internal class Get_Site_Info
    {
        private string siteId; 
        private List<SV_Site_Info> siteInfo = new List<SV_Site_Info>();
        public SV_Site_Info Get_Info(string NASSCode)
        {
            this.siteId = NASSCode;
            string site_id_format = Regex.Replace(siteId.Trim(), @"--", "").Trim();
            try
            {
                if (!string.IsNullOrEmpty(site_id_format))
                {
                    siteInfo = null;

                    if (!string.IsNullOrEmpty(AppParameters.AppSettings.SV_SITE_URL))
                    {
                        Uri parURL = new Uri(AppParameters.AppSettings.SV_SITE_URL + site_id_format);
                        string SV_Response = new SendMessage().Get(parURL, new JObject());
                        if (!string.IsNullOrEmpty(SV_Response))
                        {

                            siteInfo = JsonConvert.DeserializeObject<List<SV_Site_Info>>(SV_Response);
                            return siteInfo.FirstOrDefault();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
    }
}