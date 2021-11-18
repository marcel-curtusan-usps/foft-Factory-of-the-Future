using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Factory_of_the_Future
{
    internal class Oracle_DB_Calls
    {
        internal void Insert_TagZone(JObject jsonDataObject)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (jsonDataObject.HasValues)
                    {
                        List<string> source = new List<string>();
                        string cmdText = string.Concat("INSERT INTO TAG_DWELL_ZONE ( ");
                        foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                        {
                            if (!string.IsNullOrEmpty((string)property.Value))
                            {
                                source.Add(property.Key);
                            }
                        }
                        for (int i = 0; i < source.Count<string>(); i++)
                        {
                            if (i == 0)
                            {
                                cmdText = string.Concat(cmdText, source[i]);
                            }
                            if (i > 0)
                            {
                                cmdText = string.Concat(cmdText, " , ", source[i]);
                            }
                        }
                        cmdText = string.Concat(cmdText, " ) VALUES (");
                        for (int j = 0; j < source.Count<string>(); j++)
                        {
                            if (j == 0)
                            {
                                cmdText = string.Concat(cmdText, ":", source[j]);
                            }
                            if (j > 0)
                            {
                                cmdText = string.Concat(cmdText, " ,:", source[j]);
                            }
                        }
                        cmdText = string.Concat(cmdText, ")");
                        using (OracleConnection OraConn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand OraComm = new OracleCommand(cmdText, OraConn))
                            {
                                if (OraConn.State == ConnectionState.Closed)
                                {
                                    OraConn.Open();
                                }
                                if (OraConn.State == ConnectionState.Open)
                                {
                                    OraComm.Parameters.Clear();
                                    OraComm.BindByName = true;

                                    foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                                    {
                                        if (!property.Key.Contains("_time"))
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty((string)property.Value))
                                            {
                                                OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                            }
                                        }
                                    }
                                    int result = OraComm.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal void Update_TagZone(JObject tag)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (tag.HasValues)
                    {
                        List<string> source = new List<string>();
                        string cmdText = "UPDATE TAG_DWELL_ZONE SET ";

                        foreach (KeyValuePair<string, JToken> property in tag)
                        {
                            if (!string.IsNullOrEmpty((string)property.Value))
                            {
                                if (property.Key.StartsWith("end_date_time"))
                                {
                                    source.Add(property.Key);
                                }
                            }
                        }
                        for (int i = 0; i < source.Count<string>(); i++)
                        {
                            cmdText = string.Concat(cmdText, source[i], " = :", source[i]) ?? "";
                        }
                        cmdText = string.Concat(cmdText, " WHERE ", " COORDINATESYSTEMID = :coordinateSystemId", " and TAGID = :tagId", " and start_date_time = :start_date_time");
                        using (OracleConnection OraConn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand OraComm = new OracleCommand(cmdText, OraConn))
                            {
                                if (OraConn.State == ConnectionState.Closed)
                                {
                                    OraConn.Open();
                                }
                                if (OraConn.State == ConnectionState.Open)
                                {
                                    OraComm.Parameters.Clear();
                                    OraComm.BindByName = true;

                                    foreach (KeyValuePair<string, JToken> property in tag)
                                    {
                                        if (!string.IsNullOrEmpty((string)property.Value))
                                        {
                                            if (!property.Key.Contains("_time"))
                                            {
                                                if (property.Key.StartsWith("coordinateSystemId"))
                                                {
                                                    OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                                }
                                                if (property.Key.StartsWith("tagId"))
                                                {
                                                    OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                                }
                                            }
                                            else
                                            {
                                                OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                            }
                                        }
                                    }
                                    int result = OraComm.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal void Insert_AGVTagdata(JObject jsonDataObject)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (jsonDataObject.HasValues)
                    {
                        List<string> source = new List<string>();
                        string cmdText = string.Concat("INSERT INTO AGV_TAG ( ");
                        foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                        {
                            source.Add(property.Key);
                        }
                        for (int i = 0; i < source.Count<string>(); i++)
                        {
                            if (i == 0)
                            {
                                cmdText = string.Concat(cmdText, source[i]);
                            }
                            if (i > 0)
                            {
                                cmdText = string.Concat(cmdText, " , ", source[i]);
                            }
                        }
                        cmdText = string.Concat(cmdText, " ) VALUES (");
                        for (int j = 0; j < source.Count<string>(); j++)
                        {
                            if (j == 0)
                            {
                                cmdText = string.Concat(cmdText, ":", source[j]);
                            }
                            if (j > 0)
                            {
                                cmdText = string.Concat(cmdText, " ,:", source[j]);
                            }
                        }
                        cmdText = string.Concat(cmdText, ")");
                        using (OracleConnection OraConn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand OraComm = new OracleCommand(cmdText, OraConn))
                            {
                                if (OraConn.State == ConnectionState.Closed)
                                {
                                    OraConn.Open();
                                }
                                if (OraConn.State == ConnectionState.Open)
                                {
                                    OraComm.Parameters.Clear();
                                    OraComm.BindByName = true;

                                    foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                                    {
                                        if (!property.Key.Contains("TIME"))
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                        }
                                        else
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                        }
                                    }
                                    int result = OraComm.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal void Update_AGVTagdata(JObject data)
        {
            throw new NotImplementedException();
        }

        internal void Insert_RPG_Run_Perf(JObject data)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (data.HasValues)
                    {
                        string strNassCode = Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString();
                        DateTime dtLastUpdate = DateTime.Now;

                        if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                        {
                            if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                            {
                                dtLastUpdate = TimeZoneInfo.ConvertTime(dtLastUpdate, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                            }
                        }
                        data.Add("NASS_CODE", strNassCode);
                        data.Add("UPDATE_DATE_TIME", dtLastUpdate);
                        List<string> source = new List<string>();
                        string cmdText = string.Concat("INSERT INTO RPG_RUN_PERF ( ");
                        foreach (KeyValuePair<string, JToken> property in data)
                        {
                            source.Add(property.Key);
                        }
                        for (int i = 0; i < source.Count<string>(); i++)
                        {
                            if (i == 0)
                            {
                                cmdText = string.Concat(cmdText, source[i]);
                            }
                            if (i > 0)
                            {
                                cmdText = string.Concat(cmdText, " , ", source[i]);
                            }
                        }
                        cmdText = string.Concat(cmdText, " ) VALUES (");
                        for (int j = 0; j < source.Count<string>(); j++)
                        {
                            if (j == 0)
                            {
                                cmdText = string.Concat(cmdText, ":", source[j]);
                            }
                            if (j > 0)
                            {
                                cmdText = string.Concat(cmdText, " ,:", source[j]);
                            }
                        }
                        cmdText = string.Concat(cmdText, ")");
                        using (OracleConnection OraConn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand OraComm = new OracleCommand(cmdText, OraConn))

                            {
                                if (OraConn.State == ConnectionState.Closed)
                                {
                                    OraConn.Open();
                                }
                                if (OraConn.State == ConnectionState.Open)
                                {
                                    OraComm.Parameters.Clear();
                                    OraComm.BindByName = true;

                                    foreach (KeyValuePair<string, JToken> property in data)
                                    {
                                        if ((property.Key.Equals("current_run_start") || property.Key.Equals("current_run_end") || property.Key.ToUpper().Contains("TIME")) && !string.IsNullOrEmpty(property.Value.ToString()))
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                        }
                                        else
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                        }
                                    }
                                    int result = OraComm.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private class RPGPlan
        {
            public DateTime mods_date { get; set; }
            public string machine_num { get; set; }
            public string sort_program_name { get; set; }
            public DateTime rpg_start_dtm { get; set; }
            public DateTime rpg_end_dtm { get; set; }
            public string rpg_pieces_fed { get; set; }
            public string mail_operation_nbr { get; set; }
            public string line_4_text { get; set; }
            public DateTime mpew_start_15min_dtm { get; set; }
            public DateTime mpew_end_15min_dtm { get; set; }
            public string mpe_type { get; set; }
            public string mpe_name { get; set; }
            public DateTime update_date_time { get; set; }
            public string nass_code { get; set; }
        }

        internal void Insert_RPG_Plan(JObject dataList)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (dataList.HasValues)
                    {
                        using (OracleConnection conn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand cmd = new OracleCommand("TRUNCATE_RPG_PLAN_STAGE", conn))
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    new ErrorLogger().ExceptionLog(ex);
                                }
                            }
                        }
                        JToken dataToken = dataList.SelectToken("data");
                        List<RPGPlan> lstRPGPlanData = new List<RPGPlan>();
                        if (dataToken != null)
                        {
                            string strNassCode = Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString();
                            DateTime dtLastUpdate = DateTime.Now;

                            if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                            {
                                if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                                {
                                    dtLastUpdate = TimeZoneInfo.ConvertTime(dtLastUpdate, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                }
                            }
                            foreach (JObject data in dataToken.Children())
                            {
                                RPGPlan _rpgPlan = new RPGPlan
                                {
                                    mods_date = DateTime.ParseExact(((DateTime)data.Property("mods_date").Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null),
                                    machine_num = data.ContainsKey("machine_num") ? data.Property("machine_num").Value.ToString().Trim() : "",
                                    sort_program_name = data.ContainsKey("sort_program_name") ? data.Property("sort_program_name").Value.ToString().Trim() : "",
                                    rpg_start_dtm = DateTime.ParseExact(((DateTime)data.Property("rpg_start_dtm").Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null),
                                    rpg_end_dtm = DateTime.ParseExact(((DateTime)data.Property("rpg_end_dtm").Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null),
                                    rpg_pieces_fed = data.ContainsKey("rpg_pieces_fed") ? data.Property("rpg_pieces_fed").Value.ToString().Trim() : "",
                                    mail_operation_nbr = data.ContainsKey("mail_operation_nbr") ? data.Property("mail_operation_nbr").Value.ToString().Trim() : "",
                                    line_4_text = data.ContainsKey("line_4_text") ? data.Property("line_4_text").Value.ToString().Trim() : "",
                                    mpew_start_15min_dtm = DateTime.ParseExact(((DateTime)data.Property("mpew_start_15min_dtm").Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null),
                                    mpew_end_15min_dtm = DateTime.ParseExact(((DateTime)data.Property("mpew_end_15min_dtm").Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null),
                                    mpe_type = data.ContainsKey("mpe_type") ? data.Property("mpe_type").Value.ToString().Trim() : "",
                                    mpe_name = data.ContainsKey("mpe_name") ? data.Property("mpe_name").Value.ToString().Trim() : "",
                                    update_date_time = dtLastUpdate,
                                    nass_code = strNassCode
                                };
                                lstRPGPlanData.Add(_rpgPlan);
                            }

                            string sql = @"INSERT INTO rpg_plan_stage (
                                    mods_date,
                                    machine_num,
                                    sort_program_name,
                                    rpg_start_dtm,
                                    rpg_end_dtm,
                                    rpg_pieces_fed,
                                    mail_operation_nbr,
                                    line_4_text,
                                    mpew_start_15min_dtm,
                                    mpew_end_15min_dtm,
                                    mpe_type,
                                    mpe_name,
                                    update_date_time,
                                    nass_code
                                ) VALUES (
                                    :mods_date,
                                    :machine_num,
                                    :sort_program_name,
                                    :rpg_start_dtm,
                                    :rpg_end_dtm,
                                    :rpg_pieces_fed,
                                    :mail_operation_nbr,
                                    :line_4_text,
                                    :mpew_start_15min_dtm,
                                    :mpew_end_15min_dtm,
                                    :mpe_type,
                                    :mpe_name,
                                    :update_date_time,
                                    :nass_code
                                )";

                            using (OracleConnection OraConn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                            {
                                using (OracleCommand OraComm = new OracleCommand(sql, OraConn))
                                {
                                    OraConn.Open();
                                    OraComm.BindByName = true;
                                    OraComm.ArrayBindCount = lstRPGPlanData.Count();
                                    OraComm.Parameters.Add(":mods_date", OracleDbType.TimeStamp, lstRPGPlanData.Select(i => i.mods_date).ToArray(), ParameterDirection.Input);
                                    //OraComm.Parameters.Add(":mods_date", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)lstRPGPlanData.Select(i => i.mods_date)).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null).toa, ParameterDirection.Input);
                                    //DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null)
                                    OraComm.Parameters.Add(":machine_num", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.machine_num).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":sort_program_name", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.sort_program_name).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":rpg_start_dtm", OracleDbType.TimeStamp, lstRPGPlanData.Select(i => i.rpg_start_dtm).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":rpg_end_dtm", OracleDbType.TimeStamp, lstRPGPlanData.Select(i => i.rpg_end_dtm).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":rpg_pieces_fed", OracleDbType.Int32, lstRPGPlanData.Select(i => i.rpg_pieces_fed).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":mail_operation_nbr", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.mail_operation_nbr).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":line_4_text", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.line_4_text).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":mpew_start_15min_dtm", OracleDbType.TimeStamp, lstRPGPlanData.Select(i => i.mpew_start_15min_dtm).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":mpew_end_15min_dtm", OracleDbType.TimeStamp, lstRPGPlanData.Select(i => i.mpew_end_15min_dtm).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":mpe_type", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.mpe_type).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":mpe_name", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.mpe_name).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":update_date_time", OracleDbType.TimeStamp, lstRPGPlanData.Select(i => i.update_date_time).ToArray(), ParameterDirection.Input);
                                    OraComm.Parameters.Add(":nass_code", OracleDbType.Varchar2, lstRPGPlanData.Select(i => i.nass_code).ToArray(), ParameterDirection.Input);
                                    OraComm.ExecuteNonQuery();
                                }
                            }
                        }

                        using (OracleConnection conn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand cmd = new OracleCommand("UPDATE_RPG_PLAN", conn))
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    new ErrorLogger().ExceptionLog(ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        internal JObject Get_RPG_Plan_Info(JObject data)
        {
            JObject result = new JObject();
            try
            {
                if (data.HasValues)
                {
                    if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                    {
                        using (OracleConnection conn = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                        {
                            using (OracleCommand cmd = new OracleCommand("Get_RPG_Plan_Info", conn))
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("CUR_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                    cmd.Parameters.Add("nasscode", Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString());
                                    cmd.Parameters.Add("mpeType", data.ContainsKey("mpe_type") ? data.Property("mpe_type").Value.ToString().Trim() : "");
                                    cmd.Parameters.Add("mpeNumber", data.ContainsKey("mpe_number") ? data.Property("mpe_number").Value.ToString().Trim() : "");
                                    cmd.Parameters.Add("sortplan", data.ContainsKey("cur_sortplan") ? data.Property("cur_sortplan").Value.ToString().Trim() : "");
                                    cmd.Parameters.Add("opn", data.ContainsKey("cur_operation_id") ? data.Property("cur_operation_id").Value.ToString().Trim() : "");
                                    cmd.Parameters.Add("currundate", (DateTime)data.Property("current_run_start").Value);
                                    cmd.Connection.Open();
                                    OracleDataReader odr = cmd.ExecuteReader();
                                    if (odr.HasRows)
                                    {
                                        odr.Read();
                                        result.Add("rpg_start_dtm", odr["RPG_START_DTM"].ToString());
                                        result.Add("rpg_end_dtm", odr["RPG_END_DTM"].ToString());
                                        string expThroughput = odr["LINE_4_TEXT"].ToString();
                                        if (expThroughput.Length > 0)
                                        {
                                            string[] expth = expThroughput.Split(' ');
                                            result.Add("expected_throughput", expth[0]);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    new ErrorLogger().ExceptionLog(ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
            return result;
        }

        internal void Vehicle_History_Update(JObject jsonDataObject)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (jsonDataObject.HasValues)
                    {
                        List<string> source = new List<string>();
                        string cmdText = "UPDATE VEHICLE_STATE_HISTORY SET ";

                        foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                        {
                            if (!string.IsNullOrEmpty((string)property.Value))
                            {
                                if (property.Key.StartsWith("STATUS_CHANGE_END_DATE_TIME"))
                                {
                                    source.Add(property.Key);
                                }
                            }
                        }
                        for (int i = 0; i < source.Count<string>(); i++)
                        {
                            cmdText = string.Concat(cmdText, source[i], " = :", source[i]) ?? "";
                        }
                        cmdText = string.Concat(cmdText, " WHERE ", "MAC_ADDRESS", " = :MAC_ADDRESS", "  and NASS_CODE ='" + Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString() + "' and VEHICLE_NUMBER = :VEHICLE_NUMBER", " and STATUS_CHANGE_START_DATE_TIME = :STATUS_CHANGE_START_DATE_TIME");
                        using (OracleConnection OraConn = new OracleConnection((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                        {
                            using (OracleCommand OraComm = new OracleCommand(cmdText, OraConn))
                            {
                                if (OraConn.State == ConnectionState.Closed)
                                {
                                    OraConn.Open();
                                }
                                if (OraConn.State == ConnectionState.Open)
                                {
                                    OraComm.Parameters.Clear();
                                    OraComm.BindByName = true;

                                    foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                                    {
                                        if (!string.IsNullOrEmpty((string)property.Value))
                                        {
                                            if (!property.Key.Contains("_TIME"))
                                            {
                                                if (property.Key.StartsWith("VEHICLE_NUMBER"))
                                                {
                                                    OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                                }
                                                if (property.Key.StartsWith("IP_ADDRESS"))
                                                {
                                                    OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                                }
                                            }
                                            else
                                            {
                                                OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                            }
                                        }
                                    }
                                    int result = OraComm.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal void Vehicle_History_Insert(JObject jsonDataObject)
        {
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                {
                    if (jsonDataObject.HasValues)
                    {
                        jsonDataObject.Add("NASS_CODE", Global.AppSettings.ContainsKey("FACILITY_NASS_CODE") ? Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString() : "NA");
                        List<string> source = new List<string>();
                        string cmdText = string.Concat("INSERT INTO VEHICLE_STATE_HISTORY ( ");
                        foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                        {
                            source.Add(property.Key);
                        }
                        for (int i = 0; i < source.Count<string>(); i++)
                        {
                            if (i == 0)
                            {
                                cmdText = string.Concat(cmdText, source[i]);
                            }
                            if (i > 0)
                            {
                                cmdText = string.Concat(cmdText, " , ", source[i]);
                            }
                        }
                        cmdText = string.Concat(cmdText, " ) VALUES (");
                        for (int j = 0; j < source.Count<string>(); j++)
                        {
                            if (j == 0)
                            {
                                cmdText = string.Concat(cmdText, ":", source[j]);
                            }
                            if (j > 0)
                            {
                                cmdText = string.Concat(cmdText, " ,:", source[j]);
                            }
                        }
                        cmdText = string.Concat(cmdText, ")");
                        using (OracleConnection OraConn = new OracleConnection((string)Global.AppSettings.Property("ORACONNSTRING").Value))
                        {
                            using (OracleCommand OraComm = new OracleCommand(cmdText, OraConn))
                            {
                                if (OraConn.State == ConnectionState.Closed)
                                {
                                    OraConn.Open();
                                }
                                if (OraConn.State == ConnectionState.Open)
                                {
                                    OraComm.Parameters.Clear();
                                    OraComm.BindByName = true;

                                    foreach (KeyValuePair<string, JToken> property in jsonDataObject)
                                    {
                                        if (!property.Key.Contains("_TIME"))
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                        }
                                        else
                                        {
                                            OraComm.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                        }
                                    }
                                    int result = OraComm.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal JArray GetData(string QueryName, JObject data)
        {
            JArray list = new JArray();
            try
            {
                if (!string.IsNullOrEmpty(QueryName))
                {
                    DirectoryInfo maindir = new DirectoryInfo(Global.CodeBase.Parent.FullName.ToString());
                    if (maindir.Exists)
                    {
                        if (!string.IsNullOrEmpty(Global.AppSettings.ContainsKey("ORACONNSTRING") ? (string)Global.AppSettings.Property("ORACONNSTRING").Value : ""))
                        {
                            using (OracleConnection connection = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSTRING").Value)))
                            {
                                string item2 = new FileIO().Read(string.Concat(maindir, Global.ORAQuery), QueryName);
                                if (!string.IsNullOrEmpty(item2))
                                {
                                    using (OracleCommand command = new OracleCommand(item2, connection))
                                    {
                                        if (connection.State == ConnectionState.Closed)
                                        {
                                            connection.Open();
                                        }
                                        command.Parameters.Clear();
                                        command.BindByName = true;
                                        if (data.HasValues)
                                        {
                                            foreach (KeyValuePair<string, JToken> property in data)
                                            {
                                                if (!string.IsNullOrEmpty((string)property.Value))
                                                {
                                                    if (!property.Key.EndsWith("_time"))
                                                    {
                                                        command.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.Varchar2, (string)property.Value, ParameterDirection.Input);
                                                    }
                                                    else
                                                    {
                                                        command.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp, DateTime.ParseExact(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss.fff"), "yyyy-MM-dd HH:mm:ss.fff", null), ParameterDirection.Input);
                                                    }
                                                }
                                            }

                                            OracleDataReader reader = command.ExecuteReader();
                                            if (reader.HasRows)
                                            {
                                                while (reader.Read())
                                                {
                                                    JObject objrow = new JObject();
                                                    for (int i = 0; i < reader.FieldCount; i++)
                                                    {
                                                        objrow.Add(new JProperty(reader.GetName(i), reader[i]));
                                                    }
                                                    list.Add(objrow);
                                                }
                                            }
                                            reader.Close();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return new JArray();
            }
        }
    }
}