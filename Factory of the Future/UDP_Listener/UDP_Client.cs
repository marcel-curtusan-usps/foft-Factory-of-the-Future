using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Factory_of_the_Future
{
    public class UDP_Client
    {
        //bool shouldstop = false;
        //private static readonly ProcessRecvdMsg ProcessRecvdMsg_callback = new ProcessRecvdMsg();
        //internal void Listener(int port, string ipaddress)
        //{
        //    try
        //    {
        //        // Set the TcpListener on port 13000.
        //        IPAddress localAddr = IPAddress.Parse(ipaddress);

        //        // TcpListener server = new TcpListener(port);
        //        Global.udpServer = new TcpListener(localAddr, port);

        //        // Start listening for client requests.
        //        Global.udpServer.Start();

        //        // Buffer for reading data   receive and respond 
        //        Byte[] bytes = new Byte[256];
        //        String receive_data = null;
        //        string respond_data = "0";
        //        // Enter the listening loop.
        //        while (!shouldstop)
        //        {
        //            // Perform a blocking call to accept requests.
        //            // You could also use server.AcceptSocket() here.
        //            TcpClient client = Global.udpServer.AcceptTcpClient();


        //            receive_data = null;

        //            // Get a stream object for reading and writing
        //            NetworkStream stream = client.GetStream();

        //            int i;

        //            // Loop to receive all the data sent by the client.
        //            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        //            {
        //                // Translate data bytes to a ASCII string.
        //                receive_data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                       
        //                if (!string.IsNullOrEmpty(receive_data))
        //                {
        //                    if (Global.IsValidJson(receive_data))
        //                    {
        //                        JObject temp1 = JObject.Parse(receive_data);
        //                        ProcessRecvdMsg_callback.StartProcess(temp1, new JObject());
        //                    }
        //                    else
        //                    {
        //                        respond_data = "Invaild Json format";
        //                    }
        //                }
        //                else
        //                {
        //                    respond_data = "Invaild data";
        //                }
        //                // Process the data sent by the client.
                        

        //                byte[] msg = System.Text.Encoding.ASCII.GetBytes(respond_data);

        //                // Send back a response.
        //                stream.Write(msg, 0, msg.Length);
        //             }

        //            // Shutdown and end connection
        //            client.Close();
        //        }
        //    }
        //    catch (SocketException e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        // Stop listening for new clients.
        //        Global.udpServer.Stop();
        //        shouldstop = true;
        //    }

        //}
        //public void stopListener() {
        //    if (!shouldstop)
        //    {
        //        shouldstop = true;
        //        Global.udpServer = null;
        //    }
            
        //}
        //public void startListener() {
        //    if (Global.udpServer == null)
        //    {
        //        if (!string.IsNullOrEmpty(Global.AppSettings.ContainsKey("UDP_PORT") ? (string)Global.AppSettings.Property("UDP_PORT").Value : ""))
        //        {
        //            int port = string.IsNullOrEmpty((string)Global.AppSettings.Property("UDP_PORT").Value) ? Convert.ToInt32(Global.AppSettings.Property("UDP_PORT").Value) : 9918;
        //            new UDP_Client().Listener(port, Global.ipaddress);
        //        }
        //    }
        //    else
        //    {
        //        if (!shouldstop)
        //        {
        //            shouldstop = true;
        //            Global.udpServer = null;
        //        }
        //        else
        //        {
        //            Global.udpServer.Stop();
        //            Global.udpServer = null;
        //        }
        //    }
        //}

    }
}