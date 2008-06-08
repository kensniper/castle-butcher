using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons
{
    public static class Translator
    {
        public static PlayerInfo TranslateBetweenClientPacketAndPlayerInfo(ClientPacket from)
        {
            PlayerInfo to = new PlayerInfo();
            to.PlayerCarringWeponOne = from.PlayerCarringWeponOne;
            to.PlayerCarringWeponTwo = from.PlayerCarringWeponTwo;
            to.PlayerDucking = from.PlayerDucking;
            to.PlayerId = from.PlayerId;
            to.PlayerJumping = from.PlayerJumping;
            to.PlayerLookingDirection = from.PlayerLookingDirection;
            to.PlayerMovementDirection = from.PlayerMovementDirection;
            to.PlayerPosition = from.PlayerPosition;
            to.PlayerRunning = from.PlayerRunning;
            to.PlayerShooting = from.PlayerShooting;
            to.PlayerWalking = from.PlayerWalking;

            return to;
        }

        public static Vector TranslateVector3toVector(Microsoft.DirectX.Vector3 from)
        {
            Vector to = new Vector(from.X, from.Y, from.Z);
            return to;
        }
    }
}
