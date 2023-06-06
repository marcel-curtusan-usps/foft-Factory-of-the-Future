using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class StaffingData : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public List<Staff> NewStaffData = null;
        public List<Staff> StaffData = null;
        public Staff currenStaffInfo = null;
        private bool saveToFile;
        internal async Task<bool> LoadAsync(string data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    JToken tempData = JToken.Parse(_data);
                    IEnumerable<JToken> staff = tempData.SelectTokens("$..DATA[*]");
                    NewStaffData = GetStaffList(staff);
                    foreach (Staff NewStaffInfo in NewStaffData)
                    {
                        if (AppParameters.StaffingSortplansList.ContainsKey(NewStaffInfo.Id)
                            && AppParameters.StaffingSortplansList.TryGetValue(NewStaffInfo.Id, out currenStaffInfo))
                        {
                            foreach (PropertyInfo prop in currenStaffInfo.GetType().GetProperties())
                            {

                                if (!new Regex("^(Id)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                {
                                    if (prop.GetValue(NewStaffInfo, null).ToString() != prop.GetValue(currenStaffInfo, null).ToString())
                                    {
                                        saveToFile = true;
                                        prop.SetValue(currenStaffInfo, prop.GetValue(NewStaffInfo, null));

                                    }
                                }
                            }

                        }
                        else
                        {
                            if (AppParameters.StaffingSortplansList.TryAdd(NewStaffInfo.Id, NewStaffInfo))
                            {
                                //
                            }
                        }
                    }


                }
                if (saveToFile)
                {
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(AppParameters.StaffingSortplansList.Select(x => x.Value).ToList(), Formatting.Indented));
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

        private List<Staff> GetStaffList(IEnumerable<JToken> staff)
        {
            StaffData = new List<Staff>();
            try
            {
                foreach (JToken staff_item in staff)
                {
                    StaffData.Add(new Staff
                    {
                        MachType = (string)staff_item[0],
                        MachineNo = (int)staff_item[1],
                        Sortplan = new Utility().SortPlan_Name_Trimer(staff_item[2].ToString()),
                        Clerk = (double)staff_item[3],
                        Mh = (double)staff_item[4]
                    });
                }
                return StaffData;
            }
            catch (Exception e)
            {

                new ErrorLogger().ExceptionLog(e);
                return null;

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
                _connID = string.Empty;
                _Message_type = string.Empty;
                _data = string.Empty;
                currenStaffInfo = null;
                NewStaffData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StaffingData()
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