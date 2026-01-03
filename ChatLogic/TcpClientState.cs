using System.Net.Sockets;
using System.Net;

namespace ChatLogic
{
    public class TcpClientState
    {
        public int ClientID { get; set; }
        public NetworkStream NetworkStream { get; set; }
        public byte[] buffer { get; set; }
        public TcpClient TcpClient { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public string Username { get; set; }
    }
}
