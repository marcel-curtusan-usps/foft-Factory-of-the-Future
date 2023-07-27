using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
                    foreach (var item in from cs in FOTFManager.Instance.CoordinateSystem.Values
                                         from JObject item in tempData
                                         let query = cs.Locators.Where(sl => sl.Value.Properties.TagType == "Person" && sl.Value.Properties.EmpId == item["empId"].ToString()).Select(r => r.Value.Properties).ToList()
                                         let tagID = query.FirstOrDefault().Id.ToString()// string tagID = cs.Locators.Where(sl => sl.Value.Properties.TagType == "Person" && sl.Value.Properties.EmpId == item["empId"].ToString()).Select(r => r.Value.Properties.Id).FirstOrDefault();
                                         where !string.IsNullOrEmpty(tagID) && cs.Locators.ContainsKey(tagID)
                                                                         && cs.Locators.TryGetValue(item["empId"].ToString(), out GeoMarker currentMarker)
                                         select item)
                    {
                        Console.Write(item);
                        //currentMarker.Properties.Bdate = item["bdate"].ToString();
                        //currentMarker.Properties.Bdate = item["edate"].ToString();
                        //currentMarker.Properties.Bdate = item["blunch"].ToString();
                        //currentMarker.Properties.Bdate = item["elunch"].ToString();
                        //currentMarker.Properties.Bdate = item["tourNumber"].ToString();
                        //currentMarker.Properties.Bdate = item["reqDate"].ToString();
                        //currentMarker.Properties.Bdate = item["daysOff"].ToString();
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