using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace lab3GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private const string TextBoxNL = "\r\n";

        static private LinkedList<PeerConnection> peers = new LinkedList<PeerConnection>();
        private delegate void SocketEventDelegate(object Sender, SocketAsyncEventArgs e);
        private delegate void ShowSocketEventInfo(SocketAsyncEventArgsExtract messageInfo);

        private void SendButton_Click(object sender, EventArgs e)
        {
            string message = InputBox.Text.Trim();
            if (message.Length > 0)
            {
                lock (peers)
                {
                    foreach (PeerConnection p in peers)
                    {
                        p.SendMessage(message);
                    }
                }
                MessagesBox.AppendText("You:" + TextBoxNL + message + TextBoxNL);
                InputBox.Text = string.Empty;
                InputBox.Focus();
                // messages logging or something if needed
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (peers)
            {
                foreach (PeerConnection p in peers)
                {
                    p.Close();
                }
            }
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            var usernameInput = new UsernameDia();
            if (usernameInput.ShowDialog(this) == DialogResult.OK)
            {
                // using public textfield of another form is a bit dirty,
                // but it's modal and used only for this purpose, so...
                Networker.myUsername = usernameInput.UsernameInputField.Text;
            } else
            {
                Application.Exit();
                return;
            }
            usernameInput.Dispose();
            StartUpInitiate();
        }

        private void StartUpInitiate()
        {
            try
            {
                Networker.SendStartupMsg();
                MessagesBox.AppendText("UDP request sent, waiting for connections..." + TextBoxNL);
            }
            catch (SocketException exc)
            {
                MessageBox.Show(this, "Exception was raised when tried to send UDP broadcast message:" + TextBoxNL + exc.ToString());
                Application.Exit();
                return;
            }
            // we won't miss any of incoming TCP connections if "ListenForUDPRequests" takes too long
            // (for example in the case of UDP spamming from other clients),
            // because TCP socket is already in listening state and will hold incoming connections
            // until we accept them
            Networker.ListenForUDPRequests(this);
            Networker.ListenForConnections(this);
        }

        public void ConnectionEstablished(object Sender, SocketAsyncEventArgs e)
        {
            // InvokeRequired messes things up. I need to copy contents of SocketAsyncEventArgs
            // somewhere, so the GUI thread can be sure SocketAsyncEventArgs is not disposed before it tries
            // to read and interpret it's contents. See the same pattern below
            var connectionMessageInfo = new SocketAsyncEventArgsExtract(e.Buffer,
                    e.Offset, e.BytesTransferred, e.SocketError, ((PeerConnection)e.UserToken).peerIP,
                    ((PeerConnection)e.UserToken).peerName);

            if (this.InvokeRequired)
            {
                this.Invoke(new ShowSocketEventInfo(ShowConnectionMessage), connectionMessageInfo);
            }
            else
            {
                ShowConnectionMessage(connectionMessageInfo);
            }

            if (e.SocketError == SocketError.Success)
            {
                AddPeer((PeerConnection)e.UserToken);
                ((PeerConnection)e.UserToken).ReceiveMessage();
            }
            else
            {
                ((PeerConnection)e.UserToken).Close();
            }
        }

        private void ShowConnectionMessage(SocketAsyncEventArgsExtract messageInfo)
        {
            long connectionStatus = (long)messageInfo.errorStatus;
            if (connectionStatus == 0)
            {
                MessagesBox.AppendText($"Connection with {messageInfo.peerName} @ " +
                    $"({messageInfo.peerIP.Address.ToString()})" + $":{messageInfo.peerIP.Port.ToString()} " +
                    "is established" + TextBoxNL);

                SendButton.Enabled = true;
            }
            else
            {
                MessagesBox.AppendText($"Connection with {messageInfo.peerName} " +
                    $"({messageInfo.peerIP.Address.ToString()}) "
                    + $"has failed with code {connectionStatus}" + TextBoxNL);
            }
        }

        public void MessageReceived(object Sender, SocketAsyncEventArgs e)
        {
            if (MessageReceivedSync(Sender, e))
            {
                ((PeerConnection)e.UserToken).ReceiveMessage();
            }
        }

        // false if some connection error occured. False indicates that we can't recieve messages anymore, so need to stop
        public bool MessageReceivedSync(object Sender, SocketAsyncEventArgs e)
        {
            var messageRecievedMessageInfo = new SocketAsyncEventArgsExtract(e.Buffer,
                    e.Offset, e.BytesTransferred, e.SocketError, (IPEndPoint)e.RemoteEndPoint, ((PeerConnection)e.UserToken).peerName);

            if (this.InvokeRequired)
            {
                this.Invoke(new ShowSocketEventInfo(ShowMessageRecievedMessage), messageRecievedMessageInfo);
            }
            else
            {
                ShowMessageRecievedMessage(messageRecievedMessageInfo);
            }

            switch (e.SocketError)
            {
                case (SocketError.Success):
                    if (e.BytesTransferred == 0)
                    {
                        ((PeerConnection)e.UserToken).Close();
                        RemovePeer((PeerConnection)e.UserToken);
                        return false;
                    }
                    break;
                default:
                    ((PeerConnection)e.UserToken).Close();
                    RemovePeer((PeerConnection)e.UserToken);
                    return false;
            }
            return true;
        }

        private void ShowMessageRecievedMessage(SocketAsyncEventArgsExtract messageInfo)
        {
            switch (messageInfo.errorStatus)
            {
                case (SocketError.Success):
                    if (messageInfo.buffer.Length == 0)
                    {
                        MessagesBox.AppendText($"{messageInfo.peerName} gracefully disconnected!" + TextBoxNL);
                    }
                    else
                    {
                        string recvdMessage = Encoding.ASCII.GetString(messageInfo.buffer).TrimEnd('\0');
                        if (recvdMessage.Length > 0)
                        {
                            MessagesBox.AppendText($"{messageInfo.peerName} says:" + TextBoxNL +
                                Encoding.ASCII.GetString(messageInfo.buffer).TrimEnd('\0') + TextBoxNL);
                        }
                    }
                    break;
                // show appropriate messages in other cases
                case (SocketError.Disconnecting):
                    MessagesBox.AppendText($"{messageInfo.peerName} gracefully disconnected!" + TextBoxNL);
                    break;
                case (SocketError.ConnectionReset):
                    MessagesBox.AppendText($"{messageInfo.peerName} roughly disconnected (possible problems with network)!" + TextBoxNL);
                    break;
                case (SocketError.NetworkDown):
                    MessagesBox.AppendText($"Network is down! {messageInfo.peerName} is disconnected!" + TextBoxNL);
                    break;
                case (SocketError.TimedOut):
                    MessagesBox.AppendText($"{messageInfo.peerName} timed out!" + TextBoxNL);
                    break;
                case (SocketError.SocketError):
                    MessagesBox.AppendText($"Unspecified socket error occured with {messageInfo.peerName}." +
                        $" Disconnecting peer." + TextBoxNL);
                    break;
                default:
                    MessagesBox.AppendText($"Connection with {messageInfo.peerName} has failed with code " +
                        (long)messageInfo.errorStatus + TextBoxNL);
                    break;
            }
        }

        public void MessageSent(object Sender, SocketAsyncEventArgs e)
        {
            var messageSentMessageInfo = new SocketAsyncEventArgsExtract(e.Buffer,
                e.Offset, e.BytesTransferred, e.SocketError, (IPEndPoint)e.RemoteEndPoint, ((PeerConnection)e.UserToken).peerName);

            if (this.InvokeRequired)
            {
                this.Invoke(new ShowSocketEventInfo(ShowMessageSentMessage), messageSentMessageInfo);
            }
            else
            {
                ShowMessageSentMessage(messageSentMessageInfo);
            }

            switch (e.SocketError)
            {
                case (SocketError.Success):
                    break;
                default:
                    ((PeerConnection)e.UserToken).Close();
                    RemovePeer((PeerConnection)e.UserToken);
                    break;
            }
        }

        private void ShowMessageSentMessage(SocketAsyncEventArgsExtract messageInfo)
        {
            switch (messageInfo.errorStatus)
            {
                case (SocketError.Success):
                    break;
                case (SocketError.Disconnecting):
                    MessagesBox.AppendText($"Cannot send message to {messageInfo.peerName}: disconnection in progress" + TextBoxNL);
                    break;
                case (SocketError.ConnectionReset):
                    MessagesBox.AppendText($"Cannot send message to {messageInfo.peerName}: peer roughly disconnected!"+ TextBoxNL);
                    break;
                case (SocketError.NetworkDown):
                    MessagesBox.AppendText($"Network is down! {messageInfo.peerName} is disconnected!" + TextBoxNL);
                    break;
                case (SocketError.TimedOut):
                    MessagesBox.AppendText($"{messageInfo.peerName} timed out!" + TextBoxNL);
                    break;
                case (SocketError.SocketError):
                    MessagesBox.AppendText($"Unspecified socket error occured with {messageInfo.peerName}." +
                        $" Disconnecting peer." + TextBoxNL);
                    break;
                default:
                    MessagesBox.AppendText($"Connection with {messageInfo.peerName} has failed with code " +
                        (long)messageInfo.errorStatus + TextBoxNL);
                    break;
            }
        }

        // THREAD SAFETY
        public void AddPeer(PeerConnection newPeer)
        {
            lock (peers)
            {
                peers.AddFirst(newPeer);
            }
        }

        // THREAD SAFETY
        public void RemovePeer(PeerConnection peer)
        {
            lock (peers)
            {
                peers.Remove(peer);
            }
        }

        // THREAD SAFETY
        public bool ContainsPeer(PeerConnection peer)
        {
            lock (peers)
            {
                foreach (PeerConnection p in peers)
                {
                    if (p.Equals(peer))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
