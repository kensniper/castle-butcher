using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;
using Framework.Physics;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    public class RespawnPoint : IGameObject, Framework.IUpdateable
    {
        float timeSinceUse = RespawnCooldown;
        GameTeam team;

        public GameTeam Team
        {
            get { return team; }
            set { team = value; }
        }
        public static float RespawnCooldown = 1;

        MyVector position;

        public MyVector Position
        {
            get
            {
                return position;
            }
        }
        MyQuaternion orientation;

        public MyQuaternion Orientation
        {
            get { return orientation; }
        }


        public RespawnPoint(MyVector pos)
        {
            position = pos;
            timeSinceUse = RespawnCooldown;
            orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
        }

        public RespawnPoint(MyVector pos, MyVector rotation)
        {
            position = pos;
            timeSinceUse = RespawnCooldown;
            orientation = MyQuaternion.FromEulerAngles(rotation.X, rotation.Y, rotation.Z);
        }
        public RespawnPoint(MyVector pos, GameTeam team)
        {
            position = pos;
            timeSinceUse = RespawnCooldown;
            orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            this.team = team;
        }

        public RespawnPoint(MyVector pos, MyVector rotation, GameTeam team)
        {
            position = pos;
            timeSinceUse = RespawnCooldown;
            orientation = MyQuaternion.FromEulerAngles(rotation.X, rotation.Y, rotation.Z);
            this.team = team;
        }

        public void Reset()
        {
            timeSinceUse = 0;
        }
        public bool Ready
        {
            get
            {
                return timeSinceUse >= RespawnCooldown;
            }
        }
        #region GameObject Members

        public string Name
        {
            get
            {
                return "RespawnPoint:" + team.ToString();
            }
        }
        public Microsoft.DirectX.Matrix Transform
        {
            get
            {
                return Matrix.Translation(Position.X, Position.Y, Position.Z);
            }
            //set
            //{
            //    throw new Exception("The method or operation is not implemented.");
            //}
        }

        public Framework.Physics.IPhysicalObject PhysicalData
        {
            get
            {
                return null;
            }
        }

        public RenderingData RenderingData
        {
            get { return null; }
        }

        #endregion




        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            if (timeSinceUse < RespawnCooldown)
            {
                timeSinceUse += timeElapsed;
                return true;
            }
            return false;
        }

        #endregion


    }
}
