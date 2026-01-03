using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ChatLogic
{
    public class Server
    {
        private TcpListener tcpListener = null;
        private UdpClient udpListener = null;
        private bool running = false;
        private Dictionary<int, TcpClientState> tcpClients = new Dictionary<int, TcpClientState>();
        private int nextClientID = 1;
        private object tcpClientLock = new object();

        public void start(ushort port)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(OnAcceptTcpClient, null);

                udpListener = new UdpClient(port);
                udpListener.BeginReceive(OnReceiveDataWithUDP, null);

                running = true;

                Console.WriteLine($"Server started on port {port}.");
                Console.WriteLine("Press 'X' to shut down.");

                while (running)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.X)
                    {
                        running = false;
                    }                    
                    else if (key.Key == ConsoleKey.T)
                    {
                        sendToAllViaTCP();
                    }
                    else if (key.Key == ConsoleKey.U)
                    {
                        sendToAllViaUDP();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server error: " + ex.ToString());
            }
            finally
            {
                Console.WriteLine("Shutting down server...");
                lock (tcpClientLock)
                {
                    foreach (var client in tcpClients.Values)
                    {
                        client.TcpClient.Close();
                    }
                    tcpClients.Clear();
                }
                udpListener?.Close();
                tcpListener?.Stop();
            }
        }
        private void OnAcceptTcpClient(IAsyncResult result)
        {
            if (!running) return;
            try
            {
                TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);
                NetworkStream networkStream = tcpClient.GetStream();

                int ClientID;
                lock (tcpClients)
                {
                    ClientID = nextClientID++;
                }

                var endPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"Client connected (ID: {ClientID})");

                TcpClientState state = new TcpClientState();
                state.NetworkStream = networkStream;
                state.TcpClient = tcpClient;
                state.buffer = new byte[4096];
                state.ClientID = ClientID;
                state.EndPoint = null;

                lock (tcpClients)
                {
                    tcpClients.Add(ClientID, state);
                }

                networkStream.BeginRead(state.buffer, 0, state.buffer.Length, OnReceiveDataWithTCP, state);
                tcpListener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
            }
            catch { }
        }
        private void OnReceiveDataWithTCP(IAsyncResult result)
        {
            TcpClientState state = (TcpClientState)result.AsyncState;
            try
            {
                if (state.TcpClient == null || !state.TcpClient.Connected) return;

                int bytesRead = state.NetworkStream.EndRead(result);
                if (bytesRead > 0)
                {
                    byte[] data = new byte[bytesRead];
                    Array.Copy(state.buffer, data, bytesRead);

                    using (Packet packet = new Packet(data))
                    {
                        int id = packet.readInt();
                        if (id == 1)
                        {
                            string message = packet.readString();
                            Console.WriteLine($"[MESSAGE] ID {state.ClientID}: {message}");
                            string senderName = !string.IsNullOrEmpty(state.Username) ? state.Username : $"Client {state.ClientID}";
                            string messageToSend = $"{senderName}: {message}";

                            BroadcastTcpMessage(messageToSend);
                        }
                        else if (id == 2)
                        {
                            string username = packet.readString();
                            state.Username = username;
                            Console.WriteLine($"[INFORMATION FROM SERVER] Client {state.ClientID} choosed username: {username}");
                            BroadcastTcpMessage($"SERVER: {username} Joined to chat.");
                        }
                    }
                    state.NetworkStream.BeginRead(state.buffer, 0, state.buffer.Length, OnReceiveDataWithTCP, state);
                }
                else
                {
                    DisconnectClient(state);
                }
            }
            catch (Exception)
            {
                DisconnectClient(state);
            }
        }
        private void OnReceiveDataWithUDP(IAsyncResult result)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref endPoint);
                AssociateUdpEndPoint(endPoint);

                using (Packet packet = new Packet(data))
                {
                    int id = packet.readInt();
                    if (id == 1)
                    {
                        string message = packet.readString();
                        Console.WriteLine($"[UDP] Received: {message}");
                        int senderId = GetClientIdByEndPoint(endPoint);
                        string senderName = senderId != -1 ? $"Klient {senderId}" : "Unknown";

                        string messageToSend = $"[UDP] {senderName}: {message}";
                        BroadcastUdpMessage(messageToSend);
                    }
                }
                udpListener.BeginReceive(OnReceiveDataWithUDP, null);
            }
            catch (ObjectDisposedException) 
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error UDP: " + ex.Message);
            }
        }
        private void BroadcastTcpMessage(string message)
        {
            using (Packet packetToSend = new Packet())
            {
                packetToSend.writeInt(1);
                packetToSend.writeString(message);
                byte[] dataToSend = packetToSend.getBytesArray();

                lock (tcpClients)
                {
                    foreach (var otherClient in tcpClients.Values)
                    {
                        try
                        {
                            otherClient.NetworkStream.Write(dataToSend, 0, dataToSend.Length);
                        }
                        catch 
                        { 
                            //
                        }
                    }
                }
            }
        }
        private void BroadcastUdpMessage(string message)
        {
            using (Packet packetToSend = new Packet())
            {
                packetToSend.writeInt(1);
                packetToSend.writeString(message);
                byte[] dataToSend = packetToSend.getBytesArray();

                lock (tcpClients)
                {
                    foreach (var client in tcpClients.Values)
                    {
                        if (client.EndPoint != null)
                        {
                            udpListener.BeginSend(dataToSend, dataToSend.Length, client.EndPoint, null, null);
                        }
                    }
                }
            }
        }
        private void DisconnectClient(TcpClientState state)
        {
            removeTcpClient(state.ClientID);
            Console.WriteLine($"Client {state.ClientID} disconnected.");
            state.TcpClient.Close();
        }
        private void removeTcpClient(int clientId)
        {
            lock (tcpClients)
            {
                if (tcpClients.ContainsKey(clientId))
                {
                    tcpClients.Remove(clientId);
                }
            }
        }
        private void AssociateUdpEndPoint(IPEndPoint endPoint)
        {
            lock (tcpClients)
            {
                foreach (var client in tcpClients.Values)
                {
                    if (client.EndPoint != null) continue;

                    IPEndPoint tcpEndPoint = (IPEndPoint)client.TcpClient.Client.RemoteEndPoint;
                    if (tcpEndPoint.Address.Equals(endPoint.Address))
                    {
                        client.EndPoint = endPoint;
                        Console.WriteLine($"UDP Linked to Client {client.ClientID}");
                        break;
                    }
                }
            }
        }
        private int GetClientIdByEndPoint(IPEndPoint ep)
        {
            lock (tcpClients)
            {
                foreach (var c in tcpClients.Values)
                {
                    if (c.EndPoint != null && c.EndPoint.ToString() == ep.ToString())
                        return c.ClientID;
                }
            }
            return -1;
        }
        private void sendToAllViaTCP()
        {
            Console.Write("Server TCP > ");
            string msg = Console.ReadLine();
            BroadcastTcpMessage("SERVER: " + msg);
        }
        private void sendToAllViaUDP()
        {
            Console.Write("Server UDP > ");
            string msg = Console.ReadLine();
            BroadcastUdpMessage("SERVER: " + msg);
        }
    }
}