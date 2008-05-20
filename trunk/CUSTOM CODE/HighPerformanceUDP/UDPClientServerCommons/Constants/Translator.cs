using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public static class Translator
    {
        public static UDPClientServerCommons.PlayerInfo TranslateBetweenClientPacketAndPlayerInfo(UDPClientServerCommons.ClientPacket from)
        {
            UDPClientServerCommons.PlayerInfo to = new PlayerInfo();
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
            to.Timestamp = from.Timestamp;

            return to;
        }
    }
}
