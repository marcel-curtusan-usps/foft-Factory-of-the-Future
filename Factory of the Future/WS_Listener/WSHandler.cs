
using System;
using System.Collections.Concurrent;
using System.Security.Authentication;
using WebSocket4Net;

namespace Factory_of_the_Future
{
    public class WebSocketInstanceHandler
    {

        public ConcurrentDictionary<string, WebSocketInstance> instances = new ConcurrentDictionary<string, WebSocketInstance>();
        public void CreateWSInstance(string name, string uri,
            OnWsMessage socketIOMessageEvent, OnWsEvent closeEvent, OnWsEvent openEvent)
        {
            WebSocketInstance newInstance = new WebSocketInstance(name, uri, socketIOMessageEvent, closeEvent, openEvent);
            instances[name] = newInstance;
        }
        private WebSocketInstance GetWSInstance(string name)
        {
            bool found = instances.TryGetValue(name, out WebSocketInstance value);
            if (found)
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public void Connect(string name)
        {
            WebSocketInstance instance = GetWSInstance(name);
            if (instance != null)
            {
                instance.Connect(instance.Uri);
            }
        }
        public void Close(string name)
        {
            WebSocketInstance instance = GetWSInstance(name);
            if (instance != null)
            {
                instance.Close();
            }
        }
        public bool Connected(string name)
        {
            WebSocketInstance instance = GetWSInstance(name);
            if (instance != null)
            {
                return instance.Connected;
            }
            return false;
        }
    }
    public class WebSocketInstance : IDisposable
    {
        public WebSocketInstance(string name, string uri,
            OnWsMessage socketIOMessageEvent, OnWsEvent closeEvent, OnWsEvent openEvent)
        {
            Name = name;
            Uri = uri;
            SocketIOMessageEvent = socketIOMessageEvent;
            SocketIOCloseEvent = closeEvent;
            SocketIOOpenEvent = openEvent;
        }
        public void Dispose()
        {
            if (wsClient != null)
            {
                wsClient.Dispose();
            }
        }
        public bool Connected
        { get; set; }
        public bool Disconnected { get; set; }
        public int Received { get; set; }
        public bool Errors { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
        public OnWsMessage SocketIOMessageEvent { get; set; }
        public OnWsEvent SocketIOCloseEvent { get; set; }
        public OnWsEvent SocketIOOpenEvent { get; set; }
        WebSocket wsClient = null;

        public void Close()
        {
            if (wsClient != null)
            {
                wsClient.Close();
            }
        }


        public void Connect(string Uri)
        {
            if (Connected)
            {
                return;
            }

            /*
             * 
             * for secure connections where domain is outside of engineering,
             * use full domain specification in the uri so certificate is recognized:
             * 
             * wss:// **** .usa.dce.usps.gov *****
             * 
             * instead of
             * 
             * wss:// **** .usps.gov ****
             * 
             * */
            if (Uri.Contains("wss"))
            {
                wsClient = new WebSocket(Uri);
                //wsClient.EnableAutoSendPing = true;
                wsClient.Security.EnabledSslProtocols = SslProtocols.Tls12;
            }
            else
            {
                wsClient = new WebSocket(Uri);
            }

            // wsClient.OnAny(SocketIOMessageEvent);



            wsClient.Opened += (sender, e) =>
            {
                Connected = true;
                SocketIOOpenEvent();
            };

            wsClient.Closed += (sender, e) =>
            {
                Connected = false;
                SocketIOCloseEvent();
                try
                {
                    wsClient.Open();
                }
                catch (Exception ex)
                {
                    new ErrorLogger().ExceptionLog(ex);
                }
            };

            wsClient.Error += (sender, e) =>
            {

            };
            wsClient.MessageReceived += (sender, args) =>
            {
                SocketIOMessageEvent(args.Message);
            };
            wsClient.Open();







        }




    }
}
