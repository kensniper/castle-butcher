    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Collections;
using UDPClientServerCommons.Constants;

    namespace UDPClientServerCommons.Packets
    {
        /// <summary>
        /// Client Packet is formated this way:
        /// Packet type 2
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
        /// Timestamp 8
        /// 
        /// Packet length = 49 bytes
        /// </summary>
        public class BasePlayerInfo : Interfaces.ISerializablePacket,ICloneable
        {
            #region Fields

            public const ushort _MTU_PacketSize = 1400;

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

            private bool AckRequiredField = false;

            public bool AckRequired
            {
                get { return AckRequiredField; }
                set { AckRequiredField = value; }
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

            #endregion

            #region Constructor

            public BasePlayerInfo()
            {
                playerPositionField = new Vector();
                playerMovementDirectionField = new Vector();
                playerLookingDirectionField = new Vector();
            }

            public BasePlayerInfo(BasePlayerInfo oldPacket,
                Vector newPlayerPosition,
                Vector newPlayerMovementDirection,
                Vector newPlayerLookingDirection)
            {
                playerPositionField = new Vector(newPlayerPosition);
                playerMovementDirectionField = new Vector(newPlayerMovementDirection);
                playerLookingDirectionField = new Vector(newPlayerLookingDirection);

                //this.PacketId = (ushort)(oldPacket.PacketId + 1);
                this.playerCarringWeponOneField = oldPacket.playerCarringWeponOneField;
                this.playerCarringWeponTwoField = oldPacket.playerCarringWeponTwoField;
                this.playerDuckingField = oldPacket.playerDuckingField;
                this.playerIdField = oldPacket.playerIdField;
                this.playerJumpingField = oldPacket.playerJumpingField;
                this.playerRunningField = oldPacket.playerRunningField;
                this.playerShootingField = oldPacket.playerShootingField;
                this.playerWalkingField = oldPacket.playerWalkingField;
            }

            public BasePlayerInfo(byte[] binaryBasePlayerInfo)
            {
                HiddenContructor(binaryBasePlayerInfo, 0);
            }

            public BasePlayerInfo(byte[] binaryBasePlayerInfo, int index)
            {
                HiddenContructor(binaryBasePlayerInfo, index);
            }

            private void HiddenContructor(byte[] binaryBasePlayerInfo, int index)
            {
                this.playerPositionField = new Vector(binaryBasePlayerInfo, index + 0);
                this.playerMovementDirectionField = new Vector(binaryBasePlayerInfo, index + 12);
                this.playerLookingDirectionField = new Vector(binaryBasePlayerInfo, index + 24);
                this.playerIdField = (ushort)BitConverter.ToInt16(binaryBasePlayerInfo, index + 36);

                byte[] tmp = { binaryBasePlayerInfo[index + 38] };
                BitArray bitArray = new BitArray(tmp);
                playerCarringWeponOneField = bitArray[0];
                playerCarringWeponTwoField = bitArray[1];
                playerJumpingField = bitArray[2];
                playerShootingField = bitArray[3];
                playerWalkingField = bitArray[4];
                playerRunningField = bitArray[5];
                playerDuckingField = bitArray[6];
                AckRequiredField = bitArray[7];
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
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
                sb.Append("\n AckRequired = \t");
                sb.Append(this.AckRequiredField);
              
                return sb.ToString();
            }

            #endregion

            #region ICloneable Members

            public object Clone()
            {
                BasePlayerInfo copy = new BasePlayerInfo();
                copy.playerCarringWeponOneField = this.PlayerCarringWeponOne;
                copy.playerCarringWeponTwoField = this.playerCarringWeponTwoField;
                copy.playerDuckingField = this.playerDuckingField;
                copy.playerIdField = this.playerIdField;
                copy.playerJumpingField = this.playerJumpingField;
                copy.playerLookingDirectionField = (Vector)this.playerLookingDirectionField.Clone();
                copy.playerMovementDirectionField = (Vector)this.playerMovementDirectionField.Clone();
                copy.playerPositionField = (Vector)this.playerPositionField.Clone();
                copy.playerShootingField = this.playerShootingField;
                copy.playerWalkingField = this.playerWalkingField;
                copy.playerRunningField = this.playerRunningField;
                copy.AckRequiredField = this.AckRequiredField;
                return copy;
            }

            #endregion

            #region ISerializablePacket Members

            private byte[] ToByteConversion(int size)
            {
                MemoryStream ms = new MemoryStream(size);

              //  int pos = 0;

              //  ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
                //pos+=2;
                ms.Write(playerPositionField.ToByte(),0 , 12);
                //pos += 12;
                ms.Write(playerMovementDirectionField.ToByte(), 0, 12);
                //pos += 12;
                ms.Write(playerLookingDirectionField.ToByte(), 0, 12);
                //pos += 12;
                ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
                //pos += 2;

                BitArray bitArray = new BitArray(8);
                bitArray[0] = playerCarringWeponOneField;
                bitArray[1] = playerCarringWeponTwoField;
                bitArray[2] = playerJumpingField;
                bitArray[3] = playerShootingField;
                bitArray[4] = playerWalkingField;
                bitArray[5] = playerRunningField;
                bitArray[6] = playerDuckingField;
                bitArray[7] = AckRequiredField;

                byte[] bits = new byte[1];
                bitArray.CopyTo(bits, 0);

                ms.Write(bits, 0, 1);
                //pos += 1;

                //ms.Write(BitConverter.GetBytes(PacketId), 0, 2);

                byte[] result = ms.GetBuffer();
                ms.Close();

                return result;
            }

            public virtual byte[] ToByte()
            {
                return ToByteConversion(_MTU_PacketSize);
            }

            public virtual byte[] ToMinimalByte()
            {
                return ToByteConversion(this.ByteCount);
            }

            public int ByteCount
            {
                get
                {
                    int pos = 0;
                   // pos += 2;
                    pos += 12;
                    pos += 12;
                    pos += 12;
                    pos += 2;
                    pos += 1;
                    //pos += 8;
                    return pos;
                }
            }

            #endregion
        }
    }

