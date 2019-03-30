using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace lab3GUI
{
    public class PeerConnection
    {
        public Socket connectionSocket { get; }

        public string peerName { get; }

        public IPEndPoint peerIP { get; }

        public MainForm ownerForm { get; }

        private byte[] sendBuffer;
        private byte[] recvBuffer;
        private SocketAsyncEventArgs recieveEventArgs;
        private SocketAsyncEventArgs sendEventArgs;

        public PeerConnection(MainForm initiator, IPEndPoint destination, string username)
        {
            ownerForm = initiator;
            connectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            peerIP = destination;
            peerName = username;
            sendBuffer = new byte[1024];
            recvBuffer = new byte[1024];

            recieveEventArgs = new SocketAsyncEventArgs();
            recieveEventArgs.SetBuffer(recvBuffer, 0, recvBuffer.Length);
            recieveEventArgs.Completed += ownerForm.MessageReceived;

            sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
            sendEventArgs.Completed += ownerForm.MessageSent;
        }

        public PeerConnection(MainForm initiator, string username, Socket socket)
        {
            ownerForm = initiator;
            connectionSocket = socket;
            peerIP = socket.RemoteEndPoint as IPEndPoint;
            peerName = username;
            sendBuffer = new byte[1024];
            recvBuffer = new byte[1024];

            recieveEventArgs = new SocketAsyncEventArgs();
            recieveEventArgs.SetBuffer(recvBuffer, 0, recvBuffer.Length);
            recieveEventArgs.Completed += ownerForm.MessageReceived;

            sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
            sendEventArgs.Completed += ownerForm.MessageSent;
        }
        
        public void EstablishPeerConnection(IPEndPoint localEP)
        {
            connectionSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            connectionSocket.Bind(localEP);
            byte[] buf = new byte[255];
            Encoding.ASCII.GetBytes(Networker.myUsername, 0, Networker.myUsername.Length, buf, 0);

            // using local variable (not an object field) because after establishing
            // the connection I don't longer need any info from those event args
            var asyncTCPConnectEventArgs = new SocketAsyncEventArgs();

            asyncTCPConnectEventArgs.SetBuffer(buf, 0, Networker.MAXNAMELEN);
            asyncTCPConnectEventArgs.RemoteEndPoint = peerIP;
            asyncTCPConnectEventArgs.Completed += ownerForm.ConnectionEstablished;
            asyncTCPConnectEventArgs.UserToken = this;

            if (!connectionSocket.ConnectAsync(asyncTCPConnectEventArgs))
            {
                ownerForm.ConnectionEstablished(connectionSocket, asyncTCPConnectEventArgs);
            }
        }

        public void SendMessage(string message)
        {
            sendEventArgs.UserToken = this;
            Encoding.ASCII.GetBytes(message, 0, message.Length, sendBuffer, 0);

            while (!connectionSocket.SendAsync(sendEventArgs))
            {
                ownerForm.MessageSent(connectionSocket, sendEventArgs);
            }
            Array.Clear(sendBuffer, 0, sendBuffer.Length);
        }

        public void ReceiveMessage()
        {
            recieveEventArgs.UserToken = this;

            bool res = true;
            while (res && !connectionSocket.ReceiveAsync(recieveEventArgs))
            {
                res = ownerForm.MessageReceivedSync(connectionSocket, recieveEventArgs);
            }
            Array.Clear(recvBuffer, 0, recvBuffer.Length);
        }

        public void Close()
        {
            if (connectionSocket.Connected)
            {
                connectionSocket.Shutdown(SocketShutdown.Both);
            }
            recieveEventArgs.Dispose();
            sendEventArgs.Dispose();
            connectionSocket.Close();
        }

        public override bool Equals(object peerInfo)
        {
            if (!ReferenceEquals(peerInfo, null) && (this.GetType() == peerInfo.GetType()))
            {
                if (ReferenceEquals(this, peerInfo)) return true;

                var temp = (PeerConnection)peerInfo;
                return temp.peerName.Equals(this.peerName) && 
                    this.peerIP.Equals(temp.peerIP);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, connectionSocket) ? connectionSocket.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, peerName) ? peerName.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, ownerForm) ? ownerForm.GetHashCode() : 0);
                return hash;
            }
        }
    }
}
