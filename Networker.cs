using System.Text;
using System.Net;
using System.Net.Sockets;

namespace lab3GUI
{
    static class Networker
    {
        public const int MAXNAMELEN = 255;

        public const int UDPPORT = 50000;
        public const int TCPPORT = 50001;

        public const int MAXPENDINGCLIENTS = 8;

        static public string myUsername { get; set; }

        static private Socket UDPStartupSocket;
        static private Socket TCPListenSocket;

        static private IPEndPoint localUDPEndPoint { get; }
        static private IPEndPoint localTCPEndPoint { get; }

        static private SocketAsyncEventArgs asyncUDPRecvEventArgs;
        static private SocketAsyncEventArgs asyncTCPAcceptEventArgs;

        static Networker()
        {
            UDPStartupSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            TCPListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);           

            UDPStartupSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            localUDPEndPoint = new IPEndPoint(IPAddress.Any, UDPPORT);
            localTCPEndPoint = new IPEndPoint(IPAddress.Any, TCPPORT);

            //event args for receiving UDP request
            //after receiving a new TCP connection is opened; usertoken is set in appropriate method
            asyncUDPRecvEventArgs = new SocketAsyncEventArgs();
            byte[] buf = new byte[255];
            asyncUDPRecvEventArgs.SetBuffer(buf, 0, MAXNAMELEN);
            asyncUDPRecvEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, UDPPORT);
            asyncUDPRecvEventArgs.Completed += EstablishTCPConnection;

            //event args for event of accepting a TCP connection
            //minimum buffer for accepting initial data size = 288 bytes (see AcceptAsync at MSDN)
            //usertoken is set in appropriate method
            asyncTCPAcceptEventArgs = new SocketAsyncEventArgs();
            asyncTCPAcceptEventArgs.Completed += AcceptTCPConnection;
            asyncTCPAcceptEventArgs.SetBuffer(new byte[288 + MAXNAMELEN], 0, MAXNAMELEN);            

            UDPStartupSocket.Bind(localUDPEndPoint);
            TCPListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            TCPListenSocket.Bind(localTCPEndPoint);
            TCPListenSocket.Listen(MAXPENDINGCLIENTS);
        }

        static public void SendStartupMsg()
        {
            UDPStartupSocket.SendTo(Encoding.ASCII.GetBytes(myUsername), new IPEndPoint(IPAddress.Broadcast, UDPPORT));
        }

        static public void ListenForUDPRequests(MainForm initiator)
        {
            asyncUDPRecvEventArgs.UserToken = initiator;

            // error-checking?
            while (!UDPStartupSocket.ReceiveFromAsync(asyncUDPRecvEventArgs))
            {
                EstablishTCPConnectionSync(UDPStartupSocket, asyncUDPRecvEventArgs);
            }
        }

        static private void EstablishTCPConnection(object Sender, SocketAsyncEventArgs e)
        {
            EstablishTCPConnectionSync(Sender, e);
            //Array.Clear(e.Buffer, 0, e.Buffer.Length);
            ListenForUDPRequests((MainForm)e.UserToken);
        }

        static private void EstablishTCPConnectionSync(object Sender, SocketAsyncEventArgs e)
        {
            // RECIEVE BUFFER IS NOT CLEARED!!! (IDK about sending bufer)

            // cannot compare LocalEndPoint of socket with RemoteEndPoint of SocketAsyncEventArgs
            // bc LocalEndPoint is inaccesible until after "Connect" have been called
            // I guess only username comparing can actually work. UPD: taking all the local interfaces addresses' and comparing
            string peerName = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);
            if (!peerName.Equals(myUsername))
            {
                var connection = new PeerConnection((MainForm)e.UserToken,
                    new IPEndPoint(((IPEndPoint)e.RemoteEndPoint).Address, TCPPORT), peerName);
                // if the same peer is already here, ignore it
                if (!((MainForm)e.UserToken).ContainsPeer(connection))
                {
                    connection.EstablishPeerConnection(localTCPEndPoint);
                }
                else
                {
                    connection.Close();
                }
            }
        }

        static public void ListenForConnections(MainForm initiator)
        {
            // prepare this socket for accepting incoming connections 
            // (to be able to reuse my TCP port setting ReuseAddress option)
            var TCPAcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TCPAcceptSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            asyncTCPAcceptEventArgs.AcceptSocket = TCPAcceptSocket;
            asyncTCPAcceptEventArgs.UserToken = initiator;

            while (!TCPListenSocket.AcceptAsync(asyncTCPAcceptEventArgs))
            {
                AcceptTCPConnectionSync(TCPListenSocket, asyncTCPAcceptEventArgs);
            }
        }

        static private void AcceptTCPConnection(object Sender, SocketAsyncEventArgs e)
        {
            /* The SocketAsyncEventArgs.Completed event can occur in some cases when no connection has been accepted
             * and cause the SocketAsyncEventArgs.SocketError property to be set to ConnectionReset. 
             * This can occur as a result of port scanning using a half-open SYN type scan 
             * (a SYN -> SYN-ACK -> RST sequence).
             * Applications using the AcceptAsync method should be prepared to handle this condition.
             */

            if (e.SocketError == SocketError.Success)
            {
                AcceptTCPConnectionSync(Sender, e);
            }
            ListenForConnections((MainForm)e.UserToken);
        }

        static private void AcceptTCPConnectionSync(object Sender, SocketAsyncEventArgs e)
        {
            var acceptedConnection = new PeerConnection((MainForm)e.UserToken,
                Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred).TrimEnd('\0'), e.AcceptSocket);
            
            if (!((MainForm)e.UserToken).ContainsPeer(acceptedConnection))
            {
                var temp = (MainForm)e.UserToken;
                e.UserToken = acceptedConnection;
                temp.ConnectionEstablished(Sender, e);
                e.UserToken = temp;
            }
            else
            {
                acceptedConnection.Close();
            }
        }
    }
}
