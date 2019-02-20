// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RobotClientStarter.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The RobotClientStarter class mainly wraps a singleton pattern around the used client which connects to the cloud. 
//   Periodically the clients service-method is called in a custom gameloop where demo-specific tasks are done.
// </summary>
// <author>developer@photonengine.com</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ExitGames.Client.Infrastructure
{
    public class RobotClientStarter
    {
        #region Fields
        public RobotClient Client { get; set; }

        private static RobotClientStarter instance; 
        #endregion

        public RobotClientStarter()
        {
            this.Client = new RobotClient(true);
        }

        /// <summary>
        /// Singleton pattern. 
        /// </summary>
        public static RobotClientStarter Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new RobotClientStarter();
                }
                return instance;
            }
        }
    }
}
