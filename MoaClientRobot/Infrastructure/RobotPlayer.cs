﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RobotPlayer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This class encapsulates a "player". It has members for position, nickName and color and provides methods to
//   change these values and to send them to Photon and other players.
//   Server side, Photon cloud with loadbalancing is used. 
// </summary>
// <author>developer@photonengine.com</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ExitGames.Client.Infrastructure
{
    using System;
    using System.Collections;
    using ExitGames.Client.Photon;
    using ExitGames.Client.Photon.LoadBalancing;

    /// <summary>
    /// This RobotPlayer extends the Playerclass from the LoadbalancginApi
    /// to hold extra position and color data for this demo game. 
    /// </summary>
    public class RobotPlayer : Player
    {
        #region Variables
        
        /// <summary>Position x-value.</summary>
        /// <remarks>This is send to others in this.SendEvMove()</remarks>
        public int posX;

        /// <summary>Position y-value.</summary>
        /// <remarks>This is send to others in this.SendEvMove()</remarks>
        public int posY;

        /// <summary>This player's color (as RGB integer).</summary>
        /// <remarks>This is send to others in this.SendPlayerInfo()</remarks>
        public int color;

        /// <summary>
        /// Controls if the local player will send his position updates reliable or unreliable.
        /// In a real game, you would send some events reliable and some unreliable, depending
        /// on the type of info you send. See SendEvMove().
        /// Position data is a case for unreliable updates: Each position is outdated soon and 
        /// completely replaced with the next event. A missing update won't matter.
        /// The Player info (see SendPlayerInfo) is a sample for data you should send reliable
        /// in a real game: The username is not sent every update and it's bad if this would be
        /// lost.
        /// </summary>
        /// <remarks>
        /// Sending events reliable make sure they arrive on the other side but in case of
        /// packet loss, this might induce a delay (until the reliable package is finally arriving).
        /// </remarks>
        public static bool isSendReliable;

        #endregion

        #region Constants

        /// <summary>For this demo, we use a 16x16 tiles "game board" on which players can move.</summary>
        /// <remarks>The grid size is shared by realtime demos on other platforms, so you can "play" against a C++ Windows client.</remarks>
        public const byte GRIDSIZE = 16;

        /// <summary>
        /// Using the Lite logic on the server, this demo can send just about any key-value pair in an event.
        /// To keep things lean, we use event keys of type byte.
        /// </summary>
        /// <remarks>
        /// We don't define a type for the value anywhere. This is done "by convention of this demo". Again, the client
        /// is open to send whatever it likes but we make sure to send the same type as we expect in Game.OnEvent().
        /// </remarks>
        public enum DemoEventKey : byte { PlayerPositionX = 0, PlayerPositionY = 1, PlayerName = 2, PlayerColor = 3 }

        /// <summary>
        /// Using the Lite logic on the server, this demo can send just about any event it wants to.
        /// The event and event-content is purely defined client side. The server does not understand or know this, 
        /// which is ok. We don't want to check for cheating and don't persist data in this demo.
        /// </summary>
        public enum DemoEventCode : byte { PlayerInfo = 0, PlayerMove = 1, CountMe = 3 }
        
        #endregion

        #region Player Control

        /// <summary>
        /// A player represents some user's client in the room we're in. 
        /// </summary>
        /// <param name="actorNr">In a room, each player has a number to be identified. We initialize out local Player before we get a actor number (with -1 then).</param>
        public RobotPlayer(string nickName, int actorNr, bool isLocal) : base(nickName, actorNr, isLocal)
        {
            // actorNumbers in-game start with 1. any local creation of players gets randomized here
            if (actorNr < 1)
            {
                this.posX = SupportClass.ThreadSafeRandom.Next() % GRIDSIZE;
                this.posY = SupportClass.ThreadSafeRandom.Next() % GRIDSIZE;
                this.color = (int)SupportClass.ThreadSafeRandom.Next();
            }
        }

        /// <summary>
        /// This method puts the "player info" into a Hashtable (a serializable Datatype for Photon)
        /// and calls LitePeer.OpRaiseEvent() to make the server send an event to others in the room.
        /// </summary>
        /// <remarks>The event we send here is handled by each client's Game.OnEvent().</remarks>
        internal void SendPlayerInfo(LoadBalancingPeer peer)
        {
            if (peer == null)
            {
                return;
            }

            // Setting up the content of the event. Here we want to send a player's info: nickName and color.
            Hashtable evInfo = new Hashtable();
            evInfo.Add((byte)DemoEventKey.PlayerName, this.NickName);
            evInfo.Add((byte)DemoEventKey.PlayerColor, this.color);

            // The event's code must be of type byte, so we have to cast it. We do this above as well, to get routine ;)
            peer.OpRaiseEvent((byte)DemoEventCode.PlayerInfo, evInfo, true, null);
        }

        /// <summary>
        /// When Game.OnEvent() receives a info event (sent by SendPlayerInfo), it's content is used here.
        /// </summary>
        /// <param name="customEventContent"></param>
        internal void SetInfo(Hashtable customEventContent)
        {
            this.NickName = (string)customEventContent[(byte)DemoEventKey.PlayerName];
            this.color = (Int32)customEventContent[(byte)DemoEventKey.PlayerColor];
        }

        /// <summary>
        /// Raises an event with the player's position data. 
        /// "isSendReliable"        decides if these events are reliable or unreliable.
        /// "Game.RaiseEncrypted"   decided if the event is encrypted.
        /// 
        /// Once more: Neither sending reliable nor encrypting this event makes sense in a game.
        /// Both is just done as showcase!
        /// </summary>
        /// <remarks>
        /// Each running client will know many players, but only one is the "local" one, which
        /// actually sends it's position! See Game.SendPostion(), which choses the local player only.
        /// </remarks>
        /// <param name="peer"></param>
        internal void SendEvMove(LoadBalancingPeer peer)
        {
            if (peer == null)
            {
                return;
            }

            // prepare the event data we want to send
            // this could contain more key-values as needed by a game (think: rotation, y-coordinate)
            Hashtable eventContent = new Hashtable();
            eventContent.Add((byte)DemoEventKey.PlayerPositionX, (byte)this.posX);
            eventContent.Add((byte)DemoEventKey.PlayerPositionY, (byte)this.posY);

            // if encryption is turned off, we simply use OpRaiseEvent
            peer.OpRaiseEvent((byte)DemoEventCode.PlayerMove, eventContent, isSendReliable, null);
        }

        // updates a (remote player's) position. directly gets the new position from the received event
        internal void SetPosition(Hashtable evData)
        {
            this.posX = (byte)evData[(byte)DemoEventKey.PlayerPositionX];
            this.posY = (byte)evData[(byte)DemoEventKey.PlayerPositionY];
        }

        // moves the player x,y steps and checks boundaries
        public void Move(int x, int y)
        {
            this.posX += x;
            this.posY += y;
            this.ClampPosition();
        }

        /// <summary>
        /// Simple method to make the "player" move even without input. This way, we get some
        /// movement even if one developer tests with many running clients.
        /// </summary>
        internal void MoveRandom()
        {
            this.posX += (SupportClass.ThreadSafeRandom.Next() % 3) - 1;
            this.posY += (SupportClass.ThreadSafeRandom.Next() % 3) - 1;
            this.ClampPosition();
        }

        /// <summary>Checks if a position is in the grid (still on the board) and corrects it if needed.</summary>
        public void ClampPosition()
        {
            if (this.posX < 0)
            {
                this.posX = 0;
            }

            if (this.posX >= GRIDSIZE - 1)
            {
                this.posX = GRIDSIZE - 1;
            }

            if (this.posY < 0)
            {
                this.posY = 0;
            }

            if (this.posY > GRIDSIZE - 1)
            {
                this.posY = GRIDSIZE - 1;
            }
        }

        public override string ToString()
        {
            return this.ID + "'" + this.NickName + "':" + this.color.ToString() + " " + this.posX + ":" + this.posY + " PlayerProps: " + SupportClass.DictionaryToString(this.CustomProperties);
        }

        #endregion
    }
}