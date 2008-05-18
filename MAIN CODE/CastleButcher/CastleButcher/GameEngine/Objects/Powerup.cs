using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    public class Powerup : StaticBody,IGameObject
    {
        #region GameObject Members

        public string Name
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public new Framework.MyMath.MyVector Position
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
        public Matrix Transform
        {
            get
            {
                return Matrix.Translation(Position.X, Position.Y, Position.Z);
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Framework.Physics.IPhysicalObject PhysicalData
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public RenderingData RenderingData
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        public static Powerup FromFile(string p)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region StaticBody members
        public override float BoundingSphereRadius
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override CollisionDataType CollisionDataType
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override ICollisionData CollisionData
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

      
        #endregion

    }
}
