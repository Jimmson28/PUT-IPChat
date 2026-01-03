using System;
using System.Net;
using System.Net.Sockets;

namespace ChatLogic
{
    public class Client
    {
        private UdpClient? udpClient = null;
        private TcpClient? tcpClient = null;
        private NetworkStream? stream = null;
        private byte[]? receiverBuffer = null;
        public event Action<string>? OnMessageReceived;
        public event Action<string>? OnLog;
        public bool connected = false;
        public string username = null;
        public void Connect(string ip, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);
                stream = tcpClient.GetStream();
                receiverBuffer = new byte[4096];
                stream.BeginRead(receiverBuffer, 0, receiverBuffer.Length, OnReceiveDataWithTCP, null);
                
                udpClient = new UdpClient();
                udpClient.Connect(ip, port);
                udpClient.BeginReceive(OnReceiveDataWithUDP, null);

                OnLog?.Invoke($"Connected to server {ip} : {port}");
                connected = true;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("Connection Error: " + ex.Message);
            }
        }
        public void Disconnect()
        {
            try
            {
                udpClient?.Close();
                tcpClient?.Close();
                stream?.Close();
                OnLog?.Invoke("Client has disconnected.");
                connected = false;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("Disconeccting Error: " + ex.Message);
            }
        }
        public void SendTcp(string message)
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                OnLog?.Invoke("Connection Error TCP!");
                return;
            }
            try
            {
                using (Packet packet = new Packet())
                {
                    packet.writeInt(1);
                    packet.writeString(message);
                    var data = packet.getBytesArray();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("Sending Error TCP: " + ex.Message);
            }
        }
        public void SendUdp(string message)
        {
            if (udpClient == null)
            {
                OnLog?.Invoke("There is no UDP Client!");
                return;
            }
            try
            {
                using (Packet packet = new Packet())
                {
                    packet.writeInt(1);
                    packet.writeString(message);
                    var data = packet.getBytesArray();
                    udpClient.Send(data, data.Length);
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("Error durnig sending via UDP: " + ex.Message);
            }
        }
        private void OnReceiveDataWithTCP(IAsyncResult result)
        {
            try
            {
                if (stream == null) return;

                int bytesRead = stream.EndRead(result);
                if (bytesRead > 0)
                {
                    byte[] data = new byte[bytesRead];
                    Array.Copy(receiverBuffer, data, bytesRead);

                    using (Packet packet = new Packet(data))
                    {
                        int id = packet.readInt();
                        if (id == 1)
                        {
                            string message = packet.readString();
                            OnMessageReceived?.Invoke($"[TCP]: {message}");
                        }
                    }                 
                    stream.BeginRead(receiverBuffer, 0, receiverBuffer.Length, OnReceiveDataWithTCP, null);
                }
                else
                {
                    OnLog?.Invoke("Server has lost connection via TCP.");
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                if (stream != null && stream.CanRead)
                {
                    OnLog?.Invoke("Receiving Error TCP: " + ex.Message);
                }
            }
        }
        private void OnReceiveDataWithUDP(IAsyncResult result)
        {
            try
            {
                if (udpClient == null) return;

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpClient.EndReceive(result, ref remoteEndPoint);

                using (Packet packet = new Packet(receivedData))
                {
                    int id = packet.readInt();
                    if (id == 1)
                    {
                        string message = packet.readString();
                        OnMessageReceived?.Invoke($"[UDP]: {message}");
                    }
                }
                udpClient.BeginReceive(OnReceiveDataWithUDP, null);
            }
            catch (ObjectDisposedException)
            {
                //
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("Receiving Error UDP: " + ex.Message);
            }
        }
        public void SendUsername(string name)
        {
            if (tcpClient == null || !tcpClient.Connected) return;
            try
            {
                using (Packet packet = new Packet())
                {
                    packet.writeInt(2);
                    packet.writeString(name);
                    var data = packet.getBytesArray();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("Error sending name of the user: " + ex.Message);
            }
        }
    }
}