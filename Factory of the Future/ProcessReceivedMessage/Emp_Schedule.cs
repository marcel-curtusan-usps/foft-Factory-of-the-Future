using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml.Linq;

namespace Factory_of_the_Future
{
    internal class Emp_Schedule :IDisposable
    {
        private bool disposedValue; 
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        private bool saveToFile;

        public Emp_Schedule()
        {
        }

        internal Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                tempData = JToken.Parse(_data);
                if (tempData != null && tempData.HasValues)
                {

                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        foreach (JObject empitem in tempData)
                        {

                            var tagID = cs.Locators.Where(sl => sl.Value.Properties.TagType == "Person" && sl.Value.Properties.EmpId == empitem["empId"].ToString()).Select(r => r.Value.Properties).ToList();
                            if (tagID.Any() && cs.Locators.TryGetValue(tagID[0].Id, out GeoMarker currentMarker))
                            {
                                currentMarker.Properties.Bdate = empitem["bdate"].ToString();
                                currentMarker.Properties.Edate = empitem["edate"].ToString();
                                currentMarker.Properties.Blunch = empitem["blunch"].ToString();
                                currentMarker.Properties.Elunch = empitem["elunch"].ToString();
                                currentMarker.Properties.TourNumber = empitem["tourNumber"].ToString();
                                currentMarker.Properties.ReqDate = empitem["reqDate"].ToString();
                                currentMarker.Properties.DaysOff = empitem["daysOff"].ToString();
                                //cal is schedule
                                char[] offDays = empitem["daysOff"].ToString().ToCharArray();
                                int dayofweek = (int)empitem["weekDay"];
                                bool isShiftDone = getIsShiftDone(empitem["edate"].ToString());
                                if (offDays[dayofweek].ToString() == "N" && !isShiftDone)
                                {
                                    currentMarker.Properties.isSch = true;
                                }
                                else
                                {
                                    currentMarker.Properties.isSch = false;
                                }
                            }
                        }
                    }
                }
                return Task.FromResult(saveToFile);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Task.FromResult(saveToFile);
            }
            finally
            {

                Dispose();
            }
        }

        private bool getIsShiftDone(string endtime)
        {
           DateTime endtimeshc = DateTime.MinValue;
            if (!string.IsNullOrEmpty(endtime))
            {
                endtimeshc = DateTime.Parse(endtime);
                if (DateTime.Now <= endtimeshc)
                {
                    return true;
                }
            }
            return false;
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
                tempData = null;

            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Emp_Schedule()
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