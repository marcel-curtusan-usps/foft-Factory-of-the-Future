using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Factory_of_the_Future
{
    public class SV_DB_Call
    {
        internal static JObject GetData(JObject request_data)
        {
            JObject result = new JObject();
            try
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("ORACONNSVSTRING").Value))
                {
                    DirectoryInfo maindir = new DirectoryInfo(Global.CodeBase.Parent.FullName.ToString());
                    if (maindir.Exists)
                    {
                        if (request_data.ContainsKey("QueryName") && string.IsNullOrEmpty(request_data["QueryName"].ToString()))
                        {
                            string query = new FileIO().Read(string.Concat(maindir, Global.ORAQuery), request_data["QueryName"].ToString());
                            if (!string.IsNullOrEmpty(query))
                            {
                                using (OracleConnection connection = new OracleConnection(Global.Decrypt((string)Global.AppSettings.Property("ORACONNSVSTRING").Value)))
                                {
                                    using (OracleCommand command = new OracleCommand(query, connection))
                                    {
                                        try
                                        {
                                            if (connection.State == ConnectionState.Closed)
                                            {
                                                connection.Open();
                                            }
                                            command.Parameters.Clear();
                                            command.BindByName = true;
                                            foreach (KeyValuePair<string, JToken> property in request_data)
                                            {
                                                if (property.Key != "QueryName")
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
                                            }
                                            OracleDataReader odr = command.ExecuteReader();
                                            if (odr.HasRows)
                                            {
                                                while (odr.Read())
                                                {
                                                    JObject objrow = new JObject();
                                                    for (int i = 0; i < odr.FieldCount; i++)
                                                    {
                                                        objrow.Add(new JProperty(odr.GetName(i), odr[i]));
                                                    }
                                                    result.Add(objrow);
                                                }
                                            }
                                            odr.Close();
                                        }
                                        catch (OracleException oe)
                                        {
                                            new ErrorLogger().ExceptionLog(oe);
                                            JObject objrow = new JObject();
                                            objrow.Add(new JProperty(oe.DataSource,oe.Message));
                                        }
                                       
                                    }
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
    }
}