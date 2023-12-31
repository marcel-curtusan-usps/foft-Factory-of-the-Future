﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Factory_of_the_Future
{
    internal class GetData
    {
        internal bool Get_Site_Info(string siteId, out SV_Site_Info siteInfo)
        {
            try
            {
                string site_id_format = Regex.Replace(siteId.Trim(), @"--", "").Trim();
                if (!string.IsNullOrEmpty(site_id_format))
                {
                    siteInfo = null;

                    if (!string.IsNullOrEmpty(AppParameters.AppSettings.SV_SITE_URL))
                    {
                        Uri parURL = new Uri(AppParameters.AppSettings.SV_SITE_URL + site_id_format);
                        string SV_Response = new SendMessage().Get(parURL, new JObject());
                        if (!string.IsNullOrEmpty(SV_Response))
                        {

                            siteInfo = JsonConvert.DeserializeObject<List<SV_Site_Info>>(SV_Response).FirstOrDefault();
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
                    siteInfo = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                siteInfo = null;
                return false;
            }
        }
    }
}