﻿using Factory_of_the_Future.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    /// <summary>
    /// TCP server is used to connect, disconnect and manage TCP sessions
    /// </summary>
    /// <remarks>Thread-safe</remarks>
    public class TcpServer : IDisposable
    {
        /// <summary>
        /// Initialize TCP server with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public TcpServer(IPAddress address, int port, Connection conn) : this(new IPEndPoint(address, port), conn) { }
        /// <summary>
        /// Initialize TCP server with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public TcpServer(string address, int port, Connection conn) : this(new IPEndPoint(IPAddress.Parse(address), port), conn) { }
        /// <summary>
        /// Initialize TCP server with a given DNS endpoint
        /// </summary>
        /// <param name="endpoint">DNS endpoint</param>
        public TcpServer(DnsEndPoint endpoint, Connection conn) : this(endpoint as EndPoint, endpoint.Host, endpoint.Port, conn) { }
        /// <summary>
        /// Initialize TCP server with a given IP endpoint
        /// </summary>
        /// <param name="endpoint">IP endpoint</param>
        public TcpServer(IPEndPoint endpoint, Connection conn) : this(endpoint as EndPoint, endpoint.Address.ToString(), endpoint.Port, conn) { }
        /// <summary>
        /// Initialize TCP server with a given endpoint, address and port
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="address">Server address</param>
        /// <param name="port">Server port</param>
        private TcpServer(EndPoint endpoint, string address, int port, Connection conn)
        {
            Id = Guid.NewGuid();
            Address = address;
            Port = port;
            Endpoint = endpoint;
            Conn = conn;
        }

        /// <summary>
        /// Server Id
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Connection Id
        /// </summary>
        public Connection Conn { get; }
        /// <summary>
        /// TCP server address
        /// </summary>
        public string Address { get; }
        /// <summary>
        /// TCP server port
        /// </summary>
        public int Port { get; }
        /// <summary>
        /// Endpoint
        /// </summary>
        public EndPoint Endpoint { get; private set; }

        /// <summary>
        /// Number of sessions connected to the server
        /// </summary>
        public long ConnectedSessions { get { return Sessions.Count; } }
        /// <summary>
        /// Number of bytes pending sent by the server
        /// </summary>
        public long BytesPending { get { return _bytesPending; } }
        /// <summary>
        /// Number of bytes sent by the server
        /// </summary>
        public long BytesSent { get { return _bytesSent; } }
        /// <summary>
        /// Number of bytes received by the server
        /// </summary>
        public long BytesReceived { get { return _bytesReceived; } }

        /// <summary>
        /// Option: acceptor backlog size
        /// </summary>
        /// <remarks>
        /// This option will set the listening socket's backlog size
        /// </remarks>
        public int OptionAcceptorBacklog { get; set; } = 1024;
        /// <summary>
        /// Option: dual mode socket
        /// </summary>
        /// <remarks>
        /// Specifies whether the Socket is a dual-mode socket used for both IPv4 and IPv6.
        /// Will work only if socket is bound on IPv6 address.
        /// </remarks>
        public bool OptionDualMode { get; set; }
        /// <summary>
        /// Option: keep alive
        /// </summary>
        /// <remarks>
        /// This option will setup SO_KEEPALIVE if the OS support this feature
        /// </remarks>
        public bool OptionKeepAlive { get; set; }
        /// <summary>
        /// Option: TCP keep alive time
        /// </summary>
        /// <remarks>
        /// The number of seconds a TCP connection will remain alive/idle before keepalive probes are sent to the remote
        /// </remarks>
        public int OptionTcpKeepAliveTime { get; set; } = -1;
        /// <summary>
        /// Option: TCP keep alive interval
        /// </summary>
        /// <remarks>
        /// The number of seconds a TCP connection will wait for a keepalive response before sending another keepalive probe
        /// </remarks>
        public int OptionTcpKeepAliveInterval { get; set; } = -1;
        /// <summary>
        /// Option: TCP keep alive retry count
        /// </summary>
        /// <remarks>
        /// The number of TCP keep alive probes that will be sent before the connection is terminated
        /// </remarks>
        public int OptionTcpKeepAliveRetryCount { get; set; } = -1;
        /// <summary>
        /// Option: no delay
        /// </summary>
        /// <remarks>
        /// This option will enable/disable Nagle's algorithm for TCP protocol
        /// </remarks>
        public bool OptionNoDelay { get; set; }
        /// <summary>
        /// Option: reuse address
        /// </summary>
        /// <remarks>
        /// This option will enable/disable SO_REUSEADDR if the OS support this feature
        /// </remarks>
        public bool OptionReuseAddress { get; set; }
        /// <summary>
        /// Option: enables a socket to be bound for exclusive access
        /// </summary>
        /// <remarks>
        /// This option will enable/disable SO_EXCLUSIVEADDRUSE if the OS support this feature
        /// </remarks>
        public bool OptionExclusiveAddressUse { get; set; }
        /// <summary>
        /// Option: receive buffer size
        /// </summary>
        public int OptionReceiveBufferSize { get; set; } = 8192;
        /// <summary>
        /// Option: send buffer size
        /// </summary>
        public int OptionSendBufferSize { get; set; } = 8192;

        #region Start/Stop server

        // Server acceptor
        private Socket _acceptorSocket;
        private SocketAsyncEventArgs _acceptorEventArg;

        // Server statistic
        internal long _bytesPending;
        internal long _bytesSent;
        internal long _bytesReceived;

        /// <summary>
        /// Is the server started?
        /// </summary>
        public bool IsStarted { get; private set; }
        /// <summary>
        /// Is the server accepting new clients?
        /// </summary>
        public bool IsAccepting { get; private set; }

        /// <summary>
        /// Create a new socket object
        /// </summary>
        /// <remarks>
        /// Method may be override if you need to prepare some specific socket object in your implementation.
        /// </remarks>
        /// <returns>Socket object</returns>
        protected virtual Socket CreateSocket()
        {
            return new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Start the server
        /// </summary>
        /// <returns>'true' if the server was successfully started, 'false' if the server failed to start</returns>
        public virtual bool Start()
        {
            Debug.Assert(!IsStarted, "TCP server is already started!");
            if (IsStarted)
                return false;

            // Setup acceptor event arg
            _acceptorEventArg = new SocketAsyncEventArgs();
            _acceptorEventArg.Completed += OnAsyncCompleted;

            // Create a new acceptor socket
            _acceptorSocket = CreateSocket();

            // Update the acceptor socket disposed flag
            IsSocketDisposed = false;

            // Apply the option: reuse address
            _acceptorSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, OptionReuseAddress);
            // Apply the option: exclusive address use
            _acceptorSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, OptionExclusiveAddressUse);
            // Apply the option: dual mode (this option must be applied before listening)
            if (_acceptorSocket.AddressFamily == AddressFamily.InterNetworkV6)
                _acceptorSocket.DualMode = OptionDualMode;

            // Bind the acceptor socket to the endpoint
            _acceptorSocket.Bind(Endpoint);
            // Refresh the endpoint property based on the actual endpoint created
            Endpoint = _acceptorSocket.LocalEndPoint;

            // Call the server starting handler
            OnStarting();

            // Start listen to the acceptor socket with the given accepting backlog size
            _acceptorSocket.Listen(OptionAcceptorBacklog);

            // Reset statistic
            _bytesPending = 0;
            _bytesSent = 0;
            _bytesReceived = 0;

            // Update the started flag
            IsStarted = true;

            // Call the server started handler
            OnStarted();

            // Perform the first server accept
            IsAccepting = true;
            // Task.Run(() => updateConnection());
            StartAccept(_acceptorEventArg);

            return true;
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        /// <returns>'true' if the server was successfully stopped, 'false' if the server is already stopped</returns>
        public virtual bool Stop()
        {
            Debug.Assert(IsStarted, "TCP server is not started!");
            if (!IsStarted)
                return false;

            // Stop accepting new clients
            IsAccepting = false;
            // Task.Run(() => updateConnection());
            // Reset acceptor event arg
            _acceptorEventArg.Completed -= OnAsyncCompleted;

            // Call the server stopping handler
            OnStopping();

            try
            {
                // Close the acceptor socket
                _acceptorSocket.Close();

                // Dispose the acceptor socket
                _acceptorSocket.Dispose();

                // Dispose event arguments
                _acceptorEventArg.Dispose();

                // Update the acceptor socket disposed flag
                IsSocketDisposed = true;
            }
            catch (ObjectDisposedException) { }

            // Disconnect all sessions
            DisconnectAll();

            // Update the started flag
            IsStarted = false;

            // Call the server stopped handler
            OnStopped();

            return true;
        }

        /// <summary>
        /// Restart the server
        /// </summary>
        /// <returns>'true' if the server was successfully restarted, 'false' if the server failed to restart</returns>
        public virtual bool Restart()
        {
            if (!Stop())
                return false;

            while (IsStarted)
                Thread.Yield();

            return Start();
        }

        #endregion

        #region Accepting clients

        /// <summary>
        /// Start accept a new client connection
        /// </summary>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            // Socket must be cleared since the context object is being reused
            e.AcceptSocket = null;

            // Async accept a new client connection
            if (!_acceptorSocket.AcceptAsync(e))
            {

                ProcessAccept(e);
            }
        }
        //private void updateConnection()
        //{
        //    try
        //    {
        //        foreach (Connection m in AppParameters.ConnectionList.Where(x => x.Value.Id ==this.Conid).Select(y => y.Value))
        //        {

        //            m.ApiConnected = IsAccepting;
        //            m.LasttimeApiConnected = DateTime.Now;
        //            m.UpdateStatus = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        /// <summary>
        /// Process accepted client connection
        /// </summary>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // Create a new session to register
                var session = CreateSession();

                // Register the session
                RegisterSession(session);

                // Connect new session
                session.Connect(e.AcceptSocket);
            }
            else
                SendError(e.SocketError);

            // Accept the next client connection
            if (IsAccepting)
                StartAccept(e);
        }

        /// <summary>
        /// This method is the callback method associated with Socket.AcceptAsync()
        /// operations and is invoked when an accept operation is complete
        /// </summary>
        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (IsSocketDisposed)
                return;

            ProcessAccept(e);
        }

        #endregion

        #region Session factory

        /// <summary>
        /// Create TCP session factory method
        /// </summary>
        /// <returns>TCP session</returns>
        protected virtual TcpSession CreateSession() { return new TcpSession(this); }

        #endregion

        #region Session management

        // Server sessions
        protected readonly ConcurrentDictionary<Guid, TcpSession> Sessions = new ConcurrentDictionary<Guid, TcpSession>();

        /// <summary>
        /// Disconnect all connected sessions
        /// </summary>
        /// <returns>'true' if all sessions were successfully disconnected, 'false' if the server is not started</returns>
        public virtual bool DisconnectAll()
        {
            if (!IsStarted)
                return false;

            // Disconnect all sessions
            foreach (var session in Sessions.Values)
                session.Disconnect();

            return true;
        }

        /// <summary>
        /// Find a session with a given Id
        /// </summary>
        /// <param name="id">Session Id</param>
        /// <returns>Session with a given Id or null if the session it not connected</returns>
        public TcpSession FindSession(Guid id)
        {
            // Try to find the required session
            return Sessions.TryGetValue(id, out TcpSession result) ? result : null;
        }

        /// <summary>
        /// Register a new session
        /// </summary>
        /// <param name="session">Session to register</param>
        internal void RegisterSession(TcpSession session)
        {
            // Register a new session
            Sessions.TryAdd(session.Id, session);
        }

        /// <summary>
        /// Unregister session by Id
        /// </summary>
        /// <param name="id">Session Id</param>
        internal void UnregisterSession(Guid id)
        {
            // Unregister session by Id
            Sessions.TryRemove(id, out TcpSession _);
        }

        #endregion



        #region Server handlers

        /// <summary>
        /// Handle server starting notification
        /// </summary>
        protected virtual void OnStarting()
        {
            Conn.Status = "Strating";
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Conn)).ConfigureAwait(false);
        }
        /// <summary>
        /// Handle server started notification
        /// </summary>
        protected virtual void OnStarted()
        {
            Conn.Status = "Running";
            Conn.ActiveConnection = true;
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Conn)).ConfigureAwait(false);
        }
        /// <summary>
        /// Handle server stopping notification
        /// </summary>
        protected virtual void OnStopping()
        {
            Conn.Status = " Stopping";
            Conn.ActiveConnection = false;
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Conn)).ConfigureAwait(false);
        }
        /// <summary>
        /// Handle server stopped notification
        /// </summary>
        protected virtual void OnStopped()
        {
            Conn.Status = " Stopped/Deactivated";
            Conn.ActiveConnection = false;
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Conn)).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle session connecting notification
        /// </summary>
        /// <param name="session">Connecting session</param>
        protected virtual void OnConnecting(TcpSession session) { }
        /// <summary>
        /// Handle session connected notification
        /// </summary>
        /// <param name="session">Connected session</param>
        protected virtual void OnConnected(TcpSession session) { }
        /// <summary>
        /// Handle session disconnecting notification
        /// </summary>
        /// <param name="session">Disconnecting session</param>
        protected virtual void OnDisconnecting(TcpSession session) { }
        /// <summary>
        /// Handle session disconnected notification
        /// </summary>
        /// <param name="session">Disconnected session</param>
        protected virtual void OnDisconnected(TcpSession session) { }

        /// <summary>
        /// Handle error notification
        /// </summary>
        /// <param name="error">Socket error code</param>
        protected virtual void OnError(SocketError error) { }

        internal void OnConnectingInternal(TcpSession session) { OnConnecting(session); }
        internal void OnConnectedInternal(TcpSession session) { OnConnected(session); }
        internal void OnDisconnectingInternal(TcpSession session) { OnDisconnecting(session); }
        internal void OnDisconnectedInternal(TcpSession session) { OnDisconnected(session); }

        #endregion

        #region Error handling

        /// <summary>
        /// Send error notification
        /// </summary>
        /// <param name="error">Socket error code</param>
        private void SendError(SocketError error)
        {
            // Skip disconnect errors
            if ((error == SocketError.ConnectionAborted) ||
                (error == SocketError.ConnectionRefused) ||
                (error == SocketError.ConnectionReset) ||
                (error == SocketError.OperationAborted) ||
                (error == SocketError.Shutdown))
                return;

            OnError(error);
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Disposed flag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Acceptor socket disposed flag
        /// </summary>
        public bool IsSocketDisposed { get; private set; } = true;

        // Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposingManagedResources)
        {
            // The idea here is that Dispose(Boolean) knows whether it is
            // being called to do explicit cleanup (the Boolean is true)
            // versus being called due to a garbage collection (the Boolean
            // is false). This distinction is useful because, when being
            // disposed explicitly, the Dispose(Boolean) method can safely
            // execute code using reference type fields that refer to other
            // objects knowing for sure that these other objects have not been
            // finalized or disposed of yet. When the Boolean is false,
            // the Dispose(Boolean) method should not execute code that
            // refer to reference type fields because those objects may
            // have already been finalized."

            if (!IsDisposed)
            {
                if (disposingManagedResources)
                {
                    // Dispose managed resources here...
                    Stop();
                }

                // Dispose unmanaged resources here...

                // Set large fields to null here...

                // Mark as disposed.
                IsDisposed = true;
            }
        }

        #endregion
    }
}
