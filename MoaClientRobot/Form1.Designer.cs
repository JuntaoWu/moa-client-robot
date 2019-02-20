namespace MoaClientRobot
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lobbyPanel = new System.Windows.Forms.Panel();
            this.spi = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.roomNameInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.roomCountLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.availableRoomsListBox = new System.Windows.Forms.ListBox();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.eventsCountLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.roomPropsLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.playerListBox = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.roomPlayerCountLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.roomNameLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lobbyPanel.SuspendLayout();
            this.gamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lobbyPanel
            // 
            this.lobbyPanel.Controls.Add(this.spi);
            this.lobbyPanel.Controls.Add(this.label4);
            this.lobbyPanel.Controls.Add(this.roomNameInput);
            this.lobbyPanel.Controls.Add(this.label2);
            this.lobbyPanel.Controls.Add(this.button1);
            this.lobbyPanel.Controls.Add(this.roomCountLabel);
            this.lobbyPanel.Controls.Add(this.label1);
            this.lobbyPanel.Controls.Add(this.availableRoomsListBox);
            this.lobbyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lobbyPanel.Location = new System.Drawing.Point(0, 0);
            this.lobbyPanel.Name = "lobbyPanel";
            this.lobbyPanel.Size = new System.Drawing.Size(370, 269);
            this.lobbyPanel.TabIndex = 0;
            // 
            // spi
            // 
            this.spi.Location = new System.Drawing.Point(276, 33);
            this.spi.Name = "spi";
            this.spi.Size = new System.Drawing.Size(85, 23);
            this.spi.TabIndex = 7;
            this.spi.Text = "SetPlayerInfo";
            this.spi.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Doubleclick to join a room.";
            // 
            // roomNameInput
            // 
            this.roomNameInput.Location = new System.Drawing.Point(171, 6);
            this.roomNameInput.Name = "roomNameInput";
            this.roomNameInput.Size = new System.Drawing.Size(100, 20);
            this.roomNameInput.TabIndex = 5;
            this.roomNameInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpRoomNameInput);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(283, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Create room";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnCreateRoom);
            // 
            // roomCountLabel
            // 
            this.roomCountLabel.AutoSize = true;
            this.roomCountLabel.Location = new System.Drawing.Point(55, 47);
            this.roomCountLabel.Name = "roomCountLabel";
            this.roomCountLabel.Size = new System.Drawing.Size(13, 13);
            this.roomCountLabel.TabIndex = 2;
            this.roomCountLabel.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Rooms:";
            // 
            // availableRoomsListBox
            // 
            this.availableRoomsListBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.availableRoomsListBox.FormattingEnabled = true;
            this.availableRoomsListBox.Location = new System.Drawing.Point(0, 83);
            this.availableRoomsListBox.Name = "availableRoomsListBox";
            this.availableRoomsListBox.Size = new System.Drawing.Size(370, 186);
            this.availableRoomsListBox.TabIndex = 0;
            this.availableRoomsListBox.DoubleClick += new System.EventHandler(this.OnSelectRoomToJoin);
            // 
            // gamePanel
            // 
            this.gamePanel.Controls.Add(this.eventsCountLabel);
            this.gamePanel.Controls.Add(this.label8);
            this.gamePanel.Controls.Add(this.roomPropsLabel);
            this.gamePanel.Controls.Add(this.label6);
            this.gamePanel.Controls.Add(this.button5);
            this.gamePanel.Controls.Add(this.button4);
            this.gamePanel.Controls.Add(this.button3);
            this.gamePanel.Controls.Add(this.playerListBox);
            this.gamePanel.Controls.Add(this.button2);
            this.gamePanel.Controls.Add(this.roomPlayerCountLabel);
            this.gamePanel.Controls.Add(this.label5);
            this.gamePanel.Controls.Add(this.roomNameLabel);
            this.gamePanel.Controls.Add(this.label3);
            this.gamePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gamePanel.Location = new System.Drawing.Point(0, 0);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(370, 269);
            this.gamePanel.TabIndex = 1;
            // 
            // eventsCountLabel
            // 
            this.eventsCountLabel.AutoSize = true;
            this.eventsCountLabel.Location = new System.Drawing.Point(205, 67);
            this.eventsCountLabel.Name = "eventsCountLabel";
            this.eventsCountLabel.Size = new System.Drawing.Size(13, 13);
            this.eventsCountLabel.TabIndex = 12;
            this.eventsCountLabel.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(112, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Events received:";
            // 
            // roomPropsLabel
            // 
            this.roomPropsLabel.AutoSize = true;
            this.roomPropsLabel.Location = new System.Drawing.Point(53, 17);
            this.roomPropsLabel.Name = "roomPropsLabel";
            this.roomPropsLabel.Size = new System.Drawing.Size(79, 13);
            this.roomPropsLabel.TabIndex = 10;
            this.roomPropsLabel.Text = "room properties";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Props:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(277, 37);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 8;
            this.button5.Text = "event";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.OnSendEventClick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(196, 37);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "player prop";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnSendPlayerPropClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(115, 37);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "room prop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnSendRoomPropClick);
            // 
            // playerListBox
            // 
            this.playerListBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.playerListBox.FormattingEnabled = true;
            this.playerListBox.Location = new System.Drawing.Point(0, 83);
            this.playerListBox.Name = "playerListBox";
            this.playerListBox.Size = new System.Drawing.Size(370, 186);
            this.playerListBox.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 37);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "leave";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnLeaveClicked);
            // 
            // roomPlayerCountLabel
            // 
            this.roomPlayerCountLabel.AutoSize = true;
            this.roomPlayerCountLabel.Location = new System.Drawing.Point(53, 67);
            this.roomPlayerCountLabel.Name = "roomPlayerCountLabel";
            this.roomPlayerCountLabel.Size = new System.Drawing.Size(13, 13);
            this.roomPlayerCountLabel.TabIndex = 3;
            this.roomPlayerCountLabel.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Players:";
            // 
            // roomNameLabel
            // 
            this.roomNameLabel.AutoSize = true;
            this.roomNameLabel.Location = new System.Drawing.Point(53, 4);
            this.roomNameLabel.Name = "roomNameLabel";
            this.roomNameLabel.Size = new System.Drawing.Size(56, 13);
            this.roomNameLabel.TabIndex = 1;
            this.roomNameLabel.Text = "roomname";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "In room:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 269);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.lobbyPanel);
            this.Name = "Form1";
            this.Text = "Demo LoadBalancing";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Load += new System.EventHandler(this.OnLoad);
            this.lobbyPanel.ResumeLayout(false);
            this.lobbyPanel.PerformLayout();
            this.gamePanel.ResumeLayout(false);
            this.gamePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel lobbyPanel;
        private System.Windows.Forms.ListBox availableRoomsListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label roomCountLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label roomNameLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox playerListBox;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox roomNameInput;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label roomPropsLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label roomPlayerCountLabel;
        private System.Windows.Forms.Label eventsCountLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button spi;
    }
}

