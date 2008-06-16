using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Microsoft.DirectX.Direct3D;
using Framework.MyMath;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    public class StaticMesh : StaticBody, IGameObject
    {
        CollisionDataType collisionDataType;
        ICollisionData collisionData;
        RenderingData renderingData;
        //StaticBodyData bodyData;

        public StaticMesh(ICollisionData cdata, CollisionDataType cdataType, RenderingData rdata, MyVector position)
        {
            collisionDataType = cdataType;
            collisionData = cdata;
            renderingData = rdata;
            this.Position = position;
            //bodyData.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            this.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);

        }
        public StaticMesh(ICollisionData cdata, CollisionDataType cdataType, RenderingData rdata, MyVector position, MyVector rotation)
        {
            collisionDataType = cdataType;
            collisionData = cdata;
            renderingData = rdata;
            this.Position = position;
            this.Orientation = MyQuaternion.FromEulerAngles(rotation.X, rotation.Y, rotation.Z);

        }
        #region GameObject Members

        public string Name
        {
            get
            {
                return "StaticMesh";
            }
        }
        public Microsoft.DirectX.Matrix Transform
        {
            get
            {
                return Matrix.RotationQuaternion((Quaternion)this.Orientation) *
                    Matrix.Translation(Position.X, Position.Y, Position.Z);
            }
        }
        public Framework.Physics.IPhysicalObject PhysicalData
        {
            get
            {
                return this;
            }
        }

        public RenderingData RenderingData
        {
            get { return renderingData; }
        }

        #endregion

        public override float BoundingSphereRadius
        {
            get
            {
                if (CollisionData != null)
                {
                    return CollisionData.BoundingSphereRadius;
                }
                else
                    return renderingData.BoundingSphereRadius;
            }
        }

        public override CollisionDataType CollisionDataType
        {
            get { return collisionDataType; }
        }

        public override ICollisionData CollisionData
        {
            get { return collisionData; }
        }


    }

    public class DeadCharacter : StaticMesh
    {
        public DeadCharacter(RenderingData rdata, MyVector position)
            : base(null, CollisionDataType.None, rdata, position)
        {


        }
    }
}
