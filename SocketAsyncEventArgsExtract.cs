using System;
using System.Net.Sockets;
using System.Net;

namespace lab3GUI
{
    // used for temporary !COPY! storage for some of SocketAsyncEventArgs' fields
    // to use them later in GUI thread to display event's info.
    // It doesn't use unmanaged resources, so no need in IDisposable implementation
    class SocketAsyncEventArgsExtract
    {
        //buffer's size is equal to BytesTransferred property
        public byte[] buffer { get; }

        // may add LastOperation if needed

        public SocketError errorStatus { get; }
        public IPEndPoint peerIP { get; }
        public string peerName { get; }

        public SocketAsyncEventArgsExtract(byte[] buffer, int start, int length, SocketError error, IPEndPoint IP, string name)
        {
            this.buffer = new byte[length];
            Array.Copy(buffer, start, this.buffer, 0, length);
            this.errorStatus = error;
            this.peerIP = IP;
            this.peerName = name;
        }
    }
}
