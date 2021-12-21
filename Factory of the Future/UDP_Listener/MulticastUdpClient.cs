using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Factory_of_the_Future
{
    //internal class MulticastUdpClient : UdpClient
    //{
    //    private bool _stop;
    //    public MulticastUdpClient(string address, int port) : base(address, port){}

    //    protected override void OnConnected()
    //    {
    //        // Start receive datagrams
    //        ReceiveAsync();
    //    }

    //    protected override void OnDisconnected()
    //    {
    //        // Wait for a while...
    //        Thread.Sleep(1000);

    //        // Try to connect again
    //        if (!_stop)
    //            Connect();
    //    }

    //    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    //    {
    //        try
    //        {
    //            string incomingData = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
    //            if (!string.IsNullOrEmpty(incomingData))
    //            {
    //                if (Global.IsValidJson(incomingData))
    //                {
    //                    JObject incomingDataJobject = JObject.Parse(incomingData);
    //                    JObject temp1 = new JObject(
    //                        new JProperty("code", "0"),
    //                        new JProperty("command", "UDP_Client"),
    //                        new JProperty("outputFormatId", "DefFormat002"),
    //                        new JProperty("outputFormatName", "Location JSON"),
    //                        new JProperty("message", "TagPosition"),
    //                        new JProperty("responseTS", DateTimeOffset.Now.ToUnixTimeMilliseconds()),
    //                        new JProperty("status", "0"),
    //                        new JProperty("tags", new JArray(incomingDataJobject))

    //                        );
    //                    //create new Connection Object
    //                    JObject conn = new JObject(new JProperty("IP_ADDRESS", ((IPEndPoint)endpoint).Address.ToString()));
    //                    try
    //                    {
    //                        Global.ProcessRecvdMsg_callback.StartProcess(temp1, conn);
    //                    }
    //                    catch (Exception r)
    //                    {
    //                        new ErrorLogger().ExceptionLog(r);
    //                    }
    //                }
    //                else
    //                {
    //                    new ErrorLogger().CustomLog(incomingData, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
    //                }
    //            }
    //            ReceiveAsync();
    //        }
    //        catch (Exception e)
    //        {
    //            new ErrorLogger().ExceptionLog(e);
    //        }
    //    }

    //    protected override void OnError(SocketError error)
    //    {
    //        Global.Errors = true;
    //        new ErrorLogger().CustomLog(error.ToString(), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_Error_logs"));
    //    }
    //}
}