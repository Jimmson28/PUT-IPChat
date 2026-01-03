namespace ChatGUI
{
    partial class ChatSystem
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatSystem));
            Title = new Label();
            chatWindow = new RichTextBox();
            messageInput = new TextBox();
            button1 = new Button();
            connectButton = new Button();
            usernameInput = new TextBox();
            disconnectButton = new Button();
            SuspendLayout();
            // 
            // Title
            // 
            Title.BackColor = Color.SlateGray;
            Title.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title.Location = new Point(12, 9);
            Title.Name = "Title";
            Title.Size = new Size(260, 42);
            Title.TabIndex = 0;
            Title.Text = "ChatSystem";
            Title.TextAlign = ContentAlignment.TopCenter;
            // 
            // chatWindow
            // 
            chatWindow.BorderStyle = BorderStyle.FixedSingle;
            chatWindow.Font = new Font("Microsoft YaHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chatWindow.ForeColor = Color.Black;
            chatWindow.Location = new Point(278, 9);
            chatWindow.Name = "chatWindow";
            chatWindow.Size = new Size(510, 376);
            chatWindow.TabIndex = 1;
            chatWindow.Text = "";
            // 
            // messageInput
            // 
            messageInput.BorderStyle = BorderStyle.FixedSingle;
            messageInput.Font = new Font("Microsoft YaHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            messageInput.Location = new Point(366, 401);
            messageInput.Multiline = true;
            messageInput.Name = "messageInput";
            messageInput.Size = new Size(422, 44);
            messageInput.TabIndex = 2;
            // 
            // button1
            // 
            button1.BackColor = Color.Gray;
            button1.FlatStyle = FlatStyle.Popup;
            button1.Font = new Font("Microsoft YaHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(278, 401);
            button1.Name = "button1";
            button1.Size = new Size(82, 44);
            button1.TabIndex = 3;
            button1.Text = "Send";
            button1.UseVisualStyleBackColor = false;
            button1.Click += sendButton_Click;
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.OliveDrab;
            connectButton.BackgroundImageLayout = ImageLayout.Stretch;
            connectButton.FlatStyle = FlatStyle.Popup;
            connectButton.Font = new Font("Microsoft YaHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            connectButton.Location = new Point(12, 341);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(260, 44);
            connectButton.TabIndex = 4;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
            // 
            // usernameInput
            // 
            usernameInput.BackColor = Color.White;
            usernameInput.BorderStyle = BorderStyle.FixedSingle;
            usernameInput.Font = new Font("Microsoft YaHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            usernameInput.Location = new Point(12, 63);
            usernameInput.Name = "usernameInput";
            usernameInput.Size = new Size(260, 29);
            usernameInput.TabIndex = 5;
            usernameInput.TextChanged += usernameInput_TextChanged;
            // 
            // disconnectButton
            // 
            disconnectButton.BackColor = Color.IndianRed;
            disconnectButton.BackgroundImageLayout = ImageLayout.Stretch;
            disconnectButton.FlatStyle = FlatStyle.Popup;
            disconnectButton.Font = new Font("Microsoft JhengHei UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            disconnectButton.Location = new Point(12, 401);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(260, 44);
            disconnectButton.TabIndex = 6;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = false;
            disconnectButton.Click += disconnectButton_Click;
            // 
            // ChatSystem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SlateGray;
            ClientSize = new Size(800, 450);
            Controls.Add(disconnectButton);
            Controls.Add(usernameInput);
            Controls.Add(connectButton);
            Controls.Add(button1);
            Controls.Add(messageInput);
            Controls.Add(chatWindow);
            Controls.Add(Title);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ChatSystem";
            Text = "ChatSystem";
            Load += ChatSystem_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title;
        private RichTextBox chatWindow;
        private TextBox messageInput;
        private Button button1;
        private Button connectButton;
        private TextBox usernameInput;
        private Button disconnectButton;
    }
}
