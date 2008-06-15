using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Framework.MyMath;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace Framework.Physics
{
    public struct CFace
    {
        public uint v1, v2, v3;
        internal MyVector n;
        internal CFace(uint v1, uint v2, uint v3, MyVector normal)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            n = normal;
        }

    }
    public struct CEdge : IComparable
    {
        public uint v1, v2;

        internal CEdge(uint v1, uint v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
        #region IComparable Members

        public int CompareTo(object obj)
        {
            if ((((CEdge)obj).v1 == v1 && ((CEdge)obj).v2 == v2) ||
                (((CEdge)obj).v2 == v1 && ((CEdge)obj).v1 == v2))
                return 0;
            else return -1;
        }

        #endregion
    }
    public class CollisionMesh : ICollisionData
    {
        public MyVector[] m_vertices;
        public CFace[] m_faces;
        public CEdge[] m_edges;


        public MyVector[] m_hardPoints;

        public MyVector MeshRotation;
        private float boundingShpereRadius;

        



        //public void FromMesh(Mesh mesh)
        //{
        //    int[] adjacency=new int[mesh.NumberFaces*3];
        //    mesh.GenerateAdjacency(1,adjacency);
        //    Mesh m=mesh.Optimize(MeshFlags.SimplifyFace,adjacency);
        //    CustomVertex.PositionOnly[] vdata = (CustomVertex.PositionOnly[])m.LockVertexBuffer(typeof(CustomVertex.PositionOnly), LockFlags.None, m.NumberVertices);

        //    int[] ind = (int[])m.LockIndexBuffer(typeof(int), LockFlags.None, mesh.NumberFaces * 3);

        //    m_vertices = new MyVector[vdata.Length];
        //    for(int i=0;i<m_vertices.Length;i++)
        //    {
        //        m_vertices[i].X = vdata[i].X;
        //        m_vertices[i].Y = vdata[i].Y;
        //        m_vertices[i].Z = vdata[i].Z;
        //    }


        //}
        /// <summary>
        /// Generates collision mesh from arrays
        /// </summary>
        /// <param name="vertices">An Array of vertices</param>
        /// <param name="faces">3 zero based indices per face</param>
        public static CollisionMesh FromArrays(MyVector[] vertices, uint[] faces)
        {
            CollisionMesh mesh = new CollisionMesh();
            mesh.m_vertices = (MyVector[])vertices.Clone();
            mesh.m_faces = new CFace[faces.Length / 3];
            List<CEdge> edges = new List<CEdge>();

            mesh.boundingShpereRadius = 0;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].Length > mesh.boundingShpereRadius)
                    mesh.boundingShpereRadius = vertices[i].Length;
            }
            for (int i = 0; i < faces.Length; i += 3)
            {
                mesh.m_faces[i / 3].v1 = faces[i];
                mesh.m_faces[i / 3].v2 = faces[i + 1];
                mesh.m_faces[i / 3].v3 = faces[i + 2];
                mesh.m_faces[i / 3].n = ((mesh.m_vertices[faces[i + 1]] - mesh.m_vertices[faces[i]]) ^
                    (mesh.m_vertices[faces[i + 2]] - mesh.m_vertices[faces[i]])).Normalize();

                bool be1 = false, be2 = false, be3 = false;
                CEdge e1, e2, e3;
                e1 = new CEdge();
                e1.v1 = faces[i];
                e1.v2 = faces[i + 1];

                e2 = new CEdge();
                e2.v1 = faces[i + 1];
                e2.v2 = faces[i + 2];

                e3 = new CEdge();
                e3.v1 = faces[i + 2];
                e3.v2 = faces[i];
                for (int j = 0; j < edges.Count; j++)
                {
                    if (!be1 && edges[j].CompareTo(e1) == 0)
                        be1 = true;

                    if (!be2 && edges[j].CompareTo(e2) == 0)
                        be2 = true;

                    if (!be3 && edges[j].CompareTo(e3) == 0)
                        be3 = true;
                }
                if (!be1)
                    edges.Add(e1);

                if (!be2)
                    edges.Add(e2);

                if (!be3)
                    edges.Add(e3);


            }

            mesh.m_edges = edges.ToArray();
            mesh.m_hardPoints = (MyVector[])vertices.Clone();
            return mesh;
        }

        /// <summary>
        /// Loads collision mesh data from file
        /// </summary>
        /// <param name="path">Path to file</param>   
        /// <param name="flip">Specifies if the mesh should be flipped</param>
        /// <returns>A new CollisionMesh object cobtaining file's data</returns>
        public static CollisionMesh FromFile(string path, bool flip)
        {
            return FromFile(path, flip, 1);
        }
        /// <summary>
        /// Loads collision mesh data from file
        /// </summary>
        /// <param name="path">Path to file</param>   
        /// <param name="flip">Specifies if the mesh should be flipped</param>
        /// <param name="scale">Mesh vertex data will be multiplied by this</param>
        /// <returns>A new CollisionMesh object cobtaining file's data</returns>
        public static CollisionMesh FromFile(string path, bool flip, float scale)
        {
            BinaryReader file = new BinaryReader(new FileStream(path, FileMode.Open));
            CollisionMesh mesh = new CollisionMesh();
            mesh.m_vertices = new MyVector[file.ReadInt32()];
            for (int i = 0; i < mesh.m_vertices.Length; i++)
            {
                mesh.m_vertices[i] = new MyVector();
                mesh.m_vertices[i].X = scale * file.ReadSingle();
                mesh.m_vertices[i].Y = scale * file.ReadSingle();
                mesh.m_vertices[i].Z = scale * file.ReadSingle();
            }
            mesh.boundingShpereRadius = 0;

            for (int i = 0; i < mesh.m_vertices.Length; i++)
            {
                if (mesh.m_vertices[i].Length > mesh.boundingShpereRadius)
                    mesh.boundingShpereRadius = mesh.m_vertices[i].Length;
            }

            mesh.m_faces = new CFace[file.ReadInt32()];
            for (int i = 0; i < mesh.m_faces.Length; i++)
            {


                if (!flip)
                {
                    mesh.m_faces[i].v1 = (uint)file.ReadInt32() - 1;
                    mesh.m_faces[i].v2 = (uint)file.ReadInt32() - 1;
                    mesh.m_faces[i].v3 = (uint)file.ReadInt32() - 1;
                }
                else
                {
                    mesh.m_faces[i].v3 = (uint)file.ReadInt32() - 1;
                    mesh.m_faces[i].v2 = (uint)file.ReadInt32() - 1;
                    mesh.m_faces[i].v1 = (uint)file.ReadInt32() - 1;
                }
                mesh.m_faces[i].n = ((mesh.m_vertices[mesh.m_faces[i].v2] - mesh.m_vertices[mesh.m_faces[i].v1]) ^
                        (mesh.m_vertices[mesh.m_faces[i].v3] - mesh.m_vertices[mesh.m_faces[i].v1])).Normalize();

            }

            try
            {
                mesh.m_edges = new CEdge[file.ReadInt32()];
                for (int i = 0; i < mesh.m_edges.Length; i++)
                {
                    mesh.m_edges[i] = new CEdge((uint)file.ReadInt32() - 1, (uint)file.ReadInt32() - 1);
                }
            }
            catch
            {
                mesh.m_edges = new CEdge[0];
            }

            file.Close();
            mesh.m_hardPoints = (MyVector[])mesh.m_vertices.Clone();
            return mesh;
        }

        void FlipNormals()
        {
            for (int i = 0; i < m_faces.Length; i++)
            {
                m_faces[i].n = -m_faces[i].n;
            }
        }

        #region ICollisionData Members

        public float BoundingSphereRadius
        {
            get { return boundingShpereRadius; }
        }
        public ICollisionData Rotate(MyQuaternion orientation)
        {
            for (int i = 0; i < m_vertices.Length; i++)
            {
                m_vertices[i].Rotate(orientation);
            }
            for (int i = 0; i < m_hardPoints.Length; i++)
            {
                m_hardPoints[i].Rotate(orientation);
            }
            for (int i = 0; i < m_faces.Length; i++)
            {
                m_faces[i].n.Rotate(orientation);
            }

            MeshRotation.Rotate(orientation);
            return this;
        }
        public bool IntersectPoint(out MyVector intersectionPoint, out MyVector intersectionNormal,
            out float collisionTime, MyVector startPoint, MyVector endPoint, float dt)
        {

            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            MyVector rVelocity = (endPoint - startPoint) / dt;
            MyVector translate=(endPoint - startPoint);

            collisionTime = 0;
            bool status = false;
            float mindst = 0;
            MyVector iPoint;
            MyVector n1, n2, n3;
            float dist, speed, d1, d2, d3;


            for (int i = 0; i < m_faces.Length; i++)
            {

                if ((endPoint - m_vertices[m_faces[i].v1]) * m_faces[i].n > 0)
                {
                    //the line doesn't cross the triangle
                    continue;
                }
                dist = m_faces[i].n * (startPoint - m_vertices[m_faces[i].v1]);
                speed = -dist / (m_faces[i].n * rVelocity);

                if (speed < 0)
                {
                    if (translate * (m_faces[i].n)<0)
                    {
                        if (speed < -0.04)
                            continue;
                    }
                    else
                        continue;
                }
                if (status == true && speed >= collisionTime)
                    continue;
                iPoint = startPoint + speed * rVelocity;

                //3 planes around triangle
                //plane1
                n1 = ((m_vertices[m_faces[i].v2] - m_vertices[m_faces[i].v1]) ^ m_faces[i].n).Normalize();
                d1 = n1 * (m_vertices[m_faces[i].v1]);

                //plane2
                n2 = ((m_vertices[m_faces[i].v3] - m_vertices[m_faces[i].v2]) ^ m_faces[i].n).Normalize();
                d2 = n2 * (m_vertices[m_faces[i].v2]);

                //plane3
                n3 = ((m_vertices[m_faces[i].v1] - m_vertices[m_faces[i].v3]) ^ m_faces[i].n).Normalize();
                d3 = n3 * (m_vertices[m_faces[i].v3]);



                float x1 = n1 * iPoint - d1;
                float x2 = n2 * iPoint - d2;
                float x3 = n3 * iPoint - d3;
                if (x1 <= 0 && x2 <= 0 && x3 <= 0)
                {
                    float dst = (startPoint - iPoint).LengthSq;
                    if (status == false)
                    {
                        mindst = dst;
                        status = true;
                        intersectionPoint = iPoint;
                        intersectionNormal = m_faces[i].n;
                        collisionTime = speed;
                    }
                    else if (dst < mindst)
                    {
                        mindst = dst;
                        intersectionPoint = iPoint;
                        intersectionNormal = m_faces[i].n;
                        collisionTime = speed;
                    }
                }
            }

            return status;
        }
        public bool IntersectSphere(out MyVector intersectionPoint, out MyVector intersectionNormal,
            out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionSphere sphere)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            collisionTime = 0;
            MyVector rVelocity = (endPoint - startPoint) / dt;
            MyVector translate = (endPoint - startPoint);

            int nPoints = 0;
            //bool status = false;
            MyVector iPoint;
            MyVector n1, n2, n3;
            MyVector sPoint;
            MyVector ePoint;
            float dist, speed, d1, d2, d3;

            // check for face collision
            for (int i = 0; i < m_faces.Length; i++)
            {

                sPoint = startPoint - m_faces[i].n * sphere.Radius;
                ePoint = endPoint - m_faces[i].n * sphere.Radius;


                if ((ePoint - m_vertices[m_faces[i].v1]) * m_faces[i].n > 0)
                {
                    //the line doesn't cross the triangle
                    continue;
                }
                dist = m_faces[i].n * (sPoint - m_vertices[m_faces[i].v1]);
                speed = -dist / (m_faces[i].n * rVelocity);

                if (speed < 0)
                {
                    if (translate * (m_faces[i].n)<0)
                    {
                        if (dist < -0.5f)
                            continue;
                    }
                    else
                        continue;
                }
                if (nPoints > 0 && speed > collisionTime)
                    continue;
                iPoint = sPoint + speed * rVelocity;

                //3 planes around triangle
                //plane1
                n1 = ((m_vertices[m_faces[i].v2] - m_vertices[m_faces[i].v1]) ^ m_faces[i].n).Normalize();
                d1 = n1 * (m_vertices[m_faces[i].v1]);

                //plane2
                n2 = ((m_vertices[m_faces[i].v3] - m_vertices[m_faces[i].v2]) ^ m_faces[i].n).Normalize();
                d2 = n2 * (m_vertices[m_faces[i].v2]);

                //plane3
                n3 = ((m_vertices[m_faces[i].v1] - m_vertices[m_faces[i].v3]) ^ m_faces[i].n).Normalize();
                d3 = n3 * (m_vertices[m_faces[i].v3]);

                float x1 = n1 * iPoint - d1;
                float x2 = n2 * iPoint - d2;
                float x3 = n3 * iPoint - d3;
                if (x1 <= 0 && x2 <= 0 && x3 <= 0)
                {
                    //if (status == false)
                    //{
                    //    status = true;
                    //    intersectionPoint = iPoint;
                    //    intersectionNormal = m_faces[i].n;
                    //    collisionTime = speed;
                    //}
                    //else if (speed < collisionTime)
                    //{
                    //    intersectionPoint = iPoint;
                    //    intersectionNormal = m_faces[i].n;
                    //    collisionTime = speed;
                    //}
                    if (nPoints == 0)
                    {
                        nPoints = 1;

                        intersectionPoint = iPoint;
                        intersectionNormal = m_faces[i].n;
                        collisionTime = speed;
                    }
                    else if (speed < collisionTime)
                    {
                        nPoints = 1;

                        intersectionPoint = iPoint;
                        intersectionNormal = m_faces[i].n;
                        collisionTime = speed;
                    }
                    else if (speed == collisionTime && nPoints > 0)
                    {
                        nPoints++;
                        intersectionPoint.Add(iPoint);
                        intersectionNormal.Add(m_faces[i].n);
                    }
                }

            }
            //if (nPoints > 1)
            //{
            //    intersectionPoint.Divide(nPoints);
            //    intersectionNormal.Divide(nPoints);
            //}


            //if (status == true)
            //{
            //    return true;
            //}

            //status = false;

            // no face collision
            //check for edge collision

            foreach (CEdge e in m_edges)
            {
                MyVector DV, DVP;
                DV = m_vertices[e.v2] - m_vertices[e.v1];
                DVP = m_vertices[e.v1] - startPoint;
                double a = (double)DV.LengthSq * (double)rVelocity.LengthSq - (double)(DV * rVelocity) * (double)(DV * rVelocity);
                double b = -2 * (double)((double)DV.LengthSq * (double)(DVP * rVelocity) - (double)(DVP * DV) * (double)(rVelocity * DV));
                double c = (double)DV.LengthSq * (double)DVP.LengthSq - (double)(DV * DVP) * (double)(DV * DVP) - (double)(sphere.Radius * sphere.Radius) * (double)DV.LengthSq;
                double delta = b * b - 4 * a * c;
                if (delta >= 0)
                {

                    double t;
                    if (a > 0)
                    {
                        t = (-b - Math.Sqrt(delta)) / (2 * a);
                    }
                    else
                    {
                        t = (-b + Math.Sqrt(delta)) / (2 * a);
                    }
                    if (t < -0.04)
                        continue;
                    if (nPoints > 0 && t > collisionTime)
                        continue;

                    iPoint = startPoint + (float)t * rVelocity;
                    iPoint.Add((DV ^ (DVP ^ DV)).Normalize() * sphere.Radius);
                    if ((iPoint - m_vertices[e.v1]) * DV >= 0 && (iPoint - m_vertices[e.v2]) * DV <= 0)
                    {

                        //intersectionPoint = iPoint;
                        //intersectionNormal = startPoint - intersectionPoint;
                        //intersectionNormal.Normalize();

                        //status = true;
                        //collisionTime = t;
                        if (nPoints == 0)
                        {
                            nPoints = 1;

                            intersectionPoint = iPoint;
                            intersectionNormal = startPoint - intersectionPoint;
                            intersectionNormal.Normalize();
                            collisionTime = (float)t;
                        }
                        else if (t < collisionTime)
                        {
                            nPoints = 1;

                            intersectionPoint = iPoint;
                            intersectionNormal = startPoint - intersectionPoint;
                            intersectionNormal.Normalize();
                            collisionTime = (float)t;
                        }
                        else if (t == collisionTime && nPoints > 0)
                        {
                            nPoints++;
                            intersectionPoint.Add(iPoint);
                            intersectionNormal.Add((startPoint - intersectionPoint).Normalize());
                        }
                    }

                }
            }
            //if (status == true)
            //    return true;
            //status = false;
            //no edge collision
            //check for vertex collision
            for (int i = 0; i < m_vertices.Length; i++)
            {
                MyVector DVP;
                DVP = m_vertices[i] - startPoint;
                double a = rVelocity.LengthSq;
                double b = -2 * (DVP * rVelocity);
                double c = DVP.LengthSq - (sphere.Radius * sphere.Radius);
                double delta = b * b - 4 * a * c;
                if (delta >= 0)
                {

                    double t;
                    if (a > 0)
                    {
                        t = (-b - Math.Sqrt(delta)) / (2 * a);
                    }
                    else
                    {
                        t = (-b + Math.Sqrt(delta)) / (2 * a);
                    }
                    if (t < -0.04)
                        continue;
                    if (nPoints > 0 && t > collisionTime)
                        continue;
                    //if (status == false || (status == true && collisionTime > t))
                    //{
                    //    intersectionPoint = m_vertices[i];
                    //    intersectionNormal = startPoint - intersectionPoint;
                    //    intersectionNormal.Normalize();

                    //    status = true;
                    //    collisionTime = (float)t;
                    //}
                    if (nPoints == 0)
                    {
                        nPoints = 1;

                        intersectionPoint = m_vertices[i];
                        intersectionNormal = startPoint - intersectionPoint;
                        intersectionNormal.Normalize();
                        collisionTime = (float)t;
                    }
                    else if (t < collisionTime)
                    {
                        nPoints = 1;

                        intersectionPoint = m_vertices[i];
                        intersectionNormal = startPoint - intersectionPoint;
                        intersectionNormal.Normalize();
                        collisionTime = (float)t;
                    }
                    else if (t == collisionTime && nPoints > 0)
                    {
                        nPoints++;
                        intersectionPoint.Add(m_vertices[i]);
                        intersectionNormal.Add((startPoint - intersectionPoint).Normalize());
                    }
                }
            }
            if (nPoints > 1)
            {
                intersectionPoint.Divide(nPoints);
                intersectionNormal.Normalize();
            }


            return (nPoints > 0);
        }






        public bool IntersectPlane(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionPlane plane)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = -plane.Normal;
            MyVector rVelocity = (endPoint - startPoint) / dt;
            MyVector pVel = (rVelocity * plane.Normal) * plane.Normal;
            MyVector translate = (endPoint - startPoint);
            collisionTime = 0;

            float speed;
            float nPoints = 0;

            for (int i = 0; i < m_hardPoints.Length; i++)
            {
                if ((m_hardPoints[i] - endPoint) * pVel > 0)
                {
                    //the line doesn't cross the triangle
                    continue;
                }
                speed = pVel * (m_hardPoints[i] - startPoint);
                if (speed < -0.04)
                    continue;

                if (nPoints > 0 && speed > collisionTime)
                    continue;
                if (nPoints == 0)
                {
                    nPoints = 1;
                    intersectionPoint = m_hardPoints[i];
                    collisionTime = speed;
                }
                else if (speed < collisionTime)
                {
                    nPoints = 1;
                    intersectionPoint = m_hardPoints[i];
                    collisionTime = speed;
                }
                else if (speed == collisionTime && nPoints > 0)
                {
                    nPoints++;
                    intersectionPoint.Add(m_hardPoints[i]);
                }

            }
            if (nPoints > 1)
            {
                intersectionPoint.Divide(nPoints);
            }
            return (nPoints > 0);
        }

        public bool IntersectMesh(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionMesh mesh)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            MyVector translate = (endPoint - startPoint);
            collisionTime = 0;
            int nPoints = 0;

            MyVector rVelocity=new MyVector();
            MyVector rMove = endPoint - startPoint;
            MyVector iPoint;
            MyVector n1, n2, n3;
            float dist, speed, d1, d2, d3;


            for (int v = 0; v < mesh.m_hardPoints.Length; v++)
            {
                MyVector sPoint = startPoint + mesh.m_hardPoints[v];

                MyVector axis = mesh.MeshRotation;
                axis.Normalize();
                MyQuaternion rot = MyQuaternion.FromAxisAngle((dt * mesh.MeshRotation).Length, axis);
                
                //endPoint.Rotate(rot);
                //endPoint += relativeVelocity * dt;
                MyVector tmp=mesh.m_hardPoints[v];
                tmp.Rotate(rot);
                MyVector ePoint = startPoint + tmp + rMove;

                rVelocity = (ePoint - sPoint) / dt;
                translate = (ePoint - sPoint);
                for (int i = 0; i < m_faces.Length; i++)
                {
                    if ((ePoint - m_vertices[m_faces[i].v1]) * m_faces[i].n > 0)
                    {
                        //the line doesn't cross the triangle
                        continue;
                    }
                    dist = m_faces[i].n * (sPoint - m_vertices[m_faces[i].v1]);
                    speed = -dist / (m_faces[i].n * rVelocity);
                    if (speed < 0)
                    {
                        if (translate * (m_faces[i].n)<0)
                        {
                            if (dist < -0.5f)
                                continue;
                        }
                        else
                            continue;
                    }
                    iPoint = sPoint + speed * rVelocity;


                    //3 planes around triangle
                    //plane1
                    n1 = ((m_vertices[m_faces[i].v2] - m_vertices[m_faces[i].v1]) ^ m_faces[i].n).Normalize();
                    d1 = n1 * (m_vertices[m_faces[i].v1]);

                    //plane2
                    n2 = ((m_vertices[m_faces[i].v3] - m_vertices[m_faces[i].v2]) ^ m_faces[i].n).Normalize();
                    d2 = n2 * (m_vertices[m_faces[i].v2]);

                    //plane3
                    n3 = ((m_vertices[m_faces[i].v1] - m_vertices[m_faces[i].v3]) ^ m_faces[i].n).Normalize();
                    d3 = n3 * (m_vertices[m_faces[i].v3]);

                    float x1 = n1 * iPoint - d1;
                    float x2 = n2 * iPoint - d2;
                    float x3 = n3 * iPoint - d3;

                    if (x1 <= 0 && x2 <= 0 && x3 <= 0)
                    {

                        if (nPoints == 0)
                        {
                            nPoints = 1;

                            intersectionPoint = iPoint;
                            intersectionNormal = m_faces[i].n;
                            collisionTime = speed;
                        }
                        else if (speed < collisionTime)
                        {
                            nPoints = 1;

                            intersectionPoint = iPoint;
                            intersectionNormal = m_faces[i].n;
                            collisionTime = speed;
                        }
                        else if (speed == collisionTime && nPoints > 0)
                        {
                            nPoints++;
                            intersectionPoint.Add(iPoint);
                            intersectionNormal.Add(m_faces[i].n);
                        }
                    }
                }


            }

            //second
            //for (int v = 0; v < m_hardPoints.Length; v++)
            //{
            //    //MyVector sPoint = -startPoint + m_hardPoints[v];
            //    //MyVector ePoint = -endPoint + m_hardPoints[v];
            //    MyVector sPoint = -startPoint + m_hardPoints[v];

            //    MyVector axis = -mesh.MeshRotation;
            //    axis.Normalize();
            //    MyQuaternion rot = MyQuaternion.FromAxisAngle((dt * mesh.MeshRotation).Length, axis);

            //    //endPoint.Rotate(rot);
            //    //endPoint += relativeVelocity * dt;
            //    MyVector tmp = m_hardPoints[v];
            //    tmp.Rotate(rot);
            //    MyVector ePoint = -startPoint + tmp - rMove;

            //    rVelocity = (ePoint - sPoint) / dt;
            //    translate = (ePoint - sPoint);
            //    for (int i = 0; i < mesh.m_faces.Length; i++)
            //    {

            //        if ((ePoint - mesh.m_vertices[mesh.m_faces[i].v1]) * mesh.m_faces[i].n > 0)
            //        {
            //            //the line doesn't cross the triangle
            //            continue;
            //        }
            //        dist = mesh.m_faces[i].n * (sPoint - mesh.m_vertices[mesh.m_faces[i].v1]);
            //        speed = -dist / (mesh.m_faces[i].n * (-rVelocity));
            //        if (speed < 0)
            //        {
            //            if (translate * (mesh.m_faces[i].n) < 0)
            //            {
            //                if (speed < -0.04)
            //                    continue;
            //            }
            //            else
            //                continue;
            //        }
            //        iPoint = sPoint + speed * (-rVelocity);

            //        //3 planes around triangle
            //        //plane1
            //        n1 = ((mesh.m_vertices[mesh.m_faces[i].v2] - mesh.m_vertices[mesh.m_faces[i].v1]) ^ mesh.m_faces[i].n).Normalize();
            //        d1 = n1 * (mesh.m_vertices[mesh.m_faces[i].v1]);

            //        //plane2
            //        n2 = ((mesh.m_vertices[mesh.m_faces[i].v3] - mesh.m_vertices[mesh.m_faces[i].v2]) ^ mesh.m_faces[i].n).Normalize();
            //        d2 = n2 * (mesh.m_vertices[mesh.m_faces[i].v2]);

            //        //plane3
            //        n3 = ((mesh.m_vertices[mesh.m_faces[i].v1] - mesh.m_vertices[mesh.m_faces[i].v3]) ^ mesh.m_faces[i].n).Normalize();
            //        d3 = n3 * (mesh.m_vertices[mesh.m_faces[i].v3]);

            //        float x1 = n1 * iPoint - d1;
            //        float x2 = n2 * iPoint - d2;
            //        float x3 = n3 * iPoint - d3;
            //        if (x1 <= 0 && x2 <= 0 && x3 <= 0)
            //        {

            //            if (nPoints == 0)
            //            {
            //                nPoints = 1;

            //                intersectionPoint = iPoint + startPoint;
            //                intersectionNormal = -mesh.m_faces[i].n;
            //                collisionTime = speed;
            //            }
            //            else if (speed < collisionTime)
            //            {
            //                nPoints = 1;

            //                intersectionPoint = iPoint + startPoint;
            //                intersectionNormal = -mesh.m_faces[i].n;
            //                collisionTime = speed;
            //            }
            //            else if (speed == collisionTime && nPoints > 0)
            //            {
            //                nPoints++;
            //                intersectionPoint.Add(iPoint + startPoint);
            //                intersectionNormal.Add(-mesh.m_faces[i].n);
            //            }
            //        }
            //    }
            //}

            if (nPoints > 1)
            {
                intersectionPoint.Divide(nPoints);
                intersectionNormal.Normalize();
            }


            return (nPoints > 0);
        }

        public object Clone()
        {
            CollisionMesh newMesh = new CollisionMesh();
            newMesh.boundingShpereRadius = boundingShpereRadius;
            newMesh.m_vertices = new MyVector[m_vertices.Length];
            for (int i = 0; i < m_vertices.Length; i++)
            {
                newMesh.m_vertices[i] = m_vertices[i];
            }
            newMesh.m_faces = new CFace[m_faces.Length];
            for (int i = 0; i < m_faces.Length; i++)
            {
                newMesh.m_faces[i] = m_faces[i];
            }
            newMesh.m_edges = new CEdge[m_edges.Length];
            for (int i = 0; i < m_edges.Length; i++)
            {
                newMesh.m_edges[i] = m_edges[i];
            }
            newMesh.m_hardPoints = new MyVector[m_hardPoints.Length];
            for (int i = 0; i < m_hardPoints.Length; i++)
            {
                newMesh.m_hardPoints[i] = m_hardPoints[i];
            }

            newMesh.MeshRotation = this.MeshRotation;
            return newMesh;
        }

        #endregion
    }

}
