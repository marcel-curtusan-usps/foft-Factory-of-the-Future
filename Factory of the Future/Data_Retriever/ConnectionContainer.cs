﻿using Factory_of_the_Future.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    public class ConnectionContainer : IDisposable
    {
        public List<Api_Connection> DataConnection = new List<Api_Connection>();
        private bool disposedValue;
        public bool updateFile;
        public Connection _conn { get; protected set; }
        public Api_Connection NewConnection { get; protected set; }
        public async Task Add(Connection con, bool saveToFile)
        {
            try
            {
                updateFile = saveToFile;
                NewConnection = new Api_Connection();
                con.Status = "Idle/Active";
                if (con.ConnectionName.ToLower() == "MPEWatch".ToLower() && con.ApiConnection)
                {
                    con.IpAddress = "";
                    con.Port = 0;
                    con.Url = "";
                    string sitename = AppParameters.SiteInfo.DisplayName.ToLower().Replace(" ", "_").Replace("&", "").Replace("(", "").Replace(")", "");
                    AppParameters.MPEWatchData.Where(r => r.Value.SiteNameLocal.ToLower() == sitename).Select(y => y.Value).ToList().ForEach(m =>
                    {
                        con.IpAddress = m.Host;
                        con.Port = m.Port;
                        con.Url = m.URL;
                        con.ActiveConnection = con.ActiveConnection;
                    });
                }
                if (con.ConnectionName.ToLower() == "Quuppa".ToLower() && con.ApiConnection)
                {
                    con.IpAddress = "";
                    con.Port = 0;
                    con.Url = "";
               
                    string sitename = AppParameters.SiteInfo.DisplayName.ToLower().Replace(" ", "_").Replace("&", "").Replace("(", "").Replace(")", "");
                    AppParameters.RTLShData.Where(r => r.Value.SiteNameLocal.ToLower() == sitename).Select(y => y.Value).ToList().ForEach(m =>
                    {
                        con.IpAddress = m.Host;
                        con.Port = m.Port;
                        con.Url = m.URL;
                        con.ActiveConnection = con.ActiveConnection;
                    });
                }
                NewConnection.ID = con.Id;
                NewConnection.Status = 0;
                NewConnection.ConnectionInfo = con;
                DataConnection.Add(NewConnection);
                if (con.ActiveConnection)
                {
                    NewConnection.ConstantRefresh = false;
                    if (con.UdpConnection)
                    {
                        NewConnection.UDPThreadListener();
                    }
                    else if (con.TcpIpConnection)
                    {
                        NewConnection.TCPThreadListener();
                    }
                    else if (con.WsConnection)
                    {
                        NewConnection.WSThreadListener();
                    }
                    else if (con.ApiConnection)
                    {
                        if (con.DataRetrieve != 0)
                        {
                            NewConnection.ConstantRefresh = true;
                            NewConnection.ThreadDownload();
                            NewConnection.ThreadRefresh();
                        }
                    }
                }
                else
                {
                    NewConnection.Status = 2;
                    NewConnection.ConnectionInfo.Status = "Stopped/Deactivated";
                }
                if (updateFile)
                {
                    await Task.Run(() => FOTFManager.Instance.BroadcastAddConnection(NewConnection.ConnectionInfo)).ConfigureAwait(false);
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", AppParameters.ConnectionOutPutdata(DataConnection.Select(y => y.ConnectionInfo).ToList()));
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                Dispose();
            }

        }
        public Task EditAsync(Connection updateConndata)
        {
            try
            {
                foreach (Api_Connection Connection_item in DataConnection)
                {
                    if (Connection_item.ConnectionInfo.Id == updateConndata.Id)
                    {
                        updateConndata.LastupDate = DateTime.Now;
                        foreach (PropertyInfo prop in Connection_item.ConnectionInfo.GetType().GetProperties())
                        {
                            if (!new Regex("^(UdpConnection|TcpIpConnection|WsConnection|ApiConnection|ConstantRefresh|Status)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                            {
                                if (prop.GetValue(Connection_item.ConnectionInfo, null).ToString() != prop.GetValue(updateConndata, null).ToString())
                                {
                                    updateFile = true;
                                    prop.SetValue(Connection_item.ConnectionInfo, prop.GetValue(updateConndata, null));

                                }
                            }
                        }
                        if (!updateConndata.ActiveConnection)
                        {
                            Connection_item.ConnectionInfo.Status = "Stopped/Deactivated";
                            Connection_item.ConnectionInfo.DeactivatedDate = DateTime.Now;
                            Connection_item.ConnectionInfo.DeactivatedByUsername = updateConndata.LastupdateByUsername;
                            Connection_item.ConnectionInfo.ActiveConnection = updateConndata.ActiveConnection;
                            if (updateConndata.UdpConnection)
                            {
                                Connection_item.StopUDPListener();
                            }
                            else if (updateConndata.TcpIpConnection)
                            {
                                Connection_item.StopTCPListener();
                            }
                            else if (updateConndata.WsConnection)
                            {
                                Connection_item.WSStop();
                            }
                            else if (updateConndata.ApiConnection)
                            {
                                Connection_item.ConstantRefresh = false;
                                Connection_item.Stop();
                            }

                        }
                        else if (updateConndata.ActiveConnection)
                        {

                            if (updateConndata.UdpConnection)
                            {
                                Connection_item.ConnectionInfo.UdpConnection = updateConndata.UdpConnection;
                                if (Connection_item.ConnectionInfo.Port != updateConndata.Port)
                                {
                                    Connection_item.StartUDPListener();
                                }
                                Connection_item.StartUDPListener();
                            }
                            else if (updateConndata.TcpIpConnection)
                            {
                                Connection_item.ConnectionInfo.TcpIpConnection = updateConndata.TcpIpConnection;
                                if (Connection_item.ConnectionInfo.Port != updateConndata.Port)
                                {
                                    Connection_item.StopTCPListener();
                                }
                                Connection_item.StartTCPListener();
                            }
                            else if (updateConndata.WsConnection)
                            {
                                Connection_item.ConnectionInfo.WsConnection = updateConndata.WsConnection;
                                Connection_item.WSThreadListener();
                            }
                            else if (updateConndata.ApiConnection)
                            {
                                if (updateConndata.DataRetrieve != 0)
                                {
                                    Connection_item.ConstantRefresh = true;
                                    Connection_item.ConnectionInfo.Url = updateConndata.Url;
                                    Connection_item.ConnectionInfo.DataRetrieve = updateConndata.DataRetrieve;
                                    Connection_item.ConnectionInfo.ApiConnection = updateConndata.ApiConnection;
                                    if (Connection_item.Status != 1)
                                    {
                                        Connection_item.ConstantRefresh = true;
                                        Connection_item.ThreadDownload();
                                        Connection_item.ThreadRefresh();
                                    }
                                }

                            }
                        }
                    }
                }
                if (updateFile)
                {
                    _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", AppParameters.ConnectionOutPutdata(DataConnection.Select(y => y.ConnectionInfo).ToList()))).ConfigureAwait(true);
                    _ = Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(DataConnection.Where(r => r.ConnectionInfo.Id == updateConndata.Id).Select(y => y.ConnectionInfo).ToList().FirstOrDefault())).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                Dispose();
            }

            return Task.CompletedTask;
        }

        internal async Task ConnectionUpdate(string connID, int status)
        {
            try
            {
                foreach (Api_Connection Connection_item in DataConnection)
                {
                    if (Connection_item.ConnectionInfo.Id == connID)
                    {
                        Connection_item.Status = status;
                        switch (status)
                        {
                            case 0:
                                Connection_item.ConnectionInfo.Status = "Active";
                                break;
                            case 1:
                                Connection_item.ConnectionInfo.Status = "Running";
                                break;
                            case 2:
                                Connection_item.ConnectionInfo.Status = "Deactivated";
                                break;
                            case 3:
                                Connection_item.ConnectionInfo.Status = "Invalid URL";
                                break;
                            case 4:
                                Connection_item.ConnectionInfo.Status = "No data";
                                break;
                            default:
                                break;
                        }

                        await Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Connection_item.ConnectionInfo)).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                Dispose();
            }
        }

        internal async Task Remove(Connection connection)
        {
            bool conStoped = false;
            try
            {
                foreach (Api_Connection Connection_item in DataConnection)
                {
                    if (Connection_item.ConnectionInfo.Id == connection.Id)
                    {
                        NewConnection = Connection_item;
                        if (NewConnection.ConnectionInfo.UdpConnection)
                        {
                            NewConnection.StopUDPListener();
                            conStoped = true;
                        }
                        else if (NewConnection.ConnectionInfo.TcpIpConnection)
                        {
                            NewConnection.StopTCPListener();
                            conStoped = true;
                        }
                        else if (NewConnection.ConnectionInfo.WsConnection)
                        {
                            NewConnection.WSStop();
                            conStoped = true;
                        }
                        else if (NewConnection.ConnectionInfo.ApiConnection)
                        {
                            NewConnection.Stop();
                            conStoped = true;
                        }

                    }
                }
                if (conStoped)
                {
                    _ = AppParameters.RunningConnection.DataConnection.Remove(NewConnection);
                    await Task.Run(() => FOTFManager.Instance.BroadcastRemoveConnection(NewConnection.ConnectionInfo)).ConfigureAwait(false);
                    updateFile = true;
                }
                if (updateFile)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", AppParameters.ConnectionOutPutdata(DataConnection.Select(y => y.ConnectionInfo).ToList()));
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
                _conn = null;
                NewConnection = null;
                updateFile = false;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DataConnectionContainer()
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