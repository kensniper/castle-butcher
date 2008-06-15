using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class PlayerData:Interfaces.IPlayerDataWrite,Interfaces.IPlayerDataRead,Interfaces.IOtherPlayerData
    {
        #region IPlayerDataRead Members and IPlayerDataWrite Members

        private Microsoft.DirectX.Vector3 positionField = new Microsoft.DirectX.Vector3(0f, 0f, 0f);

        public Microsoft.DirectX.Vector3 Position
        {
            set { positionField = value ; }
            get { return positionField; }
        }

        private Microsoft.DirectX.Vector3 lookingDirectionField = new Microsoft.DirectX.Vector3(0f, 0f, 0f);

        public Microsoft.DirectX.Vector3 LookingDirection
        {
            set { lookingDirectionField =value; }
            get { return lookingDirectionField; }
        }

        private Microsoft.DirectX.Vector3 velocityField = new Microsoft.DirectX.Vector3(0f, 0f, 0f);

        public Microsoft.DirectX.Vector3 Velocity
        {
            set { velocityField = value; }
            get { return velocityField; }
        }

        private Constants.WeaponEnumeration weaponField;

        public UDPClientServerCommons.Constants.WeaponEnumeration Weapon
        {
            set { weaponField = value; }
            get { return weaponField; }
        }

        private bool jumpField;

        public bool Jump
        {
            set { jumpField = value; }
            get { return jumpField; }
        }

        private bool shootField;

        public bool Shoot
        {
            set { shootField = value; }
            get { return shootField; }
        }

        private bool duckField;

        public bool Duck
        {
            set { duckField = value; }
            get { return duckField; }
        }

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }


        #endregion
    }
}
