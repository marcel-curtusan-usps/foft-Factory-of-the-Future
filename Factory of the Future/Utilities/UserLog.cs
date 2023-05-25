using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Web.SessionState;

namespace Factory_of_the_Future
{
    public class UserLog : IDisposable
    {
        private bool disposedValue;
        public string sqlQuery { get; protected set; }
        public HttpSessionState session { get; protected set; }

        internal void LoginUser(HttpSessionState _session)
        {
            try
            {
                session = _session;

                if (AppParameters.CodeBase.Parent.Exists && !string.IsNullOrEmpty(AppParameters.AppSettings.ORACONNASSTRING))
                {
                    using (OracleConnection connection = new OracleConnection(AppParameters.Decrypt(AppParameters.AppSettings.ORACONNASSTRING)))
                    {
                        sqlQuery = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.ORAQuery), "UserSessionIN_Query.txt");
                        if (!string.IsNullOrEmpty(sqlQuery))
                        {
                            using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                            {
                                if (connection.State == ConnectionState.Closed)
                                {
                                    connection.Open();
                                }
                                command.Parameters.Clear();
                                command.BindByName = true;
                                command.Parameters.Add(":SESSION_ID", OracleDbType.Varchar2, session[SessionKey.Session_ID], ParameterDirection.Input);
                                command.Parameters.Add(":CONNECTIONID", OracleDbType.Varchar2, session[SessionKey.Session_ID], ParameterDirection.Input);
                                command.Parameters.Add(":WORKSTATION_ID", OracleDbType.Varchar2, session[SessionKey.IpAddress], ParameterDirection.Input);
                                command.Parameters.Add(":DOMAIN", OracleDbType.Varchar2, session[SessionKey.Domain], ParameterDirection.Input);
                                command.Parameters.Add(":NASS_CODE", OracleDbType.Varchar2, session[SessionKey.Facility_NASS_CODE], ParameterDirection.Input);
                                command.Parameters.Add(":FDB_ID", OracleDbType.Varchar2, session[SessionKey.Facility_FDBID], ParameterDirection.Input);
                                command.Parameters.Add(":BROWSER_TYPE", OracleDbType.Varchar2, session[SessionKey.Browser_Type], ParameterDirection.Input);
                                command.Parameters.Add(":BROWSER_NAME", OracleDbType.Varchar2, session[SessionKey.Browser_Name], ParameterDirection.Input);
                                command.Parameters.Add(":BROWSER_VERSION", OracleDbType.Varchar2, session[SessionKey.Browser_Version], ParameterDirection.Input);
                                command.Parameters.Add(":SOFTWARE_VERSION", OracleDbType.Varchar2, session[SessionKey.SoftwareVersion], ParameterDirection.Input);
                                command.Parameters.Add(":SERVER_IP", OracleDbType.Varchar2, session[SessionKey.Server_IpAddress], ParameterDirection.Input);
                                command.Parameters.Add(":ACE_ID", OracleDbType.Varchar2, session[SessionKey.AceId], ParameterDirection.Input);
                                command.Parameters.Add(":APPPLIACTION_ENVIRONMENT", OracleDbType.Varchar2, AppParameters.ApplicationEnvironment, ParameterDirection.Input);
                                command.Parameters.Add(":USER_ROLE", OracleDbType.Varchar2, session[SessionKey.UserRole], ParameterDirection.Input);
                                command.Parameters.Add(":FULL_NAME", OracleDbType.Varchar2, session[SessionKey.UserFirstName] + " " + session[SessionKey.UserLastName], ParameterDirection.Input);
                                command.Parameters.Add(":LOGIN_DATE", OracleDbType.TimeStamp, DateTime.Now, ParameterDirection.Input);
                                command.Parameters.Add(":APP_TYPE", OracleDbType.Varchar2, session[SessionKey.ApplicationAbbr], ParameterDirection.Input);
                                command.ExecuteReader().Close();

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

        internal void LogoutUser(HttpSessionState _session)
        {
            session = _session;
            try
            {
                if (AppParameters.CodeBase.Parent.Exists && !string.IsNullOrEmpty(AppParameters.AppSettings.ORACONNASSTRING))
                {
                    using (OracleConnection connection = new OracleConnection(AppParameters.Decrypt(AppParameters.AppSettings.ORACONNASSTRING)))
                    {
                        sqlQuery = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.ORAQuery), "UserSessionOUT_Query.txt");
                        if (!string.IsNullOrEmpty(sqlQuery))
                        {
                            using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                            {
                                if (connection.State == ConnectionState.Closed)
                                {
                                    connection.Open();
                                }
                                command.Parameters.Clear();
                                command.BindByName = true;
                                command.Parameters.Add(":SESSION_ID", OracleDbType.Varchar2, session[SessionKey.Session_ID], ParameterDirection.Input);
                                command.Parameters.Add(":LOGOUT_DATE", OracleDbType.TimeStamp, DateTime.Now, ParameterDirection.Input);
                                command.ExecuteReader().Close();
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
                session = null;
                sqlQuery = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~User_Log()
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