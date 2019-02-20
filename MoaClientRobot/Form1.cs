// -----------------------------------------------------------------------
// <copyright file="RobotClient.cs" company="Exit Games GmbH">
//   Loadbalancing Demo for Photon - Copyright (C) 2012 Exit Games GmbH
// </copyright>
// <summary>
//   This project is a simple demo for the DotNet Loadbalancing API and 
//   Photon Cloud usage.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using ExitGames.Client.Infrastructure;

namespace MoaClientRobot
{
    using System;
    using System.Collections;
    using System.Windows.Forms;
    using ExitGames.Client.Photon;
    using ExitGames.Client.Photon.LoadBalancing;

    public partial class Form1 : Form
    {
        private LobbyClient lobby;

        private Random rand = new Random();
        //private Thread updateThread;
        private bool showGame;

        delegate void UpdateViewDelegate();

        public Form1()
        {
            this.InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            RobotClientStarter.Instance.Client.OnUpdate = this.UpdateView;

            this.lobby = new LobbyClient();

            this.lobbyPanel.Visible = true;
            this.gamePanel.Visible = false;
        }
        
        private void UpdateView()
        {
            if (this.availableRoomsListBox.InvokeRequired)
            {
                UpdateViewDelegate d = new UpdateViewDelegate(this.UpdateView);
                this.Invoke(d);
            }
            else
            {
                if (RobotClientStarter.Instance.Client.AppId.Equals("<your appid>"))
                {
                    Console.Out.WriteLine("The appId is not set. Customize your appId in RobotClient.cs. Find help in readme.txt");
                    this.Text = "Error: default appId in use. Customize your appId in RobotClient.cs.";
                }

                this.showGame = RobotClientStarter.Instance.Client.State == ClientState.Joined;

                this.lobbyPanel.Visible = !this.showGame;
                this.gamePanel.Visible = this.showGame;
                
                this.availableRoomsListBox.Items.Clear();
                foreach (var v in RobotClientStarter.Instance.Client.RoomInfoList.Values)
                {
                    this.availableRoomsListBox.Items.Add(v);
                }
                this.roomCountLabel.Text = RobotClientStarter.Instance.Client.RoomInfoList.Values.Count.ToString();

                this.playerListBox.Items.Clear();

                // display the room's properties first
                if (RobotClientStarter.Instance.Client != null && RobotClientStarter.Instance.Client.CurrentRoom != null)
                {
                    // display room's name, player count, event count (something this demo's RobotClient does) and properties
                    this.roomNameLabel.Text = RobotClientStarter.Instance.Client.CurrentRoom.Name;
                    this.roomPlayerCountLabel.Text = RobotClientStarter.Instance.Client.CurrentRoom.PlayerCount.ToString();

                    this.roomPropsLabel.Text = SupportClass.DictionaryToString(RobotClientStarter.Instance.Client.CurrentRoom.CustomProperties);
                    this.eventsCountLabel.Text = RobotClientStarter.Instance.Client.ReceivedCountMeEvents.ToString();
                    
                    // list all players (by keys, which are most likely sorted)
                    foreach (int playerId in RobotClientStarter.Instance.Client.CurrentRoom.Players.Keys)
                    {
                        this.playerListBox.Items.Add(RobotClientStarter.Instance.Client.CurrentRoom.Players[playerId]);
                    }
                }
            }
        }

        #region Button Clicks And Events

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            OnLeaveClicked(sender, e);
            Environment.Exit(0);
        }

        /// <summary>
        /// Creates a room, assigning the name of the input field.
        /// </summary>
        private void OnCreateRoom(object sender, EventArgs e)
        {
            if (RobotClientStarter.Instance.Client.State == ClientState.JoinedLobby)
            {
                // make up some custom properties (key is a string for those)
                Hashtable customGameProperties = new Hashtable() { { "map", "blue" }, { "units", 2 } };

                try
                {
                    // tells the master to create the room and pass on our locally set properties of "this" player
                 // the last parameter makes the custom prop "map" show up in the lobby. "units" won't be in the shown in the lobby
                    bool createRoomResult = RobotClientStarter.Instance.Client.OpCreateRoom("Room_" + rand.Next(0, 10), new RoomOptions() { MaxPlayers = 4, CustomRoomProperties = customGameProperties, CustomRoomPropertiesForLobby = new string[] { "map" } }, null);
                    bool x = createRoomResult;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void OnSelectRoomToJoin(object sender, EventArgs e)
        {
            if (RobotClientStarter.Instance.Client.State == ClientState.JoinedLobby)
            {
                RoomInfo selected = this.availableRoomsListBox.SelectedItem as RoomInfo;
                if (selected == null)
                {
                    return;
                }

                bool joinedRoom = RobotClientStarter.Instance.Client.OpJoinRoom(selected.Name);
                bool x = joinedRoom;
            }
        }

        private void OnLeaveClicked(object sender, EventArgs e)
        {
            RobotClientStarter.Instance.Client.OpLeaveRoom();
        }

        private void OnSendRoomPropClick(object sender, EventArgs e)
        {
            Hashtable customRoomProperties = new Hashtable();
            if (rand.Next(2) > 0)
            {
                customRoomProperties["map"] = "map" + rand.Next(0, 10);
            }
            else
            {
                customRoomProperties["units"] = rand.Next(0, 10);
            }

            RobotClientStarter.Instance.Client.OpSetCustomPropertiesOfRoom(customRoomProperties);
            this.UpdateView();
        }

        private void OnSendPlayerPropClick(object sender, EventArgs e)
        {
            Hashtable customPlayerProps = new Hashtable();
            if (rand.Next(2) > 0)
            {
                customPlayerProps["class"] = "tank" + rand.Next(0, 10);
            }
            else
            {
                customPlayerProps["lvl"] = rand.Next(0, 10);
            }

            RobotClientStarter.Instance.Client.OpSetCustomPropertiesOfActor(RobotClientStarter.Instance.Client.LocalPlayer.ID, customPlayerProps);
            this.UpdateView();
        }

        private void OnSendEventClick(object sender, EventArgs e)
        {
            RobotClientStarter.Instance.Client.SendCountMe();            
        }

        private void OnKeyUpRoomNameInput(object sender, KeyEventArgs e)
        {
            // pass-on the event on "Enter"
            if (e.KeyCode.Equals(Keys.Enter))
            {
                this.OnCreateRoom(sender, e);
            }
        }

        #endregion
    }
}
