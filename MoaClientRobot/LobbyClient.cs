using ExitGames.Client.Infrastructure;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using MoaRobotClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MoaClientRobot
{
    public class LobbyClient : LoadBalancingClient
    {
        private readonly Thread updateThread;

        // networking / timing related settings
        internal int intervalDispatch = 10;                 // interval between DispatchIncomingCommands() calls
        internal int lastDispatch = Environment.TickCount;
        internal int intervalSend = 50;                     // interval between SendOutgoingCommands() calls
        internal int lastSend = Environment.TickCount;
        internal int intervalMove = 500;                    // interval for auto-movement - each movement creates an OpRaiseEvent
        internal int lastMove = Environment.TickCount;

        // update control variables for UI
        internal int lastUiUpdate = Environment.TickCount;
        private int intervalUiUpdate = 1000;  // the UI update interval. this demo has a low update interval, cause we update also on event and status change

        public LobbyClient() : base(ConnectionProtocol.Tcp)
        {
            this.updateThread = new Thread(this.UpdateLoop);
            this.updateThread.IsBackground = true;
            this.updateThread.Start();

            this.OnEventAction += LobbyClient_OnEventAction;

            this.MasterServerAddress = "192.168.2.202:4532";
            this.AppId = "f0e09630-c30e-4d9d-8d60-64d91ebf642b";
            this.AppVersion = "1.0";

            // this demo uses the Name Server to connect to the EU region unless you set this.MasterServerAddress
            // setting a MasterServerAddress is required, when you host Photon Servers yourself (using Photon OnPremise)
            // this.MasterServerAddress = "<your server address>";
            bool couldConnect = false;
            if (!string.IsNullOrEmpty(this.MasterServerAddress))
            {
                couldConnect = this.Connect();
            }
            else
            {
                couldConnect = this.ConnectToRegionMaster("eu");
            }
            if (!couldConnect)
            {
                this.DebugReturn(DebugLevel.ERROR, "Can't connect to: " + this.CurrentServerAddress);
            }
        }

        private void LobbyClient_OnEventAction(EventData obj)
        {
            switch (obj.Code)
            {
                case EventCode.GameListUpdate:

                    break;
            }
        }

        /// <summary>
        /// Photon library callback for state changes (connect, disconnect, etc.)
        /// Processed within PhotonPeer.DispatchIncomingCommands()!
        /// </summary>
        public override void OnStatusChanged(StatusCode statusCode)
        {
            base.OnStatusChanged(statusCode);

            if (statusCode == StatusCode.Disconnect && this.DisconnectedCause != DisconnectCause.None)
            {
                DebugReturn(DebugLevel.ERROR, this.DisconnectedCause + " caused a disconnect. State: " + this.State + " statusCode: " + statusCode + ".");
            }
        }

        public override void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case EventCode.AppStats:

                    break;
                case EventCode.LobbyStats:
                    this.RoomInfoList.Keys.ToList().ForEach(roomName =>
                    {
                        var roomProp = this.RoomInfoList[roomName] as RoomInfo;
                        var playerCount = roomProp.PlayerCount;
                        var maxPlayers = roomProp.MaxPlayers;
                        var isOpen = roomProp.IsOpen;

                        if (isOpen && playerCount < maxPlayers)
                        {
                            RoomShared roomShared = new RoomShared();
                            for (int i = playerCount, clientId = 0; i < maxPlayers; ++i, ++clientId)
                            {
                                RobotClient client = new RobotClient(true, clientId, roomShared);
                                client.OpJoinRoom(roomName);
                            }
                        }
                    });
                    break;
                case EventCode.GameListUpdate:
                    // todo: Enter a room automatically.
                    if (photonEvent.Parameters[ParameterCode.GameList] is Hashtable)
                    {
                        var hashtable = photonEvent.Parameters[ParameterCode.GameList] as Hashtable;

                        this.RoomInfoList.Keys.ToList().ForEach(roomName =>
                        {
                            var roomNames = hashtable.Keys.OfType<string>();
                            if (roomNames.Count() > 0 && roomNames.Contains(roomName))
                            {
                                var roomProp = hashtable[roomName] as Hashtable;
                                var playerCount = Convert.ToInt32(roomProp[GamePropertyKey.PlayerCount]);
                                var maxPlayers = Convert.ToInt32(roomProp[GamePropertyKey.MaxPlayers]);
                                var isOpen = Convert.ToBoolean(roomProp[GamePropertyKey.IsOpen]);

                                if (isOpen && playerCount < maxPlayers)
                                {
                                    RoomShared roomShared = new RoomShared();
                                    for (int i = playerCount, clientId = 0; i < maxPlayers; ++i, ++clientId)
                                    {
                                        RobotClient client = new RobotClient(true, clientId, roomShared);

                                        client.OnStateChangeAction += (ClientState obj) =>
                                        {
                                            if(obj == ClientState.JoinedLobby)
                                            {
                                                client.OpJoinRoom(roomName);
                                            }
                                        };
                                    }
                                }
                            }
                        });


                    }

                    break;
            }
            base.OnEvent(photonEvent);
        }

        /// <summary>Endless loop to be run by background thread (ends when app exits).</summary>
        public void UpdateLoop()
        {
            while (true)
            {
                this.Update();
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Update must be called by a gameloop (a single thread), so it can handle
        /// automatic movement and networking.
        /// </summary>
        public virtual void Update()
        {
            if (Environment.TickCount - this.lastDispatch > this.intervalDispatch)
            {
                this.lastDispatch = Environment.TickCount;
                this.loadBalancingPeer.DispatchIncomingCommands();
            }

            if (Environment.TickCount - this.lastSend > this.intervalSend)
            {
                this.lastSend = Environment.TickCount;
                this.loadBalancingPeer.SendOutgoingCommands(); // will send pending, outgoing commands
            }

            if (Environment.TickCount - this.lastMove > this.intervalMove)
            {
                this.lastMove = Environment.TickCount;
            }

            // Update call for windows phone UI-Thread
            if (Environment.TickCount - this.lastUiUpdate > this.intervalUiUpdate)
            {
                this.lastUiUpdate = Environment.TickCount;
            }
        }

    }
}
