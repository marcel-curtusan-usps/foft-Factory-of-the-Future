using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;

namespace Factory_of_the_Future
{
    public class User_Log
    {
        public void LoginUser(ADUser Ad_User)
        {
            try
            {
                JObject adUser = JObject.Parse(JsonConvert.SerializeObject(Ad_User, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                if (AppParameters.CodeBase.Parent.Exists)
                {
                    if (!string.IsNullOrEmpty(AppParameters.AppSettings.ContainsKey("ORACONNASSTRING") ? (string)AppParameters.AppSettings.Property("ORACONNASSTRING").Value : ""))
                    {
                        using (OracleConnection connection = new OracleConnection(AppParameters.Decrypt((string)AppParameters.AppSettings.Property("ORACONNASSTRING").Value)))
                        {
                            string item2 = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.ORAQuery), "UserSessionIN_Query.txt");
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
                                    if (adUser.ContainsKey("SessionID"))
                                    {
                                        command.Parameters.Add(":SESSION_ID", OracleDbType.Varchar2, (string)adUser.Property("SessionID").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("ConnectionId"))
                                    {
                                        command.Parameters.Add(":CONNECTIONID", OracleDbType.Varchar2, (string)adUser.Property("ConnectionId").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("IpAddress"))
                                    {
                                        command.Parameters.Add(":WORKSTATION_ID", OracleDbType.Varchar2, (string)adUser.Property("IpAddress").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("Domain"))
                                    {
                                        command.Parameters.Add(":DOMAIN", OracleDbType.Varchar2, (string)adUser.Property("Domain").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("NASSCode"))
                                    {
                                        command.Parameters.Add(":NASS_CODE", OracleDbType.Varchar2, (string)adUser.Property("NASSCode").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("FDBID"))
                                    {
                                        command.Parameters.Add(":FDB_ID", OracleDbType.Varchar2, (string)adUser.Property("FDBID").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("BrowserType"))
                                    {
                                        command.Parameters.Add(":BROWSER_TYPE", OracleDbType.Varchar2, (string)adUser.Property("BrowserType").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("BrowserName"))
                                    {
                                        command.Parameters.Add(":BROWSER_NAME", OracleDbType.Varchar2, (string)adUser.Property("BrowserName").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("BrowserVersion"))
                                    {
                                        command.Parameters.Add(":BROWSER_VERSION", OracleDbType.Varchar2, (string)adUser.Property("BrowserVersion").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("SoftwareVersion"))
                                    {
                                        command.Parameters.Add(":SOFTWARE_VERSION", OracleDbType.Varchar2, (string)adUser.Property("SoftwareVersion").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("ServerIpAddress"))
                                    {
                                        command.Parameters.Add(":SERVER_IP", OracleDbType.Varchar2, (string)adUser.Property("ServerIpAddress").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("UserId"))
                                    {
                                        command.Parameters.Add(":ACE_ID", OracleDbType.Varchar2, (string)adUser.Property("UserId").Value, ParameterDirection.Input);
                                    }
                                    if (!string.IsNullOrEmpty(AppParameters.ApplicationEnvironment))
                                    {
                                        command.Parameters.Add(":APPPLIACTION_ENVIRONMENT", OracleDbType.Varchar2, AppParameters.ApplicationEnvironment, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("Role"))
                                    {
                                        command.Parameters.Add(":USER_ROLE", OracleDbType.Varchar2, (string)adUser.Property("Role").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("FirstName"))
                                    {
                                        command.Parameters.Add(":FULL_NAME", OracleDbType.Varchar2, (string)adUser.Property("FirstName").Value + " " + adUser.Property("SurName").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("LoginDate"))
                                    {
                                        command.Parameters.Add(":LOGIN_DATE", OracleDbType.TimeStamp, (DateTime)adUser.Property("LoginDate").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("AppType"))
                                    {
                                        command.Parameters.Add(":APP_TYPE", OracleDbType.Varchar2, (string)adUser.Property("AppType").Value, ParameterDirection.Input);
                                    }
                                    command.ExecuteReader().Close();

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
            finally
            {

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
        }

        internal void LoginUser_Update(string DirectoryPath, JObject adUser)
        {
            try
            {
                DirectoryInfo maindir = new DirectoryInfo(DirectoryPath);
                if (maindir.Exists)
                {
                    if (!string.IsNullOrEmpty(AppParameters.AppSettings.ContainsKey("ORACONNASSTRING") ? (string)AppParameters.AppSettings.Property("ORACONNASSTRING").Value : ""))
                    {
                        using (OracleConnection connection = new OracleConnection(AppParameters.Decrypt((string)AppParameters.AppSettings.Property("ORACONNASSTRING").Value)))
                        {
                            string item2 = new FileIO().Read(string.Concat(maindir, AppParameters.ORAQuery), "UserSessionUpdate_Query.txt");
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
                                    if (adUser.ContainsKey("Session_ID"))
                                    {
                                        command.Parameters.Add(":SESSION_ID", OracleDbType.Varchar2, (string)adUser.Property("Session_ID").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("Domain"))
                                    {
                                        command.Parameters.Add(":DOMAIN", OracleDbType.Varchar2, (string)adUser.Property("Domain").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("FirstName"))
                                    {
                                        command.Parameters.Add(":FULL_NAME", OracleDbType.Varchar2, (string)adUser.Property("FirstName").Value + " " + adUser.Property("SurName").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("UserId"))
                                    {
                                        command.Parameters.Add(":ACE_ID", OracleDbType.Varchar2, (string)adUser.Property("UserId").Value, ParameterDirection.Input);
                                    }
                                    if (adUser.ContainsKey("Role"))
                                    {
                                        command.Parameters.Add(":USER_ROLE", OracleDbType.Varchar2, adUser.Property("Role").Value, ParameterDirection.Input);
                                    }
                                    command.ExecuteReader().Close();
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

        internal void Login_User(ADUser adUser)
        {
            throw new NotImplementedException();
        }

        public void LogoutUser(ADUser Ad_User)
        {
            try
            {
                JObject adUser = JObject.Parse(JsonConvert.SerializeObject(Ad_User, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                if (AppParameters.CodeBase.Parent.Exists)
                {
                    if (!string.IsNullOrEmpty(AppParameters.AppSettings.ContainsKey("ORACONNASSTRING") ? (string)AppParameters.AppSettings.Property("ORACONNASSTRING").Value : ""))
                    {
                        using (OracleConnection connection = new OracleConnection(AppParameters.Decrypt((string)AppParameters.AppSettings.Property("ORACONNASSTRING").Value)))
                        {
                            string item2 = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.ORAQuery), "UserSessionOUT_Query.txt");
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
                                    if (adUser.ContainsKey("ConnectionId"))
                                    {
                                        command.Parameters.Add(":CONNECTIONID", OracleDbType.Varchar2, (string)adUser.Property("ConnectionId").Value, ParameterDirection.Input);
                                    }
                                    command.Parameters.Add(":LOGOUT_DATE", OracleDbType.TimeStamp, DateTime.Now, ParameterDirection.Input);
                                    command.ExecuteReader().Close();
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
            finally
            {

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
        }
    }
}