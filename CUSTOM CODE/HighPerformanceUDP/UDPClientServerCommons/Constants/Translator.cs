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
            //to.PlayerRunning = from.PlayerRunning;
            to.PlayerShooting = from.PlayerShooting;
            //to.PlayerWalking = from.PlayerWalking;

            return to;
        }

        public static Vector TranslateBetweenVectorAndVector(Microsoft.DirectX.Vector3 from)
        {
            Vector to = new Vector(from.X, from.Y, from.Z);
            return to;
        }

        public static Interfaces.IOtherPlayerData TranslatePlayerInfoToPlayerOtherData(Packets.PlayerInfo from)
        {
            Usefull.PlayerData to = new UDPClientServerCommons.Usefull.PlayerData();

            to.PlayerId = from.PlayerId;
            to.LookingDirection = TranslateBetweenVectorAndVector(from.PlayerLookingDirection);
            to.Position = TranslateBetweenVectorAndVector(from.PlayerPosition);
            to.Velocity = TranslateBetweenVectorAndVector(from.PlayerMovementDirection);
            if (from.PlayerCarringWeponOne)
                to.Weapon = UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow;
            else if (from.PlayerCarringWeponTwo)
                to.Weapon = UDPClientServerCommons.Constants.WeaponEnumeration.Sword;
            else
                to.Weapon = UDPClientServerCommons.Constants.WeaponEnumeration.None;

            return (Interfaces.IOtherPlayerData)to;
        }

        public static Microsoft.DirectX.Vector3 TranslateBetweenVectorAndVector(Vector from)
        {
            Microsoft.DirectX.Vector3 to = new Microsoft.DirectX.Vector3(from.X, from.Y, from.Z);
            return to;
        }
    }
}
