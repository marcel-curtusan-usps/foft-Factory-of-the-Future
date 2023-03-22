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
    public class SV_DB_Call : IDisposable
    {
        private bool disposedValue;
        private JObject result = new JObject();
        private DirectoryInfo maindir = null;
        private string query = string.Empty;

        internal JObject GetData(JObject request_data)
        {
            
            try
            {
                if (!string.IsNullOrEmpty(AppParameters.AppSettings["ORACONNSVSTRING"].ToString()))
                {
                    maindir = new DirectoryInfo(AppParameters.CodeBase.Parent.FullName.ToString());
                    if (maindir.Exists)
                    {
                        if (request_data.ContainsKey("QueryName") && string.IsNullOrEmpty(request_data["QueryName"].ToString()))
                        {
                            query = new FileIO().Read(string.Concat(maindir, AppParameters.ORAQuery), request_data["QueryName"].ToString());
                            if (!string.IsNullOrEmpty(query))
                            {
                                using (OracleConnection connection = new OracleConnection(AppParameters.Decrypt((string)AppParameters.AppSettings.Property("ORACONNSVSTRING").Value)))
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
                                                            command.Parameters.Add(string.Concat(":", property.Key) ?? "", OracleDbType.TimeStamp,(DateTime)property.Value, ParameterDirection.Input);
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
                                            return result;
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
                return result;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return result;
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
                result = new JObject();
                maindir = null;
                query = string.Empty;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SV_DB_Call()
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