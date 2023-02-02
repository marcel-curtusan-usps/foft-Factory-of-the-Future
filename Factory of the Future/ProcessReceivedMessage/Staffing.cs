using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Factory_of_the_Future
{
    internal class Staffing : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData { get; protected set; }
        public JArray sortplanlist { get; protected set; }
        public IEnumerable<JToken> staff { get; protected set; }
        public bool saveToFile { get; protected set; }
        internal bool Load(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {

                if (data != null)
                {
                    tempData = JToken.Parse(_data);
                    staff = tempData.SelectTokens("$..DATA[*]");
                    sortplanlist = new JArray();
                    if (staff.Any())
                    {
                        foreach (JToken stafff_item in staff)
                        {
                            sortplanlist.Add(new JObject
                            {
                                ["mach_type"] = (string)stafff_item[0],
                                ["machine_no"] = (int)stafff_item[1],
                                ["sortplan"] = (string)stafff_item[2],
                                ["clerk"] = (string)stafff_item[3],
                                ["mh"] = (string)stafff_item[4]
                            });
                        }
                    }

                    if (sortplanlist.HasValues)
                    {
                        foreach (JToken Dataitem in sortplanlist)
                        {
                            if (!string.IsNullOrEmpty((string)Dataitem["sortplan"]))
                            {
                                string mach_type = (string)Dataitem["mach_type"];
                                string machine_no = (string)Dataitem["machine_no"];
                                string sortplan = (string)Dataitem["sortplan"];
                                if (mach_type == "APBS")
                                {
                                    mach_type = "SPBSTS";
                                }

                                sortplan = SortPlan_Name_Trimer(sortplan);
                                Dataitem["mach_type"] = mach_type;
                                Dataitem["sortplan"] = sortplan;
                                string mch_sortplan_id = mach_type + "-" + machine_no + "-" + sortplan;
                                string newtempData = JsonConvert.SerializeObject(Dataitem, Formatting.None);
                                AppParameters.StaffingSortplansList.AddOrUpdate(mch_sortplan_id, newtempData,
                                     (key, existingVal) =>
                                     {
                                         saveToFile = true;
                                         return newtempData;
                                     });
                            }
                        }
                    }

                    if (saveToFile)
                    {
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(AppParameters.StaffingSortplansList.Select(x => x.Value).ToList(), Formatting.Indented));
                    }

                }
                return saveToFile;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return saveToFile;
            }
            finally
            {
                Dispose();
            }
        }
        public string SortPlan_Name_Trimer(string sortplan)
        {
            string sortplan_name = "";
            try
            {
                if (!string.IsNullOrEmpty(sortplan))
                {
                    int dotindex = sortplan.IndexOf(".", 1);
                    if ((dotindex == -1))
                    {
                        sortplan_name = sortplan;
                    }
                    else
                    {
                        sortplan_name = sortplan.Substring(0, dotindex);
                    }
                }
                return sortplan_name;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Staffing()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}