﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using System.IO;
using System.Collections;

namespace UDPClientPacket
{
    /// <summary>
    /// Client Packet is formmated this way:
    /// PlayerPosition 12
    /// PlayerMovementDirection 12
    /// PlayerLookingDirection 12
    /// PlayerId 2
    ///     {PlayerCarringWeponOne,
    ///     PlayerCarringWeponTwo,
    ///     PlayerJumping,
    ///     PlayerShooting,
    ///     PlayerWalking,
    ///     PlayerRunning,
    ///     PlayerDucking} 1
    /// PacketId 2
    /// Timestamp 8
    /// 
    /// Packet length = 49 bytes
    /// </summary>
    public class ClientPacket :UDPClientServerCommons.PacketIdCommon, UDPClientServerCommons.IPacket
    {
        private const ushort _MTU_PacketSize = 1400;

        private Vector playerPositionField;

        /// <summary>
        /// Stores X,Y,Z floats for player position
        /// </summary>
        public Vector PlayerPosition
        {
            get { return playerPositionField; }
            set { playerPositionField = value; }
        }

        private Vector playerMovementDirectionField;

        /// <summary>
        /// Stores X,Y,Z floats for player moving diection
        /// </summary>
        public Vector PlayerMovementDirection
        {
            get { return playerMovementDirectionField; }
            set { playerMovementDirectionField = value; }
        }
        
        private Vector playerLookingDirectionField;

public Vector PlayerLookingDirection
{
  get { return playerLookingDirectionField; }
  set { playerLookingDirectionField = value; }
}

        private ushort playerIdField;

        /// <summary>
        /// ID of current player
        /// </summary>
        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private bool playerCarringWeponOneField;

        /// <summary>
        /// tells if player is carrying wepon one
        /// </summary>
        public bool PlayerCarringWeponOne
        {
            get { return playerCarringWeponOneField; }
            set { playerCarringWeponOneField = value; }
        }

        private bool playerCarringWeponTwoField;

        /// <summary>
        /// tells if player is carrying wepon two
        /// </summary>
        public bool PlayerCarringWeponTwo
        {
            get { return playerCarringWeponTwoField; }
            set { playerCarringWeponTwoField = value; }
        }

        private bool playerJumpingField;

        /// <summary>
        /// tells if player is jumping
        /// </summary>
        public bool PlayerJumping
        {
            get { return playerJumpingField; }
            set { playerJumpingField = value; }
        }

        private bool playerShootingField;

        /// <summary>
        /// tells if player is shooting with currently held wepon
        /// </summary>
        public bool PlayerShooting
        {
            get { return playerShootingField; }
            set { playerShootingField = value; }
        }

        private bool playerWalkingField;

        /// <summary>
        /// tells if curent movement type is walking
        /// </summary>
        public bool PlayerWalking
        {
            get { return playerWalkingField; }
            set { playerWalkingField = value; }
        }

        private bool playerRunningField;

        /// <summary>
        /// tells if curent movement type is running
        /// </summary>
        public bool PlayerRunning
        {
            get { return playerRunningField; }
            set { playerRunningField = value; }
        }

        private bool playerDuckingField;

        /// <summary>
        /// tells if player is currently in ducking position
        /// </summary>
        public bool PlayerDucking
        {
            get { return playerDuckingField; }
            set { playerDuckingField = value; }
        }

        private DateTime timestampField;

        public DateTime Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);

            ms.Write(playerPositionField.ToByte(),0,12);
            ms.Write(playerMovementDirectionField.ToByte(), 0, 12);
            ms.Write(playerLookingDirectionField.ToByte(), 0, 12);
            ms.Write(BitConverter.GetBytes(playerIdField),0,2);

            BitArray bitArray = new BitArray(8);
            bitArray[0] = playerCarringWeponOneField;
            bitArray[1] = playerCarringWeponTwoField;
            bitArray[2] = playerJumpingField;
            bitArray[3] = playerShootingField;
            bitArray[4] = playerWalkingField;
            bitArray[5] = playerRunningField;
            bitArray[6] = playerDuckingField;

            byte[] bits = new byte[1];
            bitArray.CopyTo(bits, 0);

            ms.Write(bits, 0, 1);

            ms.Write(BitConverter.GetBytes(PacketId), 0, 2);
            ms.Write(BitConverter.GetBytes(Timestamp.ToBinary()), 0, 8);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public ClientPacket()
        { }

        public ClientPacket(ClientPacket oldPacket,
            Vector newPlayerPosition,
            Vector newPlayerMovementDirection,
            Vector newPlayerLookingDirection)
        {
            playerPositionField = new Vector(newPlayerPosition);
            playerMovementDirectionField = new Vector(newPlayerMovementDirection);
            playerLookingDirectionField = new Vector(newPlayerLookingDirection);

            this.PacketId = (ushort)(oldPacket.PacketId+1);
            this.playerCarringWeponOneField = oldPacket.playerCarringWeponOneField;
            this.playerCarringWeponTwoField = oldPacket.playerCarringWeponTwoField;
            this.playerDuckingField = oldPacket.playerDuckingField;
            this.playerIdField = oldPacket.playerIdField;
            this.playerJumpingField = oldPacket.playerJumpingField;
            this.playerRunningField = oldPacket.playerRunningField;
            this.playerShootingField = oldPacket.playerShootingField;
            this.playerWalkingField = oldPacket.playerWalkingField;
            this.Timestamp = DateTime.Now;
        }

        public ClientPacket(byte[] binaryClientPacket)
        {
            this.playerPositionField = new Vector(binaryClientPacket, 0);
            this.playerMovementDirectionField = new Vector(binaryClientPacket, 12);
            this.playerLookingDirectionField = new Vector(binaryClientPacket, 24);
            this.playerIdField = (ushort)BitConverter.ToInt16(binaryClientPacket, 36);

            byte[] tmp = { binaryClientPacket[38] };
            BitArray bitArray = new BitArray(tmp);
            playerCarringWeponOneField = bitArray[0];
            playerCarringWeponTwoField = bitArray[1];
            playerJumpingField = bitArray[2];
            playerShootingField = bitArray[3];
            playerWalkingField = bitArray[4];
            playerRunningField = bitArray[5];
            playerDuckingField = bitArray[6];

            this.PacketId = (ushort)BitConverter.ToInt16(binaryClientPacket, 39);
            this.Timestamp = DateTime.FromBinary( BitConverter.ToInt64(binaryClientPacket, 41));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" PacketId = \t");
            sb.Append(this.PacketId);
            sb.Append("\n PlayerCarringWeponOne = \t");
            sb.Append(playerCarringWeponOneField);
            sb.Append("\n playerCarringWeponTwo = \t");
            sb.Append(this.playerCarringWeponTwoField);
            sb.Append("\n playerDuckingField = \t");
            sb.Append(this.playerDuckingField);
            sb.Append("\n playerIdField = \t");
            sb.Append(this.playerIdField);
            sb.Append("\n playerJumpingField = \t");
            sb.Append(this.playerJumpingField);
            sb.Append("\n playerLookingDirectionField = \t");
            sb.Append(this.playerLookingDirectionField);
            sb.Append("\n playerMovementDirectionField = \t");
            sb.Append(this.playerMovementDirectionField);
            sb.Append("\n playerPositionField = \t");
            sb.Append(this.playerPositionField);
            sb.Append("\n playerRunningField = \t");
            sb.Append(this.playerRunningField);
            sb.Append("\n playerShootingField = \t");
            sb.Append(this.playerShootingField);
            sb.Append("\n playerWalkingField = \t");
            sb.Append(this.playerWalkingField);
            sb.Append("\n timestampField = \t");
            sb.Append(this.Timestamp);

            return sb.ToString();
        }

        #endregion
    }
}
