using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{
    class OctreeNode
    {
        public MyVector Center;
        public float Size;

        public IndexedCollisionMesh Mesh;
        public OctreeNode[] Children;

        public bool ContainsPoint(MyVector p)
        {
            return ((Center.X - Size / 2 <= p.X && Center.X + Size / 2 >= p.X) && (Center.Y - Size / 2 <= p.Y && Center.Y + Size / 2 >= p.Y)
                && (Center.Z - Size / 2 <= p.Z && Center.Z + Size / 2 >= p.Z));
        }
        public bool ContainsTriangle(MyVector v1, MyVector v2, MyVector v3)
        {
            if (ContainsPoint(v1) || ContainsPoint(v2) || ContainsPoint(v3))
                return true;

            if ((v1 - v2).Length >= Size || (v1 - v3).Length >= Size || (v2 - v3).Length >= Size)
            {
                MyVector c1 = (v1 + v2) / 2;
                MyVector c2 = (v1 + v3) / 2;
                MyVector c3 = (v3 + v2) / 2;
                return (ContainsTriangle(v1, c1, c2) || ContainsTriangle(v2, c1, c3) || ContainsTriangle(v3, c2, c3) ||
                    ContainsTriangle(c1, c2, c3));
            }
            else
                return false;



        }
    }

    class IndexedCollisionMesh : ICollisionData
    {
        public CollisionOctree tree;
        public uint[] vertices;
        public uint[] hardPoints;
        public uint[] faces;
        public uint[] edges;



        #region ICollisionData Members

        public float BoundingSphereRadius
        {
            get
            {
                return tree.BoundingSphereRadius;
            }
        }

        public ICollisionData Rotate(MyQuaternion orientation)
        {
            throw new Exception("What a waste of CPU power...wtf?? do you want to rotate this??");
        }

        public bool IntersectPoint(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            MyVector translate = (endPoint - startPoint);
            collisionTime = 0;
            int nPoints = 0;

            MyVector rVelocity = new MyVector();
            MyVector rMove = endPoint - startPoint;
            MyVector iPoint;
            MyVector n1, n2, n3;
            float dist, speed, d1, d2, d3;


            MyVector sPoint = startPoint;


            MyVector ePoint = endPoint;

            rVelocity = (ePoint - sPoint) / dt;
            translate = (ePoint - sPoint);
            for (int j = 0; j < faces.Length; j++)
            {
                int i = (int)faces[j];
                if ((ePoint - tree.vertices[tree.faces[i].v1]) * tree.faces[i].n > 0)
                {
                    //the line doesn't cross the triangle
                    continue;
                }
                dist = tree.faces[i].n * (sPoint - tree.vertices[tree.faces[i].v1]);
                speed = -dist / (tree.faces[i].n * rVelocity);
                if (speed < 0)
                {
                    if (translate * (tree.faces[i].n) < 0)
                    {
                        if (dist < -tree.safeDepth)
                            continue;
                    }
                    else
                        continue;
                }
                iPoint = sPoint + speed * rVelocity;


                //3 planes around triangle
                //plane1
                n1 = ((tree.vertices[tree.faces[i].v2] - tree.vertices[tree.faces[i].v1]) ^ tree.faces[i].n).Normalize();
                d1 = n1 * (tree.vertices[tree.faces[i].v1]);

                //plane2
                n2 = ((tree.vertices[tree.faces[i].v3] - tree.vertices[tree.faces[i].v2]) ^ tree.faces[i].n).Normalize();
                d2 = n2 * (tree.vertices[tree.faces[i].v2]);

                //plane3
                n3 = ((tree.vertices[tree.faces[i].v1] - tree.vertices[tree.faces[i].v3]) ^ tree.faces[i].n).Normalize();
                d3 = n3 * (tree.vertices[tree.faces[i].v3]);

                float x1 = n1 * iPoint - d1;
                float x2 = n2 * iPoint - d2;
                float x3 = n3 * iPoint - d3;

                if (x1 <= 0 && x2 <= 0 && x3 <= 0)
                {

                    if (nPoints == 0)
                    {
                        nPoints = 1;

                        intersectionPoint = iPoint;
                        intersectionNormal = tree.faces[i].n;
                        collisionTime = speed;
                    }
                    else if (speed < collisionTime)
                    {
                        nPoints = 1;

                        intersectionPoint = iPoint;
                        intersectionNormal = tree.faces[i].n;
                        collisionTime = speed;
                    }
                    else if (speed == collisionTime && nPoints > 0)
                    {
                        nPoints++;
                        intersectionPoint.Add(iPoint);
                        intersectionNormal.Add(tree.faces[i].n);
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

        public bool IntersectSphere(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionSphere sphere)
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
            for (int j = 0; j < faces.Length; j++)
            {
                int i = (int)faces[j];
                sPoint = startPoint - tree.faces[i].n * sphere.Radius;
                ePoint = endPoint - tree.faces[i].n * sphere.Radius;


                if ((ePoint - tree.vertices[tree.faces[i].v1]) * tree.faces[i].n > 0)
                {
                    //the line doesn't cross the triangle
                    continue;
                }
                dist = tree.faces[i].n * (sPoint - tree.vertices[tree.faces[i].v1]);
                speed = -dist / (tree.faces[i].n * rVelocity);

                if (speed < 0)
                {
                    if (translate * (-tree.faces[i].n) < 0)
                    {
                        if (dist < -tree.safeDepth)
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
                n1 = ((tree.vertices[tree.faces[i].v2] - tree.vertices[tree.faces[i].v1]) ^ tree.faces[i].n).Normalize();
                d1 = n1 * (tree.vertices[tree.faces[i].v1]);

                //plane2
                n2 = ((tree.vertices[tree.faces[i].v3] - tree.vertices[tree.faces[i].v2]) ^ tree.faces[i].n).Normalize();
                d2 = n2 * (tree.vertices[tree.faces[i].v2]);

                //plane3
                n3 = ((tree.vertices[tree.faces[i].v1] - tree.vertices[tree.faces[i].v3]) ^ tree.faces[i].n).Normalize();
                d3 = n3 * (tree.vertices[tree.faces[i].v3]);

                float x1 = n1 * iPoint - d1;
                float x2 = n2 * iPoint - d2;
                float x3 = n3 * iPoint - d3;
                if (x1 <= 0 && x2 <= 0 && x3 <= 0)
                {
                    //if (status == false)
                    //{
                    //    status = true;
                    //    intersectionPoint = iPoint;
                    //    intersectionNormal = tree.faces[i].n;
                    //    collisionTime = speed;
                    //}
                    //else if (speed < collisionTime)
                    //{
                    //    intersectionPoint = iPoint;
                    //    intersectionNormal = tree.faces[i].n;
                    //    collisionTime = speed;
                    //}
                    if (nPoints == 0)
                    {
                        nPoints = 1;

                        intersectionPoint = iPoint;
                        intersectionNormal = tree.faces[i].n;
                        collisionTime = speed;
                    }
                    else if (speed < collisionTime)
                    {
                        nPoints = 1;

                        intersectionPoint = iPoint;
                        intersectionNormal = tree.faces[i].n;
                        collisionTime = speed;
                    }
                    else if (speed == collisionTime && nPoints > 0)
                    {
                        nPoints++;
                        intersectionPoint.Add(iPoint);
                        intersectionNormal.Add(tree.faces[i].n);
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

            for (int j = 0; j < edges.Length; j++)
            {
                CEdge e = tree.edges[edges[j]];
                MyVector DV, DVP;
                DV = tree.vertices[e.v2] - tree.vertices[e.v1];
                DVP = tree.vertices[e.v1] - startPoint;
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
                    if ((iPoint - tree.vertices[e.v1]) * DV >= 0 && (iPoint - tree.vertices[e.v2]) * DV <= 0)
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
            for (int j = 0; j < vertices.Length; j++)
            {
                int i = (int)vertices[j];
                MyVector DVP;
                DVP = tree.vertices[i] - startPoint;
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
                    //    intersectionPoint = tree.vertices[i];
                    //    intersectionNormal = startPoint - intersectionPoint;
                    //    intersectionNormal.Normalize();

                    //    status = true;
                    //    collisionTime = (float)t;
                    //}
                    if (nPoints == 0)
                    {
                        nPoints = 1;

                        intersectionPoint = tree.vertices[i];
                        intersectionNormal = startPoint - intersectionPoint;
                        intersectionNormal.Normalize();
                        collisionTime = (float)t;
                    }
                    else if (t < collisionTime)
                    {
                        nPoints = 1;

                        intersectionPoint = tree.vertices[i];
                        intersectionNormal = startPoint - intersectionPoint;
                        intersectionNormal.Normalize();
                        collisionTime = (float)t;
                    }
                    else if (t == collisionTime && nPoints > 0)
                    {
                        nPoints++;
                        intersectionPoint.Add(tree.vertices[i]);
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
            throw new NotImplementedException();
        }

        public bool IntersectMesh(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionMesh mesh)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            collisionTime = 0;
            int nPoints = 0;
            if (startPoint.X == endPoint.X && startPoint.Y == endPoint.Y && startPoint.Z == endPoint.Z &&
                mesh.MeshRotation.X == 0 && mesh.MeshRotation.Y == 0 && mesh.MeshRotation.Z == 0)
                return false;
            MyVector translate = (endPoint - startPoint);


            MyVector rVelocity = new MyVector();
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
                MyVector tmp = mesh.m_hardPoints[v];
                tmp.Rotate(rot);
                MyVector ePoint = startPoint + tmp + rMove;

                rVelocity = (ePoint - sPoint) / dt;
                translate = (ePoint - sPoint);
                for (int j = 0; j < faces.Length; j++)
                {
                    int i = (int)faces[j];
                    if ((ePoint - tree.vertices[tree.faces[i].v1]) * tree.faces[i].n > 0)
                    {
                        //the line doesn't cross the triangle
                        continue;
                    }
                    dist = tree.faces[i].n * (sPoint - tree.vertices[tree.faces[i].v1]);
                    speed = -dist / (tree.faces[i].n * rVelocity);

                    if (speed < 0)
                    {
                        if (translate * (tree.faces[i].n) < 0)
                        {
                            if (dist < -tree.safeDepth)
                                continue;
                        }
                        else
                            continue;
                    }
                    iPoint = sPoint + speed * rVelocity;


                    //3 planes around triangle
                    //plane1
                    n1 = ((tree.vertices[tree.faces[i].v2] - tree.vertices[tree.faces[i].v1]) ^ tree.faces[i].n).Normalize();
                    d1 = n1 * (tree.vertices[tree.faces[i].v1]);

                    //plane2
                    n2 = ((tree.vertices[tree.faces[i].v3] - tree.vertices[tree.faces[i].v2]) ^ tree.faces[i].n).Normalize();
                    d2 = n2 * (tree.vertices[tree.faces[i].v2]);

                    //plane3
                    n3 = ((tree.vertices[tree.faces[i].v1] - tree.vertices[tree.faces[i].v3]) ^ tree.faces[i].n).Normalize();
                    d3 = n3 * (tree.vertices[tree.faces[i].v3]);

                    float x1 = n1 * iPoint - d1;
                    float x2 = n2 * iPoint - d2;
                    float x3 = n3 * iPoint - d3;

                    if (x1 <= 0 && x2 <= 0 && x3 <= 0)
                    {

                        if (nPoints == 0)
                        {
                            nPoints = 1;

                            intersectionPoint = iPoint;
                            intersectionNormal = tree.faces[i].n;
                            collisionTime = speed;
                        }
                        else if (speed < collisionTime)
                        {
                            nPoints = 1;

                            intersectionPoint = iPoint;
                            intersectionNormal = tree.faces[i].n;
                            collisionTime = speed;
                        }
                        else if (speed == collisionTime && nPoints > 0)
                        {
                            nPoints++;
                            intersectionPoint.Add(iPoint);
                            intersectionNormal.Add(tree.faces[i].n);
                        }
                    }
                }


            }

            //second
            //for (int vv = 0; vv < hardPoints.Length; vv++)
            //{
            //    uint v = hardPoints[vv];
            //    //MyVector sPoint = -startPoint + m_hardPoints[v];
            //    //MyVector ePoint = -endPoint + m_hardPoints[v];
            //    MyVector sPoint = -startPoint + tree.hardPoints[v];

            //    MyVector axis = -mesh.MeshRotation;
            //    axis.Normalize();
            //    MyQuaternion rot = MyQuaternion.FromAxisAngle((dt * mesh.MeshRotation).Length, axis);

            //    //endPoint.Rotate(rot);
            //    //endPoint += relativeVelocity * dt;
            //    MyVector tmp = tree.hardPoints[v];
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
            //        speed = -dist / (mesh.m_faces[i].n * (rVelocity));
            //        if (speed < 0)
            //        {
            //            if (translate * (mesh.m_faces[i].n) < 0)
            //            {
            //                if (dist < -tree.safeDepth)
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

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class CollisionOctree : ICollisionData
    {
        public MyVector[] vertices;
        public MyVector[] hardPoints;
        public CFace[] faces;
        public CEdge[] edges;

        OctreeNode rootNode;
        OctreeNode[] findResults = new OctreeNode[72];

        public float safeDepth;
        CollisionOctree()
        {
        }

        public float BoundingSphereRadius
        {
            get
            {
                if (rootNode != null)
                    return (rootNode.Size / 2) * 1.42f + rootNode.Center.Length;
                else
                    return 0;
            }
        }
        private int CompactFindResults()
        {
            int i = 0, j = findResults.Length - 1;
            while (j >= 0 && findResults[j] == null)
            {
                j--;
            }
            if (j == -1)
                return 0;
            while (i < j)
            {
                if (findResults[i] == null)
                {
                    findResults[i] = findResults[j];
                    findResults[j] = null;
                    while (findResults[j] == null)
                    {
                        j--;
                    }
                }
                i++;
            }
            return j + 1;
        }

        private void FindNode(MyVector position, float radius)
        {
            OctreeNode node = null;

            node = rootNode;
            findResults[0] = rootNode;
            for (int i = 1; i < findResults.Length; i++)
                findResults[i] = null;
            int resultCount = 1;
            int processedResults = 0;
            float currentSize = node.Size;
            while (currentSize / 2 > radius && processedResults < resultCount)
            {
                int n = resultCount - 1;
                currentSize /= 2;
                while (n >= 0)
                {
                    if (findResults[n].Children != null)
                    {
                        node = findResults[n];
                        findResults[n] = null;
                        for (int i = 0; i < 8; i++)
                        {
                            if (node.Children[i] != null)
                            {
                                if ((Math.Abs(node.Children[i].Center.X - position.X) <
                                    radius + node.Children[i].Size / 2) &&
                                    (Math.Abs(node.Children[i].Center.Y - position.Y) <
                                    radius + node.Children[i].Size / 2) &&
                                    (Math.Abs(node.Children[i].Center.Z - position.Z) <
                                    radius + node.Children[i].Size / 2))
                                {
                                    findResults[resultCount++] = node.Children[i];

                                }
                            }
                        }
                    }
                    else
                    {
                        processedResults++;
                    }
                    n--;
                }
                resultCount = CompactFindResults();

            }
        }
        #region ICollisionData Members

        public ICollisionData Rotate(Framework.MyMath.MyQuaternion orientation)
        {
            throw new Exception("What a waste of CPU power...wtf?? do you want to rotate this??");
        }

        public bool IntersectPoint(out Framework.MyMath.MyVector intersectionPoint, out Framework.MyMath.MyVector intersectionNormal, out float collisionTime, Framework.MyMath.MyVector startPoint, Framework.MyMath.MyVector endPoint, float dt)
        {
            FindNode((startPoint + endPoint) / 2, (startPoint - endPoint).Length);
            intersectionNormal = new MyVector();
            intersectionPoint = new MyVector();
            collisionTime = -1;
            int i = 0;
            MyVector i_p, i_n;
            float ct;
            float min_ct = dt + 1;
            int npoints = 0;
            while (findResults[i] != null)
            {
                if (findResults[i].Mesh.IntersectPoint(out i_p, out i_n, out ct, startPoint, endPoint, dt))
                {
                    if (ct < min_ct)
                    {
                        intersectionPoint = i_p;
                        intersectionNormal = i_n;
                        collisionTime = ct;
                        npoints = 1;
                        min_ct = ct;
                    }
                    else if (ct == min_ct)
                    {
                        intersectionPoint += i_p;
                        intersectionNormal += i_n;
                        npoints++;
                    }
                }
                i++;
            }
            if (npoints > 0)
            {
                intersectionPoint /= npoints;
                intersectionNormal.Normalize();
                return true;
            }
            else
                return false;

        }

        public bool IntersectSphere(out Framework.MyMath.MyVector intersectionPoint, out Framework.MyMath.MyVector intersectionNormal, out float collisionTime, Framework.MyMath.MyVector startPoint, Framework.MyMath.MyVector endPoint, float dt, CollisionSphere sphere)
        {
            FindNode((startPoint + endPoint) / 2, (startPoint - endPoint).Length);
            intersectionNormal = new MyVector();
            intersectionPoint = new MyVector();
            collisionTime = -1;
            int i = 0;
            MyVector i_p, i_n;
            float ct;
            float min_ct = dt + 1;
            int npoints = 0;
            while (findResults[i] != null)
            {
                if (findResults[i].Mesh.IntersectSphere(out i_p, out i_n, out ct, startPoint, endPoint, dt, sphere))
                {
                    if (ct < min_ct)
                    {
                        intersectionPoint = i_p;
                        intersectionNormal = i_n;
                        collisionTime = ct;
                        npoints = 1;
                        min_ct = ct;
                    }
                    else if (ct == min_ct)
                    {
                        intersectionPoint += i_p;
                        intersectionNormal += i_n;
                        npoints++;
                    }
                }
                i++;
            }
            if (npoints > 0)
            {
                intersectionPoint /= npoints;
                intersectionNormal.Normalize();
                return true;
            }
            else
                return false;

        }

        public bool IntersectPlane(out Framework.MyMath.MyVector intersectionPoint, out Framework.MyMath.MyVector intersectionNormal, out float collisionTime, Framework.MyMath.MyVector startPoint, Framework.MyMath.MyVector endPoint, float dt, CollisionPlane plane)
        {
            throw new NotImplementedException();
        }

        public bool IntersectMesh(out Framework.MyMath.MyVector intersectionPoint, out Framework.MyMath.MyVector intersectionNormal, out float collisionTime, Framework.MyMath.MyVector startPoint, Framework.MyMath.MyVector endPoint, float dt, CollisionMesh mesh)
        {
            FindNode((startPoint + endPoint) / 2, (startPoint - endPoint).Length + mesh.BoundingSphereRadius);
            intersectionNormal = new MyVector();
            intersectionPoint = new MyVector();
            collisionTime = -1;
            int i = 0;
            MyVector i_p, i_n;
            float ct;
            float min_ct = dt + 1;
            int npoints = 0;
            while (findResults[i] != null)
            {
                if (findResults[i].Mesh.IntersectMesh(out i_p, out i_n, out ct, startPoint, endPoint, dt, mesh))
                {
                    if (ct < min_ct)
                    {
                        intersectionPoint = i_p;
                        intersectionNormal = i_n;
                        collisionTime = ct;
                        npoints = 1;
                        min_ct = ct;
                    }
                    else if (ct == min_ct)
                    {
                        intersectionPoint += i_p;
                        intersectionNormal += i_n;
                        npoints++;
                    }
                }
                i++;
            }
            if (npoints > 0)
            {
                intersectionPoint /= npoints;
                intersectionNormal.Normalize();
                return true;
            }
            else
                return false;

        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        OctreeNode BuildNode(OctreeNode parent, MyVector center, float size, int maxTriangles, float minSize, int remainingDepth)
        {
            //if (remainingDepth == 0)
            //    return null;
            OctreeNode node = new OctreeNode();

            node.Center = center;
            node.Size = size;
            List<uint> vertices = new List<uint>();
            List<uint> hardPoints = new List<uint>();
            List<uint> faces = new List<uint>();
            List<uint> edges = new List<uint>();
            if (parent == null)
            {
                for (uint i = 0; i < this.vertices.Length; i++)
                {
                    vertices.Add(i);
                }
                for (uint i = 0; i < this.hardPoints.Length; i++)
                {
                    hardPoints.Add(i);
                }
                for (uint i = 0; i < this.faces.Length; i++)
                {
                    faces.Add(i);

                }
                for (uint i = 0; i < this.edges.Length; i++)
                {

                    edges.Add(i);
                }
            }
            else
            {




                for (uint i = 0; i < parent.Mesh.vertices.Length; i++)
                {
                    if (node.ContainsPoint(this.vertices[parent.Mesh.vertices[i]]))
                    {
                        vertices.Add(parent.Mesh.vertices[i]);
                    }
                }
                for (uint i = 0; i < parent.Mesh.hardPoints.Length; i++)
                {
                    if (node.ContainsPoint(this.hardPoints[parent.Mesh.hardPoints[i]]))
                    {
                        hardPoints.Add(parent.Mesh.hardPoints[i]);
                    }
                }
                for (uint i = 0; i < parent.Mesh.faces.Length; i++)
                {
                    //if (node.ContainsPoint(this.vertices[this.faces[parent.Mesh.faces[i]].v1]))
                    //{
                    //    faces.Add(parent.Mesh.faces[i]);
                    //}
                    //else if (node.ContainsPoint(this.vertices[this.faces[parent.Mesh.faces[i]].v2]))
                    //{
                    //    faces.Add(parent.Mesh.faces[i]);
                    //}
                    //else if (node.ContainsPoint(this.vertices[this.faces[parent.Mesh.faces[i]].v3]))
                    //{
                    //    faces.Add(parent.Mesh.faces[i]);
                    //}
                    if (node.ContainsTriangle(this.vertices[this.faces[parent.Mesh.faces[i]].v1],
                        this.vertices[this.faces[parent.Mesh.faces[i]].v2],
                        this.vertices[this.faces[parent.Mesh.faces[i]].v3]))
                    {
                        faces.Add(parent.Mesh.faces[i]);
                    }
                }
                for (uint i = 0; i < parent.Mesh.edges.Length; i++)
                {
                    if (node.ContainsPoint(this.vertices[this.edges[parent.Mesh.edges[i]].v1]))
                    {
                        edges.Add(parent.Mesh.edges[i]);
                    }
                    else if (node.ContainsPoint(this.vertices[this.edges[parent.Mesh.edges[i]].v2]))
                    {
                        edges.Add(parent.Mesh.edges[i]);
                    }
                }
            }
            node.Mesh = new IndexedCollisionMesh();
            node.Mesh.tree = this;
            node.Mesh.edges = edges.ToArray();
            node.Mesh.faces = faces.ToArray();
            node.Mesh.vertices = vertices.ToArray();
            node.Mesh.hardPoints = hardPoints.ToArray();
            //if (node.Mesh.vertices.Length == 0)
            //    return null;
            if (remainingDepth > 1)
            {
                MyVector up = new MyVector(0, size / 4, 0);
                MyVector right = new MyVector(size / 4, 0, 0);
                MyVector front = new MyVector(0, 0, size / 4);
                node.Children = new OctreeNode[8];

                node.Children[0] = BuildNode(node, center + up + right + front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[1] = BuildNode(node, center + up - right + front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[2] = BuildNode(node, center - up + right + front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[3] = BuildNode(node, center - up - right + front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[4] = BuildNode(node, center + up + right - front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[5] = BuildNode(node, center + up - right - front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[6] = BuildNode(node, center - up + right - front, size / 2, maxTriangles, minSize, remainingDepth - 1);
                node.Children[7] = BuildNode(node, center - up - right - front, size / 2, maxTriangles, minSize, remainingDepth - 1);

            }
            else if (node.Mesh.faces.Length > maxTriangles && size / 2 > minSize)
            {
                MyVector up = new MyVector(0, size / 4, 0);
                MyVector right = new MyVector(size / 4, 0, 0);
                MyVector front = new MyVector(0, 0, size / 4);
                node.Children = new OctreeNode[8];

                node.Children[0] = BuildNode(node, center + up + right + front, size / 2, maxTriangles, minSize, 1);
                node.Children[1] = BuildNode(node, center + up - right + front, size / 2, maxTriangles, minSize, 1);
                node.Children[2] = BuildNode(node, center - up + right + front, size / 2, maxTriangles, minSize, 1);
                node.Children[3] = BuildNode(node, center - up - right + front, size / 2, maxTriangles, minSize, 1);
                node.Children[4] = BuildNode(node, center + up + right - front, size / 2, maxTriangles, minSize, 1);
                node.Children[5] = BuildNode(node, center + up - right - front, size / 2, maxTriangles, minSize, 1);
                node.Children[6] = BuildNode(node, center - up + right - front, size / 2, maxTriangles, minSize, 1);
                node.Children[7] = BuildNode(node, center - up - right - front, size / 2, maxTriangles, minSize, 1);


            }
            return node;

        }

        public static CollisionOctree FromMesh(CollisionMesh mesh, float yBias,float xBias,float zBias, float minSize, int minDepth, int maxTriangles)
        {
            MyVector center = new MyVector(0, 0, 0);
            float Size = 0;
            CollisionOctree tree = new CollisionOctree();
            tree.safeDepth = 0.5f;
            for (int i = 0; i < mesh.m_vertices.Length; i++)
            {
                center += mesh.m_vertices[i];
            }
            center /= mesh.m_vertices.Length;
            center.Y += yBias;
            center.X += xBias;
            center.Z += zBias;
            for (int i = 0; i < mesh.m_vertices.Length; i++)
            {

                if (Math.Abs(mesh.m_vertices[i].X - center.X) > Size)
                    Size = (float)Math.Abs(mesh.m_vertices[i].X - center.X);
                if (Math.Abs(mesh.m_vertices[i].Y - center.Y) > Size)
                    Size = (float)Math.Abs(mesh.m_vertices[i].Y - center.Y);
                if (Math.Abs(mesh.m_vertices[i].Z - center.Z) > Size)
                    Size = (float)Math.Abs(mesh.m_vertices[i].Z - center.Z);
            }
            //float maxTriangleSize = 0;
            //for (int i = 0; i < mesh.m_faces.Length; i++)
            //{
            //    MyVector v1 = mesh.m_vertices[mesh.m_faces[i].v1];
            //    MyVector v2 = mesh.m_vertices[mesh.m_faces[i].v2];
            //    MyVector v3 = mesh.m_vertices[mesh.m_faces[i].v3];

            //    if ((v1 - v2).Length > maxTriangleSize)
            //        maxTriangleSize = (v1 - v2).Length;
            //    if ((v1 - v3).Length > maxTriangleSize)
            //        maxTriangleSize = (v1 - v3).Length;
            //    if ((v2 - v3).Length > maxTriangleSize)
            //        maxTriangleSize = (v2 - v3).Length;
            //}
            //if (minSize < maxTriangleSize)
            //    minSize = maxTriangleSize;
            Size *= 2;
            tree.vertices = mesh.m_vertices;
            tree.hardPoints = mesh.m_hardPoints;
            tree.faces = mesh.m_faces;
            tree.edges = mesh.m_edges;
            tree.rootNode = tree.BuildNode(null, center, Size, maxTriangles, minSize, minDepth);

            return tree;

        }
        //OctreeNode BuildNode(MyVector center, float size, int maxTriangles)
        //{
        //    OctreeNode node = new OctreeNode();

        //    node.Center = center;
        //    node.Size = size;
        //    List<uint> vertices = new List<uint>();
        //    List<uint> hardPoints = new List<uint>();
        //    List<uint> faces = new List<uint>();
        //    List<uint> edges = new List<uint>();
        //    safeDepth = 1f;

        //    for (uint i = 0; i < this.vertices.Length; i++)
        //    {
        //        if (node.ContainsPoint(this.vertices[i]))
        //        {
        //            vertices.Add(i);
        //        }
        //    }
        //    for (uint i = 0; i < this.hardPoints.Length; i++)
        //    {
        //        if (node.ContainsPoint(this.hardPoints[i]))
        //        {
        //            hardPoints.Add(i);
        //        }
        //    }
        //    for (uint i = 0; i < this.faces.Length; i++)
        //    {
        //        if (node.ContainsPoint(this.vertices[this.faces[i].v1]))
        //        {
        //            faces.Add(i);
        //        }
        //        else if (node.ContainsPoint(this.vertices[this.faces[i].v2]))
        //        {
        //            faces.Add(i);
        //        }
        //        else if (node.ContainsPoint(this.vertices[this.faces[i].v3]))
        //        {
        //            faces.Add(i);
        //        }
        //    }
        //    for (uint i = 0; i < this.edges.Length; i++)
        //    {
        //        if (node.ContainsPoint(this.vertices[this.edges[i].v1]))
        //        {
        //            edges.Add(i);
        //        }
        //        else if (node.ContainsPoint(this.vertices[this.edges[i].v2]))
        //        {
        //            edges.Add(i);
        //        }
        //    }
        //    node.Mesh = new IndexedCollisionMesh();
        //    node.Mesh.tree = this;
        //    node.Mesh.edges = edges.ToArray();
        //    node.Mesh.faces = faces.ToArray();
        //    node.Mesh.vertices = vertices.ToArray();
        //    node.Mesh.hardPoints = hardPoints.ToArray();
        //    if (node.Mesh.vertices.Length == 0)
        //        return null;
        //    if (node.Mesh.faces.Length > maxTriangles)
        //    {
        //        MyVector up = new MyVector(0, size / 4, 0);
        //        MyVector right = new MyVector(size / 4, 0, 0);
        //        MyVector front = new MyVector(0, 0, size / 4);
        //        node.Children = new OctreeNode[8];

        //        node.Children[0] = BuildNode(center + up + right + front, size / 2, maxTriangles);
        //        node.Children[1] = BuildNode(center + up - right + front, size / 2, maxTriangles);
        //        node.Children[2] = BuildNode(center - up + right + front, size / 2, maxTriangles);
        //        node.Children[3] = BuildNode(center - up - right + front, size / 2, maxTriangles);
        //        node.Children[4] = BuildNode(center + up + right - front, size / 2, maxTriangles);
        //        node.Children[5] = BuildNode(center + up - right - front, size / 2, maxTriangles);
        //        node.Children[6] = BuildNode(center - up + right - front, size / 2, maxTriangles);
        //        node.Children[7] = BuildNode(center - up - right - front, size / 2, maxTriangles);


        //    }
        //    return node;

        //}

        //public static CollisionOctree FromMesh2(CollisionMesh mesh, float yBias, float minSize, float maxDepth, int maxTriangles)
        //{
        //    MyVector center = new MyVector(0, 0, 0);
        //    float Size = 0;

        //    CollisionOctree tree = new CollisionOctree();
        //    tree.safeDepth = 10f;
        //    for (int i = 0; i < mesh.m_vertices.Length; i++)
        //    {
        //        center += mesh.m_vertices[i];
        //    }
        //    center /= mesh.m_vertices.Length;
        //    center.Y += yBias;
        //    for (int i = 0; i < mesh.m_vertices.Length; i++)
        //    {

        //        if (Math.Abs(mesh.m_vertices[i].X - center.X) > Size)
        //            Size = (float)Math.Abs(mesh.m_vertices[i].X - center.X);
        //        if (Math.Abs(mesh.m_vertices[i].Y - center.Y) > Size)
        //            Size = (float)Math.Abs(mesh.m_vertices[i].Y - center.Y);
        //        if (Math.Abs(mesh.m_vertices[i].Z - center.Z) > Size)
        //            Size = (float)Math.Abs(mesh.m_vertices[i].Z - center.Z);
        //    }
        //    Size *= 2;
        //    int MaxDepth = 3;
        //    tree.vertices = mesh.m_vertices;
        //    tree.hardPoints = mesh.m_hardPoints;
        //    tree.faces = mesh.m_faces;
        //    tree.edges = mesh.m_edges;
        //    tree.rootNode = tree.BuildNode(center, Size, maxTriangles);

        //    return tree;

        //}

    }
}
