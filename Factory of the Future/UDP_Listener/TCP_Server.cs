using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    public class TCP_Server
    {
  
        readonly static CancellationTokenSource tokenSource = new CancellationTokenSource();
        static CancellationToken token = tokenSource.Token;
        static IPAddress ipAddress = null;

        readonly static Dictionary<string, Metadata> clients = new Dictionary<string, Metadata>();

        public static void StartServer(int port, string ipaddress)
        {
            bool ValidateIP = false;
            if (!string.IsNullOrEmpty(ipaddress))
            {
                ValidateIP = IPAddress.TryParse(ipaddress, out ipAddress);
            }
            if (port > 0 && ValidateIP == true)
            {
                Global.listener = new TcpListener(ipAddress, port);
                Global.listener.Start();
                Task.Run(() => AcceptConnections(token), token);
            }  
        }

        private static async Task AcceptConnections(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client = await Global.listener.AcceptTcpClientAsync();
                Metadata md = new Metadata(client);
                clients.Add(client.Client.RemoteEndPoint.ToString(), md);
                await Task.Run(() => DataReceiver(md), md.token);
            }
        }
        public static void DisposeServer()
        {
            try
            {
                if (clients != null && clients.Count > 0)
                {
                    foreach (KeyValuePair<string, Metadata> curr in clients)
                    {
                        string data = string.Concat("Disconnecting : ", curr.Key);
                        new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
                        curr.Value.Dispose();
                    }
                }

                tokenSource.Cancel();
                tokenSource.Dispose();

                if (Global.listener != null && Global.listener.Server != null)
                {
                    Global.listener.Server.Close();
                    Global.listener.Server.Dispose();
                }

                if (Global.listener != null) Global.listener.Stop();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        static void ListClients()
        {
            foreach (KeyValuePair<string, Metadata> curr in clients)
            {
                string data = string.Concat(" : ", curr.Key);
                new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
            }
        }

        static void RemoveClient()
        {
            ListClients();
            string data = string.Concat("Client : ");
            new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
            string key = Console.ReadLine();
            if (String.IsNullOrEmpty(key)) return;
            Metadata md = clients[key];
            clients.Remove(md.tcpClient.Client.RemoteEndPoint.ToString());
            md.Dispose();
        }
        private static async Task DataReceiver(Metadata md)
        {
            string header = "[" + md.tcpClient.Client.RemoteEndPoint.ToString() + "]";
            string log_data = string.Concat(header, ": ", " data receiver started");
            new ErrorLogger().CustomLog(log_data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));

            try
            {
                while (true)
                {
                    if (!IsClientConnected(md.tcpClient))
                    {
                        log_data = string.Concat(header, ": ", " client no longer connected");
                        new ErrorLogger().CustomLog(log_data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
                        break;
                    }

                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine(header + " cancellation requested");
                        log_data = string.Concat(header, ": ", " cancellation requested");
                        new ErrorLogger().CustomLog(log_data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
                        break;
                    }

                    byte[] data = await DataReadAsync(md.tcpClient, token);
                    if (data == null || data.Length < 1)
                    {
                        await Task.Delay(30);
                        continue;
                    }
                    log_data = string.Concat(header , ": " , Encoding.UTF8.GetString(data));
                    new ErrorLogger().CustomLog(log_data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
                    await Task.Run(() => DataProcess(Encoding.UTF8.GetString(data)), md.token);

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }

            log_data = string.Concat(header, ": ", " data receiver terminating");
            new ErrorLogger().CustomLog(log_data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TCPServer"));
            clients.Remove(md.tcpClient.Client.RemoteEndPoint.ToString());
            md.Dispose();
        }

        private static void DataProcess(string data)
        {
            try
            {
                if (Global.IsValidJson(data))
                {
                    if (data.StartsWith("{"))
                    {
                      JObject UdpData= JObject.Parse(data);
                        if (UdpData.HasValues)
                        {
                            if (UdpData.ContainsKey("message"))
                            {
                                new ProcessRecvdMsg().StartProcess(JObject.Parse(data), new JObject(new JProperty("MESSAGE_TYPE", "TagPosition")));
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

        private static async Task<byte[]> DataReadAsync(TcpClient client, CancellationToken token)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException();

            NetworkStream stream = client.GetStream();
            if (!stream.CanRead) return null;
            if (!stream.DataAvailable) return null;

            byte[] buffer = new byte[1024];
            int read = 0;

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    read = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (read > 0)
                    {
                        ms.Write(buffer, 0, read);
                        return ms.ToArray();
                    }
                    else
                    {
                        throw new SocketException();
                    }
                }
            }
        }

        private static bool IsClientConnected(TcpClient client)
        {
            if (client.Connected)
            {
                if ((client.Client.Poll(0, SelectMode.SelectWrite)) && (!client.Client.Poll(0, SelectMode.SelectError)))
                {
                    byte[] buffer = new byte[1];
                    if (client.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}