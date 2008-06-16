using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    /// <summary>
    /// player health
    /// </summary>
    public class PlayerHealthData
    {
        private ushort playerIdField;

        /// <summary>
        /// player id
        /// </summary>
        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private ushort playerHealthField;

        /// <summary>
        /// player health
        /// 0 - means dead
        /// 100 - means healthy
        /// (0,100) - means wounded
        /// </summary>
        public ushort PlayerHealth
        {
            get { return playerHealthField; }
            set { playerHealthField = value; }
        }
    }
}
