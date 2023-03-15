using Factory_of_the_Future.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class MPEWatch_RPGPlan :IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        private bool saveToFile;
        public JToken tempData = null;
        public JToken planInfo = null;
        //internal async Task LoadAsync(dynamic data/*, string connID*/)
        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                tempData = JToken.Parse(data);
                planInfo = tempData.SelectToken("data");
                if (planInfo != null && planInfo.HasValues)
                {
                    var newPlanData = GetMPEPlanList(planInfo);

                    foreach (RPGPlan RPG_item in newPlanData)
                    {
                        UpdateRPGPlanData(RPG_item); /*If this is executing for the first time then concurrentdictionary is empty, thus we populate the dictionary and assing the key part with the complex Key*/
                    }
                }

                //remove old data
                RemoveOldDataFromMPEPRPGList();
                return saveToFile;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return saveToFile;
            }
            finally
            {
                Dispose();
            }
        }

        private string RemoveFileExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private int GetMailOperationNumber(string unformattedMailOperationNumber)
        {
            string mailOperationNumber = unformattedMailOperationNumber.Remove(unformattedMailOperationNumber.Length - 3);
            if (mailOperationNumber.Length >= 3)
            {
                if (int.TryParse(mailOperationNumber, out int parsedMailOperationNumber) == true)
                {
                    return parsedMailOperationNumber;
                }
            }
            return 0;
        }

        private string ConvertDateTimeToId(DateTime dateToConvert)
        {
            int month = dateToConvert.Month;
            int day = dateToConvert.Day;
            int year = dateToConvert.Year;
            //int hour = dateToConvert.Hour;
            //int minutes = dateToConvert.Minute;
            //int seconds = dateToConvert.Second;
            return string.Concat(month.ToString(), day.ToString(), year.ToString());
            //return string.Concat(month.ToString(), day.ToString(), year.ToString(), hour.ToString(), minutes.ToString(), seconds.ToString());
        }

        private void UpdateRPGPlanData(RPGPlan newRPGPlan)
        {
            if (AppParameters.MPEPRPGList.ContainsKey(newRPGPlan.Id))
            {
                AppParameters.MPEPRPGList[newRPGPlan.Id] = newRPGPlan;
            }
            else
            {
                if (!AppParameters.MPEPRPGList.TryAdd(newRPGPlan.Id, newRPGPlan))
                {
                    new ErrorLogger().CustomLog("Unable to Add new RPG Plan" + newRPGPlan.MpeType + " " + newRPGPlan.MachineNum.ToString() + " " + newRPGPlan.SortProgramName, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                }
            }
        }

        private int GetExpectedThroughput(string rpgExpectedThruput)
        {
            string expectedThroughput;
            if (string.IsNullOrEmpty(rpgExpectedThruput) == true)
            {
                return 0;
            }

            expectedThroughput = !string.IsNullOrEmpty(rpgExpectedThruput)? rpgExpectedThruput.Split(' ').FirstOrDefault() : "0";
            if (Int32.TryParse(expectedThroughput, out int intExpectedThroughput) == true)
            {
                return intExpectedThroughput;
            }

            new ErrorLogger().CustomLog("Unable to get expected throughput from RPGPlan object method GetExpectedThroughput", string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
            return 0;
        }


        private void RemoveOldDataFromMPEPRPGList()
        {
            if (AppParameters.MPEPRPGList.Keys.Count > 0)
            {
                //var keys = AppParameters.MPEPRPGList.Where(f => f.Value.RpgStartDtm.Date <= DateTime.Now.AddDays(-2).Date).Select(y => y.Key).ToList();
                foreach (string existingkey in AppParameters.MPEPRPGList.Where(f => f.Value.RpgStartDtm.Date <= DateTime.Now.AddDays(-2).Date).Select(y => y.Key).ToList())
                {
                    if (!AppParameters.MPEPRPGList.TryRemove(existingkey, out RPGPlan existingValue))
                    {
                        new ErrorLogger().CustomLog("Unable to remove RPG Plan item" + existingValue.MpeName + " " + existingValue.MpeType + " " + existingValue.MachineNum, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                    }
                }
            }
        }

        private List<RPGPlan> GetMPEPlanList(JToken planInfo)
        {
            List<RPGPlan> MPEPlanData = new List<RPGPlan>();
            try
            {
                foreach (JObject item in planInfo.Children())
                {
                    MPEPlanData.Add(new RPGPlan
                    {
                        MachineNum = !string.IsNullOrEmpty(item["machine_num"].ToString()) || item["machine_num"].ToString() == "0"? Convert.ToInt32(item["machine_num"]) : 0,
                        SortProgramName = new Utility().SortPlan_Name_Trimer(item["sort_program_name"].ToString()),
                        RpgStartDtm = (DateTime)item["rpg_start_dtm"],
                        RpgEndDtm = (DateTime)item["rpg_end_dtm"],
                        RpgPiecesFed = !string.IsNullOrEmpty(item["rpg_pieces_fed"].ToString()) || item["rpg_pieces_fed"].ToString() == "0"? Convert.ToInt32(item["rpg_pieces_fed"].ToString()) : 0,
                        MailOperationNbr = !string.IsNullOrEmpty(item["mail_operation_nbr"].ToString())? GetMailOperationNumber(item["mail_operation_nbr"].ToString()) : 0,
                        RpgExpectedThruput = !string.IsNullOrEmpty(item["rpg_expected_thruput"].ToString())? GetExpectedThroughput(item["rpg_expected_thruput"].ToString()) : 0,
                        MpewStart15minDtm = (DateTime)item["mpew_start_15min_dtm"],
                        MpewEnd15minDtm = (DateTime)item["mpew_end_15min_dtm"],
                        MpeType = item["mpe_type"].ToString(),
                        MpeName = item["mpe_name"].ToString(),
                        Id = string.Concat(item["mpe_name"].ToString(),
                             !string.IsNullOrEmpty(item["machine_num"].ToString())? item["machine_num"].ToString() : "0",
                             new Utility().SortPlan_Name_Trimer(item["sort_program_name"].ToString()),
                             !string.IsNullOrEmpty(item["mail_operation_nbr"].ToString())? item["mail_operation_nbr"].ToString() : "0",
                             ConvertDateTimeToId((DateTime)item["rpg_start_dtm"]))
                    });
                }
                return MPEPlanData;
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
                /**As this are not variables declared for the entire class, it is not needed to set them as null, but I left them here just in case you want to try**/
                //tempData = null;
                //planInfo = null;
                //newPlanData = null;
                disposedValue = true;
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
                planInfo = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MPEWatch_RPGPlan()
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