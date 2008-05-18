using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{
    class CollidableBoxPM : PointMass
    {
        CollisionMesh m_collisionMesh;

        float m_radius;
        bool m_flipped = false;
        float m_size;

        public CollidableBoxPM(MyVector pos, float size, bool flipped):base(new PhysicalProperties(0.2f,0.3f,1))
        {

            m_data.Position = pos;
            m_data.Acceleration = new MyVector(0, 0, 0);
            m_data.AngularVelocity = new MyVector(0, 0, 0);
            m_data.Velocity = new MyVector(0, 0, 0);
            m_data.VelocityBody = new MyVector(0, 0, 0);
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            m_data.Mass = size * size * size;
            m_data.Forces = new MyVector(0, 0, 0);

            
            m_flipped = flipped;
            m_size = size;
            m_radius = 0.5f * size * (float)Math.Sqrt(2);
            //if (flipped)
            //    CreateMeshFlipped();
            //else
            //    CreateMesh();

            if (flipped)
                m_collisionMesh = CollisionMesh.FromFile(DefaultValues.MeshPath + "box1.cm", true, size / 70);
            else
                m_collisionMesh = CollisionMesh.FromFile(DefaultValues.MeshPath + "box1.cm", false, size / 70);



        }

        private void CreateMesh()
        {
            float d = m_size / 2;
            MyVector[] verts = new MyVector[8];
            verts[0] = new MyVector(-d, -d, -d);
            verts[1] = new MyVector(-d, -d, d);
            verts[2] = new MyVector(d, -d, d);
            verts[3] = new MyVector(d, -d, -d);
            verts[4] = new MyVector(d, d, -d);
            verts[5] = new MyVector(-d, d, -d);
            verts[6] = new MyVector(-d, d, d);
            verts[7] = new MyVector(d, d, d);

            uint[] ind ={ 0, 2, 1, 0, 3, 2, 5, 6, 7, 5, 7, 4, 1, 7, 6, 1, 2,
                7, 2, 3, 4, 2, 4, 7, 3, 5, 4, 3, 0, 5, 0, 1, 6, 0, 6, 5 };

            m_collisionMesh = CollisionMesh.FromArrays(verts, ind);
        }
        private void CreateMeshFlipped()
        {
            float d = m_size / 2;
            MyVector[] verts = new MyVector[8];
            verts[0] = new MyVector(-d, -d, -d);
            verts[1] = new MyVector(-d, -d, d);
            verts[2] = new MyVector(d, -d, d);
            verts[3] = new MyVector(d, -d, -d);
            verts[4] = new MyVector(d, d, -d);
            verts[5] = new MyVector(-d, d, -d);
            verts[6] = new MyVector(-d, d, d);
            verts[7] = new MyVector(d, d, d);

            uint[] ind ={ 0, 1, 2, 0, 2, 3, 5, 7, 6, 5, 4, 7, 1, 6, 7, 1, 7,
                2, 2, 4, 3, 2, 7, 4, 3, 4, 5, 3, 5, 0, 0, 6, 1, 0, 5, 6 };

            m_collisionMesh = CollisionMesh.FromArrays(verts, ind);
        }

        public bool Flipped
        {
            get
            {
                return m_flipped;
            }
        }


        public override float BoundingSphereRadius
        {
            get
            {
                return m_radius;
            }
        }

        public override CollisionDataType CollisionDataType
        {
            get
            {
                return CollisionDataType.CollisionMesh;
            }
        }
        public override ICollisionData CollisionData
        {
            get
            {
                return m_collisionMesh;
            }
        }

    }
}
