using ChatLogic;
using System;
using System.Windows.Forms;

namespace ChatGUI
{
    public partial class ChatSystem : Form
    {
        private Client client = new Client();

        public ChatSystem()
        {
            InitializeComponent();

            client.OnMessageReceived += HandleMessage;
            client.OnLog += HandleLog;
            disconnectButton.Enabled = false;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            string ip = "192.168.18.13";
            int port = 5000;
            string username = usernameInput.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Give your name first!");
                return;
            }

            client.Connect(ip, port);

            client.SendUsername(username);

            usernameInput.Enabled = false;
            connectButton.Enabled = false;
            disconnectButton.Enabled = true;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (client.connected)
            {
                client.Disconnect();

                usernameInput.Enabled = true;
                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
                chatWindow.AppendText("[SYSTEM]: CLIENT HAS DISCONNECTED\n");
            }
            else 
            {
                chatWindow.AppendText("[SYSTEM]: CLIENT NOT CONNECTED\n");
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string msg = messageInput.Text;
            if (!string.IsNullOrWhiteSpace(msg))
            {
                client.SendTcp(msg);
                messageInput.Clear();
            }
        }

        private void HandleMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleMessage(message)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            chatWindow.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            chatWindow.SelectionStart = chatWindow.Text.Length;
            chatWindow.ScrollToCaret();
        }
        private void HandleLog(string log)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleLog(log)));
                return;
            }
            chatWindow.AppendText($"[SYSTEM]: {log}{Environment.NewLine}");
        }
        private void usernameInput_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void ChatSystem_Load(object sender, EventArgs e)
        {

        }
    }
}