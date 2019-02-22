// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RobotClient.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The RobotClient class wraps up usage of a LoadBalancingClient, event handling and simple game logic. To make it work,
//   it must be integrated in a "UpdateLoop", which regularly calls Update().
//   A PhotonPeer, which is the basis of the underlying LoadBalancingPeer is not thread safe, so make sure Update()
//   is only called by one thread.
//
//   This sample should show how to get a player's position across to other players in the same room.
//   Each running instance will connect to PhotonCloud (with a local player / Peer), go into the same room and
//   move around.
//   Players have positions (updated regularly), name and color (updated only when someone joins).
//   This class encapsulates the (simple!) logic for the Realtime Demo run in der PhotonCloud via LoadBalancing. It can be used on several
//   DotNet platforms (DotNet, Unity3D and Silverlight).
// </summary>
// <author>developer@photonengine.com</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ExitGames.Client.Infrastructure
{
    using System;
    using System.Collections;
    using System.Threading;
    using MoaClientRobot;
    using ExitGames.Client.Photon;
    using ExitGames.Client.Photon.LoadBalancing;
    using System.Linq;
    using System.Collections.Generic;
    using MoaRobotClient;

    /// <summary>
    /// The RobotClient class wraps up usage of a LoadBalancingClient, event handling and simple game logic.
    /// </summary>
    public class RobotClient : LoadBalancingClient
    {
        #region Fields
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
        public Action OnUpdate { get; set; }

        public int ReceivedCountMeEvents { get; set; }

        #endregion

        private int clientId;
        private RoomShared roomShared;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoClient"/> class.
        /// </summary>
        public RobotClient(bool createGameLoopThread, int clientId, RoomShared roomShared = null) : base(ConnectionProtocol.Tcp)
        {
            this.roomShared = roomShared;
            this.clientId = clientId;
            this.MasterServerAddress = "192.168.2.202:4532";

            //this.loadBalancingPeer.DebugOut = DebugLevel.INFO;
            //this.loadBalancingPeer.TrafficStatsEnabled = true;
            if (createGameLoopThread)
            {
                this.updateThread = new Thread(this.UpdateLoop);
                this.updateThread.IsBackground = true;
                this.updateThread.Start();
            }

            this.NickName = "Player_" + this.clientId;
            this.LocalPlayer.SetCustomProperties(new Hashtable() { { "class", "tank" + (SupportClass.ThreadSafeRandom.Next() % 99) }, { "nickName", this.NickName } });


            // insert your game's AppID (replace <your appid>). hosting yourself: use any name. using Photon Cloud: use your cloud subscription's appID
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

        /// <summary>
        /// Overriding CreatePlayer from LoadBalancingClient to return the RobotPlayer which holds
        /// the needed position and color data.
        /// </summary>
        protected override Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
        {
            RobotPlayer tmpPlayer = null;
            if (this.CurrentRoom != null)
            {
                tmpPlayer = (RobotPlayer)this.CurrentRoom.GetPlayer(actorNumber);
            }

            if (tmpPlayer == null)
            {
                tmpPlayer = new RobotPlayer(actorName, actorNumber, isLocal);
                tmpPlayer.InternalCacheProperties(actorProperties);

                if (this.CurrentRoom != null)
                {
                    this.CurrentRoom.StorePlayer(tmpPlayer);
                }
            }
            else
            {
                this.DebugReturn(DebugLevel.ERROR, "Player already listed: " + actorNumber);
            }

            return tmpPlayer;
        }

        #endregion

        #region Send data & update

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
                if (this.State == ClientState.Joined)
                {
                    ((RobotPlayer)LocalPlayer).MoveRandom();
                    this.SendPosition();
                }
            }

            // Update call for windows phone UI-Thread
            if (Environment.TickCount - this.lastUiUpdate > this.intervalUiUpdate)
            {
                this.lastUiUpdate = Environment.TickCount;
                if (this.OnUpdate != null)
                {
                    this.OnUpdate();
                }
            }
        }

        /// <summary>
        /// Send an event to all other players telling them you just have attacked
        /// and their health must be decreased by one.
        /// </summary>
        public void SendPosition()
        {
            // dont move if player does not have a number or peer is not connected
            if (this.LocalPlayer == null || this.LocalPlayer.ID == 0)
            {
                return;
            }

            ((RobotPlayer)this.LocalPlayer).SendEvMove(this.loadBalancingPeer);
        }

        /// <summary>
        /// Will create and queue the operation OpRaiseEvent with local player's color and name (not position).
        /// Actually sent by a call to SendOutgoingCommands().
        /// At this point, we could also use properties (so we don't have to re-send this data when someone joins).
        /// </summary>
        public void SendPlayerInfo()
        {
            // dont move if player does not have a number or Peer is not connected
            if (this.LocalPlayer == null || this.LocalPlayer.ID == 0)
            {
                return;
            }

            ((RobotPlayer)this.LocalPlayer).SendPlayerInfo(this.loadBalancingPeer);
        }

        // Used to measure RTT of CountMe event. Supports only 1 event transmitted at a time.
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public void SendCountMe()
        {
            // to send an event, "raise" it. apply any code (here 3) and set any content (or even null)
            Hashtable eventContent = new Hashtable();
            eventContent[(byte)10] = "my data";                     // using bytes as event keys is most efficient

            RobotClientStarter.Instance.Client.loadBalancingPeer.OpRaiseEvent((byte)RobotPlayer.DemoEventCode.CountMe, eventContent, false, new RaiseEventOptions() { Receivers = ReceiverGroup.All });    // this is received by OnEvent()
            RobotClientStarter.Instance.Client.loadBalancingPeer.SendOutgoingCommands();
            stopwatch.Reset();
            stopwatch.Start();
        }
        #endregion

        #region Event handling

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

            if (this.OnUpdate != null)
            {
                this.OnUpdate();
            }
        }

        /// <summary>
        /// Called by Photon lib for each incoming event (player- and position-data in this demo, as well as joins and leaves).
        /// Processed within PhotonPeer.DispatchIncomingCommands()!
        /// </summary>
        public override void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (byte)RobotPlayer.DemoEventCode.PlayerInfo:

                    // get the player that raised this event
                    int actorNr = (int)photonEvent[ParameterCode.ActorNr];
                    RobotPlayer p;
                    p = (RobotPlayer)this.CurrentRoom.GetPlayer(actorNr);

                    // this is a custom event, which is defined by this application.
                    // if player is known (and it should be known!), update info
                    if (p != null)
                    {
                        p.SetInfo((Hashtable)photonEvent[(byte)ParameterCode.CustomEventContent]);
                    }
                    else
                    {
                        this.DebugReturn("did not find player to set info: " + actorNr);
                    }

                    break;

                case (byte)RobotPlayer.DemoEventCode.PlayerMove:

                    // get the player that raised this event
                    actorNr = (int)photonEvent[ParameterCode.ActorNr];
                    p = (RobotPlayer)this.CurrentRoom.GetPlayer(actorNr);

                    // this is a custom event, which is defined by this application.
                    // if player is known (and it should be known) update position)
                    if (p != null)
                    {
                        p.SetPosition((Hashtable)photonEvent[(byte)ParameterCode.CustomEventContent]);
                    }
                    else
                    {
                        this.DebugReturn("did not find player to move: " + actorNr);
                    }

                    break;

                case (byte)RobotPlayer.DemoEventCode.CountMe:
                    if ((int)photonEvent[ParameterCode.ActorNr] == this.LocalPlayer.ID)
                    {
                        this.DebugReturn("RTT: " + stopwatch.ElapsedMilliseconds + " (" + this.loadBalancingPeer.UsedProtocol + ", " + (System.Diagnostics.Stopwatch.IsHighResolution ? "high" : "low") + " resolution)");
                        if (this.loadBalancingPeer.TrafficStatsEnabled)
                        {
                            this.DebugReturn("Stats:" + this.loadBalancingPeer.VitalStatsToString(true));
                        }
                    }
                    // this event has no content. we just count it if it's sent                    
                    this.ReceivedCountMeEvents++;
                    break;

                case EventCode.Join:

                    // the new peer does not have our info, so send it again);
                    // everything else is handeled by the base-class
                    ((RobotPlayer)LocalPlayer).SendPlayerInfo(this.loadBalancingPeer);

                    // TakeSeat after join in.
                    this.TakeSeat();
                    break;
                case (byte)CustomPhotonEvents.StartChoosingRole:

                    // ChooseRole after received StartChoosingRole.
                    this.ChooseRole();
                    break;

                case (byte)CustomPhotonEvents.UpdateCurrentTurn:

                    // SyncMyTurnState.
                    this.UpdateCurrentTurn(photonEvent.CustomData);
                    break;

                case (byte)CustomPhotonEvents.UpdateCurrentActor:

                    // Update CurrentActorState.
                    this.UpdateCurrentActor(photonEvent.CustomData);
                    break;
            }

            base.OnEvent(photonEvent);
            if (this.OnUpdate != null)
            {
                this.OnUpdate();
            }
        }

        #endregion

        #region CustomPhotonEventsHandling methods.

        private void TakeSeat()
        {
            if (roomShared != null && (roomShared.Seats == null || roomShared.Seats.Count == 0))
            {
                roomShared.Seats = new List<int>();
                Hashtable hashtable = this.CurrentRoom.CustomProperties["gameState"] as Hashtable;
                var seats = (hashtable["seats"] as object[]);
                for (var i = 1; i <= this.CurrentRoom.MaxPlayers; ++i)
                {
                    if (seats.Length <= i || seats[i] == null)
                    {
                        roomShared.Seats.Add(i);
                    }
                }
            }
            //Hashtable hashtable = this.CurrentRoom.CustomProperties["gameState"] as Hashtable;
            //var seats = (hashtable["seats"] as object[]);
            //var minimumIndex = seats.Length;
            //for (var i = 1; i < seats.Length; ++i)
            //{
            //    if (seats[i] == null)
            //    {
            //        minimumIndex = i;
            //        break;
            //    }
            //}

            if (roomShared.Seats != null && roomShared.Seats.Count > 0)
            {
                Dictionary<string, int> parameters = new Dictionary<string, int>();
                parameters.Add("oldSeatNumber", -1);
                parameters.Add("newSeatNumber", roomShared.Seats[this.clientId] % (this.CurrentRoom.MaxPlayers + 1));

                DebugReturn(string.Format(@"TakeSeat: {0} take {1}", this.clientId, roomShared.Seats[this.clientId] % (this.CurrentRoom.MaxPlayers + 1)));

                this.RaiseEvent(CustomPhotonEvents.TakeSeat, parameters);
            }
        }

        private void ChooseRole()
        {
            if(roomShared != null &&(roomShared.Roles == null || roomShared.Roles.Count == 0))
            {

            }


            Hashtable hashtable = this.CurrentRoom.CustomProperties["gameState"] as Hashtable;
            var roles = (hashtable["role"] as object[]);
            var minimumIndex = roles.Length;
            for (var i = 1; i < roles.Length; ++i)
            {
                if (roles[i] == null)
                {
                    minimumIndex = i;
                }
            }

            Dictionary<string, int> parameters = new Dictionary<string, int>();
            parameters.Add("oldRoleId", -1);
            parameters.Add("newRoleId", (minimumIndex + this.clientId - 1) % this.CurrentRoom.MaxPlayers);

            this.RaiseEvent(CustomPhotonEvents.ChooseRole, parameters);
        }

        private void UpdateCurrentTurn(object data)
        {
            string action;
            if (data is Dictionary<string, object>)
            {
                action = Convert.ToString((data as Dictionary<string, object>)["action"]);
            }
            else
            {
                action = Convert.ToString((data as Hashtable)["action"]);
            }

            if (action == "isVoting")
            {
                this.RaiseEvent(CustomPhotonEvents.toupiaoend, "");
            }
        }

        private void UpdateCurrentActor(object data)
        {
            int actorNr;
            if (data is Dictionary<string, object>)
            {
                actorNr = Convert.ToInt32((data as Dictionary<string, object>)["actorNr"]);
            }
            else
            {
                actorNr = Convert.ToInt32((data as Hashtable)["actorNr"]);
            }

            if (this.LocalPlayer.ID == actorNr)
            {
                // todo: isAuthing then shunwei
                Hashtable hashtable = this.CurrentRoom.CustomProperties["gameState"] as Hashtable;
                object[] shunwei_one_been = hashtable["shunwei_one_been"] as object[];
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                var minimumActors = this.CurrentRoom.Players.ToList().Where(player => !player.Value.IsLocal && !shunwei_one_been.Contains(player.Key.ToString()));

                if (minimumActors.Count() > 0)
                {
                    parameters.Add("actorNr", minimumActors.First().Key);
                    parameters.Add("action", "isAuthing");
                    parameters.Add("updateOthers", false);
                    this.RaiseEvent(CustomPhotonEvents.UpdateCurrentActor, parameters);
                }

            }
        }

        #endregion

        #region Helper methods

        private void RaiseEvent(CustomPhotonEvents ev, object message)
        {
            this.OpRaiseEvent((byte)(int)ev, message, false, new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            });
        }

        /// <summary>
        /// Write to console for debugging output.
        /// </summary>
        private void DebugReturn(string debugStr)
        {
            Console.Out.WriteLine(debugStr);
        }

        #endregion
    }
}
