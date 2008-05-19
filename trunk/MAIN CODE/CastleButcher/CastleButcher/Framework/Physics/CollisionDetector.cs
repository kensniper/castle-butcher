using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Framework.MyMath;

namespace Framework.Physics
{
    public delegate bool CollisionHandler(CollisionParameters parameters);
    public class CollisionDetector : Framework.Physics.ICollisionDetector
    {
        struct CollisionPair : IEquatable<CollisionPair>
        {
            public CollisionPair(IPhysicalObject a, IPhysicalObject b)
            {
                this.a = a;
                this.b = b;
            }
            IPhysicalObject a;
            IPhysicalObject b;

            #region IEquatable<CollisionPair> Members

            public bool Equals(CollisionPair other)
            {
                if ((a == other.a && b == other.b) || (b == other.a && a == other.b))
                    return true;
                else
                    return false;
            }

            #endregion
        }
        #region Fields
        List<IPhysicalObject> objects = new List<IPhysicalObject>();

        IntersectionQueue intersections = new IntersectionQueue();

        CollisionQueue collisions = new CollisionQueue();

        List<CollisionPair> resolvedCollisions = new List<CollisionPair>();

        RigidBodySimulator simulator;
        float currentTime;
        float endTime;


        //float m_collisionTime = 0.000f;
        //float m_collisionTimeTolerance = 0.001f;
        //float m_collisionTolerance = 0.1f;

        //int nf = 0;
        #endregion

        public event CollisionHandler OnPossibleCollision;
        public event CollisionHandler OnCollision;

        public CollisionDetector(RigidBodySimulator simulator)
        {
            this.simulator = simulator;
        }

        public void AddObject(IPhysicalObject obj)
        {
            objects.Add(obj);
        }
        public void RemoveObject(IPhysicalObject obj)
        {
            objects.Remove(obj);
        }

        //private bool MeshToSphere(out MyVector collisionPoint,out MyVector collisionNormal,
        //    out float TimeOfCollision, Intersection i)
        //{
        //    CollisionMesh mesh = (CollisionMesh)i.A.CollisionData;
        //    collisionPoint = new MyVector();
        //    collisionNormal = new MyVector();
        //    //float ctime;
        //    float dt=i.T1-i.A.CurrentTime;

        //    TimeOfCollision = 0;

        //    if (mesh.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //        i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity-i.A.Velocity)),
        //        i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), (CollisionSphere)i.B.CollisionData))
        //    {
        //        TimeOfCollision += i.A.CurrentTime;
        //        if (TimeOfCollision > i.T1)
        //            return false;

        //        i.A.MoveToTime(TimeOfCollision);
        //        i.B.MoveToTime(TimeOfCollision);

        //        collisionPoint = i.A.PointToGlobalCoords(collisionPoint);
        //        collisionNormal = i.A.NormalToGlobalCoords(collisionNormal);         
        //        return true;
        //    }

        //    return false;
        //}
        //private bool PlaneToSphere(out MyVector collisionPoint,out MyVector collisionNormal, 
        //    out float TimeOfCollision, Intersection i)
        //{
        //    CollisionPlane plane = (CollisionPlane)i.A.CollisionData;
        //    //MyVector cpoint, cnormal;
        //    //float ctime;
        //    float dt = i.T1 - i.A.CurrentTime;

        //    TimeOfCollision = 0;

        //    if (plane.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //        i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //        i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), (CollisionSphere)i.B.CollisionData))
        //    {
        //        TimeOfCollision += i.A.CurrentTime;
        //        if (TimeOfCollision > i.T1)
        //            return false;

        //        i.A.MoveToTime(TimeOfCollision);
        //        i.B.MoveToTime(TimeOfCollision);

        //        return true;
        //    }

        //    return false;
        //}
        //private bool SphereToSphere(out MyVector collisionPoint,out MyVector collisionNormal,
        //    out float TimeOfCollision, Intersection i)
        //{
        //    TimeOfCollision = i.T0;

        //    //AdvanceObjects to the time of collision
        //    i.A.MoveToTime(i.T0);
        //    i.B.MoveToTime(i.T0);
        //    i.A.CurrentTime = i.T0 + m_collisionTime;
        //    i.B.CurrentTime = i.T0 + m_collisionTime;
        //    //data.A = i.A;
        //    //data.B = i.B;

        //    collisionNormal = i.B.Position - i.A.Position;
        //    collisionNormal.Normalize();
        //    collisionPoint = i.A.Position + collisionNormal * i.A.BoundingSphereRadius;
        //    return true;
        //}

        //private bool CheckForCollision(out CollisionParameters data,out float TimeOfCollision, Intersection i)
        //{
        //    data = new CollisionParameters();
        //    TimeOfCollision = 0;
        //    bool status = false;
        //    float dt = i.T1 - i.A.CurrentTime;

        //    MyVector collisionPoint=new MyVector();
        //    MyVector collisionNormal=new MyVector();
        //    nf++;
        //    if(nf>90)
        //    {
        //        nf--;
        //        nf++;
        //    }



        //    //don't compute collisions backwards
        //    if (i.T0 < 0)
        //        return false;

        //    if (i.A.CollisionDataType == CollisionDataType.Point)
        //    {
        //        ICollidable a = i.A;
        //        i.A = i.B;
        //        i.B = a;

        //    }
        //    if (i.A.CollisionDataType == CollisionDataType.Point)
        //    {
        //        return false;
        //    }

        //    //swap A and B for better performance
        //    switch (i.B.CollisionDataType)
        //    {
        //        case CollisionDataType.Point:
        //            status = i.A.CollisionData.IntersectPoint(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //                i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //                i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity));
        //            break;
        //        case CollisionDataType.CollisionPlane:
        //            status = i.A.CollisionData.IntersectPlane(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //                i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //                i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), (CollisionPlane)((CollisionPlane)i.B.CollisionData.Clone()).Rotate((~i.A.Orientation) * i.B.Orientation));
        //            break;
        //        case CollisionDataType.CollisionSphere:
        //            status = i.A.CollisionData.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //                i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //                i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), (CollisionSphere)((CollisionSphere)i.B.CollisionData.Clone()));
        //            break;
        //        case CollisionDataType.CollisionMesh:

        //            status = i.A.CollisionData.IntersectMesh(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //                i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //                i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), (CollisionMesh)((CollisionMesh)i.B.CollisionData.Clone()).Rotate((~i.A.Orientation) * i.B.Orientation));
        //            break;
        //        default:
        //            return false;

        //    }            

        //    if (status == true && TimeOfCollision<=i.T1)
        //    {
        //        data.A = i.A;
        //        data.B = i.B;

        //        data.CollisionPoint = i.A.PointToGlobalCoords(collisionPoint);
        //        data.CollisionNormal = i.A.NormalToGlobalCoords(collisionNormal);
        //        if (i.A.Type == ObjectType.RigidBody && i.B.Type == ObjectType.RigidBody)
        //        {
        //            MyVector r1 = (data.CollisionPoint - i.A.Position).Rotate(~i.A.RigidBodyData.Orientation);
        //            MyVector v1 = i.A.RigidBodyData.Velocity + (i.A.RigidBodyData.AngularVelocity ^ r1).Rotate(i.A.RigidBodyData.Orientation);


        //            MyVector r2 = (data.CollisionPoint - i.B.Position).Rotate(~i.B.RigidBodyData.Orientation);
        //            MyVector v2 = i.B.RigidBodyData.Velocity + (i.B.RigidBodyData.AngularVelocity ^ r2).Rotate(i.B.RigidBodyData.Orientation);

        //            data.RelativeVelocity = v1 - v2;
        //        }
        //        else if (i.A.Type == ObjectType.RigidBody && i.B.Type == ObjectType.Static)
        //        {
        //            MyVector r1 = (data.CollisionPoint - i.A.Position).Rotate(~i.A.RigidBodyData.Orientation);
        //            MyVector v1 = i.A.RigidBodyData.Velocity + (i.A.RigidBodyData.AngularVelocity ^ r1).Rotate(i.A.RigidBodyData.Orientation);

        //            data.RelativeVelocity = v1;
        //        }
        //        else if (i.A.Type == ObjectType.Static && i.B.Type == ObjectType.RigidBody)
        //        {
        //            MyVector r2 = (data.CollisionPoint - i.B.Position).Rotate(~i.B.RigidBodyData.Orientation);
        //            MyVector v2 = i.B.RigidBodyData.Velocity + (i.B.RigidBodyData.AngularVelocity ^ r2).Rotate(i.B.RigidBodyData.Orientation);

        //            data.RelativeVelocity = -v2;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        data.CollisionTangent = -data.RelativeVelocity + (data.CollisionNormal * (data.CollisionNormal * data.RelativeVelocity));
        //        data.CollisionTangent.Normalize();
        //        //unused for collisions of moving objects
        //        data.RelativeAcceleration = new MyVector(0, 0, 0);
        //        return true;
        //    }            
        //    return false;
        //}

        //protected bool PMeshToSphere(out MyVector collisionPoint, out MyVector collisionNormal,
        //    ICollidable A,ICollidable B)
        //{
        //    CollisionMesh mesh = (CollisionMesh)A.CollisionData;
        //    collisionPoint = new MyVector();
        //    collisionNormal = new MyVector();
        //    //float ctime;


        //    TimeOfCollision = 0;

        //    if (mesh.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //        i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //        i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), i.B.BoundingSphereRadius))
        //    {
        //        TimeOfCollision += i.A.CurrentTime;
        //        if (TimeOfCollision > i.T1)
        //            return false;

        //        i.A.MoveToTime(TimeOfCollision);
        //        i.B.MoveToTime(TimeOfCollision);

        //        collisionPoint = i.A.PointToGlobalCoords(collisionPoint);
        //        collisionNormal = i.A.NormalToGlobalCoords(collisionNormal);
        //        return true;
        //    }

        //    return false;
        //}
        //protected bool PPlaneToSphere(out MyVector collisionPoint, out MyVector collisionNormal,
        //    out float TimeOfCollision, Intersection i)
        //{
        //    CollisionPlane plane = (CollisionPlane)i.A.CollisionData;
        //    //MyVector cpoint, cnormal;
        //    //float ctime;
        //    float dt = i.T1 - i.A.CurrentTime;

        //    TimeOfCollision = 0;

        //    if (plane.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
        //        i.A.PointToLocalCoords(i.B.Position), i.A.PointToLocalCoords(i.B.Position + dt * (i.B.Velocity - i.A.Velocity)),
        //        i.A.NormalToLocalCoords(i.B.Velocity - i.A.Velocity), i.B.BoundingSphereRadius))
        //    {
        //        TimeOfCollision += i.A.CurrentTime;
        //        if (TimeOfCollision > i.T1)
        //            return false;

        //        i.A.MoveToTime(TimeOfCollision);
        //        i.B.MoveToTime(TimeOfCollision);

        //        return true;
        //    }

        //    return false;
        //}
        //protected bool PSphereToSphere(out MyVector collisionPoint, out MyVector collisionNormal,
        //    out float TimeOfCollision, Intersection i)
        //{
        //    TimeOfCollision = i.T0;

        //    //AdvanceObjects to the time of collision
        //    i.A.MoveToTime(i.T0);
        //    i.B.MoveToTime(i.T0);
        //    i.A.CurrentTime = i.T0 + m_collisionTime;
        //    i.B.CurrentTime = i.T0 + m_collisionTime;
        //    //data.A = i.A;
        //    //data.B = i.B;

        //    collisionNormal = i.B.Position - i.A.Position;
        //    collisionNormal.Normalize();
        //    collisionPoint = i.A.Position + collisionNormal * i.A.BoundingSphereRadius;
        //    return true;
        //}

        //protected bool CheckForPenetration(out CollisionParameters data, ICollidable A, ICollidable B)
        //{
        //    return true;
        //}
        //public void ProcessFrame(float frameTime)
        //{
        //    currentTime = 0;
        //    endTime = frameTime;
        //    for (int s = 0; s < m_objects.Count; s++)
        //    {
        //        for (int k = s + 1; k < m_objects.Count; k++)
        //        {
        //            Intersection i = FindIntersection(m_objects[s], m_objects[k]);
        //            if (i != null && i.T0 < endTime && i.T0 >= currentTime)
        //            {
        //                m_objects[s].Collisions++;
        //                m_objects[k].Collisions++;
        //                m_intersections.Insert(i);
        //            }
        //        }
        //    }

        //    while (m_intersections.Count > 0)
        //    {
        //        Intersection i = m_intersections.PopHead();
        //        ICollidable a, b;
        //        a = i.A;
        //        b = i.B;
        //        a.Collisions--;
        //        b.Collisions--;



        //        CollisionParameters cparams;
        //        float TimeOfCollision;
        //        if (CheckForCollision(out cparams, out TimeOfCollision, i))
        //        {
        //            currentTime = TimeOfCollision;
        //            //perform collision between A and B
        //            m_simulator.ResolveCollision(cparams);

        //            //remove intersections that contain a and b
        //            for (int j = 0; j < m_intersections.Count; j++)
        //            {
        //                if (m_intersections[j].A == a || m_intersections[j].A == b ||
        //                    m_intersections[j].B == a || m_intersections[j].B == b)
        //                {
        //                    m_intersections[j].A.Collisions--;
        //                    m_intersections[j].B.Collisions--;
        //                    m_intersections.Remove(j);
        //                    j--;

        //                }
        //            }
        //            //find new possible intersections
        //            foreach (ICollidable s in m_objects)
        //            {
        //                if (s == a || s == b) continue;
        //                i = FindIntersection(a, s);
        //                if (i != null && i.T0 < endTime && i.T0 >= currentTime)
        //                {
        //                    m_intersections.Insert(i);
        //                    i.A.Collisions++;
        //                    i.B.Collisions++;
        //                }

        //                i = FindIntersection(b, s);
        //                if (i != null && i.T0 < endTime && i.T0 >= currentTime)
        //                {
        //                    m_intersections.Insert(i);
        //                    i.A.Collisions++;
        //                    i.B.Collisions++;
        //                }
        //            }
        //        }
        //        else
        //        {

        //        }
        //    }
        //    //move all objects
        //    foreach (ICollidable s in m_objects)
        //    {
        //        s.MoveToTime(endTime);
        //    }
        //}

        public float RiseObject(IPhysicalObject obj, ICollisionData collisionData, IPhysicalObject staticWorld, float amount)
        {
            IPhysicalObject A = staticWorld;
            IPhysicalObject B = obj;


           
            MyVector tmp = obj.Position;
            tmp.Y += amount;
            obj.Position = tmp;

            return amount;
            //IPhysicalObject A = staticWorld;
            //IPhysicalObject B = obj;

            //MyVector startPoint = A.PointToLocalCoords(B.Position);
            ////MyVector relativeVelocity = A.NormalToLocalCoords(B.Velocity - A.Velocity);
            //MyVector endPoint = startPoint;

            //MyVector collisionPoint, collisionNormal;
            //float TimeOfCollision, dt = 1;
            //endPoint.Y += amount;
            //MyVector relativeVelocity = A.NormalToLocalCoords(new MyVector(0, amount / dt, 0));
            //CollisionDataType collisionDataType = CollisionDataType.CollisionMesh;
            //bool status;
            //switch (collisionDataType)
            //{
            //    case CollisionDataType.CollisionPoint:
            //        status = A.CollisionData.IntersectPoint(out collisionPoint, out collisionNormal, out TimeOfCollision,
            //            startPoint, endPoint, dt);
            //        break;
            //    case CollisionDataType.CollisionPlane:
            //        status = A.CollisionData.IntersectPlane(out collisionPoint, out collisionNormal, out TimeOfCollision,
            //            startPoint, endPoint, dt, (CollisionPlane)((CollisionPlane)B.CollisionData.Clone()).Rotate((~A.Orientation) * B.Orientation));
            //        break;
            //    case CollisionDataType.CollisionSphere:
            //        status = A.CollisionData.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
            //            startPoint, endPoint, dt, (CollisionSphere)B.CollisionData);
            //        break;
            //    case CollisionDataType.CollisionMesh:

            //        CollisionMesh m = (CollisionMesh)collisionData.Clone();
            //        status = A.CollisionData.IntersectMesh(out collisionPoint, out collisionNormal, out TimeOfCollision,
            //            startPoint, endPoint, dt, (CollisionMesh)(m).Rotate((~A.Orientation) * B.Orientation));
            //        break;
            //    default:
            //        status = false;
            //        TimeOfCollision = 0;
            //        collisionNormal = new MyVector();
            //        collisionPoint = new MyVector();
            //        status = false;
            //        break;

            //}
            //if (status == true && TimeOfCollision > dt)
            //    status = false;
            //if (status == false)
            //{
            //    MyVector tmp = obj.Position;
            //    tmp.Y += amount;
            //    obj.Position = tmp;
            //    return amount;
            //}
            //else
            //{
            //    amount = amount * (TimeOfCollision / dt);
            //    MyVector tmp = obj.Position;
            //    tmp.Y += amount;
            //    obj.Position = tmp;
            //    Collision c = new Collision();

            //    c.Params.A = A;
            //    c.Params.B = B;

            //    c.Params.CollisionPoint = A.PointToGlobalCoords(collisionPoint);
            //    c.Params.CollisionNormal = A.NormalToGlobalCoords(collisionNormal);

            //    MyVector r2 = (c.Params.CollisionPoint - B.Position).Rotate(~B.Orientation);
            //    MyVector v2 = B.Velocity +
            //            (B.AngularVelocity ^ r2).Rotate(B.Orientation);


            //    c.Params.RelativeVelocity = -v2;
            //    c.Params.CollisionTangent = -c.Params.RelativeVelocity + (c.Params.CollisionNormal * (c.Params.CollisionNormal * c.Params.RelativeVelocity));
            //    c.Params.CollisionTangent.Normalize();
            //    //unused for collisions of moving objects
            //    c.Params.RelativeAcceleration = new MyVector(0, 0, 0);
            //    simulator.ResolveCollision(c.Params);

            //    //B.Advance(dt-TimeOfCollision);

            //}


            //return amount;
        }
        public float LowerObject(IPhysicalObject obj,ICollisionData collisionData, IPhysicalObject staticWorld, float amount)
        {
            IPhysicalObject A = staticWorld;
            IPhysicalObject B = obj;

            MyVector startPoint = A.PointToLocalCoords(B.Position);
            //MyVector relativeVelocity = A.NormalToLocalCoords(B.Velocity - A.Velocity);
            MyVector endPoint = startPoint;

            MyVector collisionPoint, collisionNormal;
            float TimeOfCollision, dt = 1;
            endPoint.Y -= amount;
            MyVector relativeVelocity = A.NormalToLocalCoords(new MyVector(0, -amount / dt, 0));
            CollisionDataType collisionDataType = CollisionDataType.CollisionMesh;
            bool status;
            switch (collisionDataType)
            {
                case CollisionDataType.CollisionPoint:
                    status = A.CollisionData.IntersectPoint(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt);
                    break;
                case CollisionDataType.CollisionPlane:
                    status = A.CollisionData.IntersectPlane(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt, (CollisionPlane)((CollisionPlane)B.CollisionData.Clone()).Rotate((~A.Orientation) * B.Orientation));
                    break;
                case CollisionDataType.CollisionSphere:
                    status = A.CollisionData.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt, (CollisionSphere)B.CollisionData);
                    break;
                case CollisionDataType.CollisionMesh:

                    CollisionMesh m = (CollisionMesh)collisionData.Clone();
                    status = A.CollisionData.IntersectMesh(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt, (CollisionMesh)(m).Rotate((~A.Orientation) * B.Orientation));
                    break;
                default:
                    status = false;
                    TimeOfCollision = 0;
                    collisionNormal = new MyVector();
                    collisionPoint = new MyVector();
                    status = false;
                    break;

            }
            if (status == true && TimeOfCollision > dt)
                status = false;
            if (status == false)
            {
                MyVector tmp = obj.Position;
                tmp.Y -= amount;
                obj.Position = tmp;
                return amount;
            }
            else
            {
                amount = amount * (TimeOfCollision / dt);
                MyVector tmp = obj.Position;
                tmp.Y -= amount;
                obj.Position = tmp;
                Collision c=new Collision();

                c.Params.A = A;
                c.Params.B = B;

                c.Params.CollisionPoint = A.PointToGlobalCoords(collisionPoint);
                c.Params.CollisionNormal = A.NormalToGlobalCoords(collisionNormal);

                MyVector r2 = (c.Params.CollisionPoint - B.Position).Rotate(~B.Orientation);
                MyVector v2 = B.Velocity +
                        (B.AngularVelocity ^ r2).Rotate(B.Orientation);


                c.Params.RelativeVelocity = -v2;
                c.Params.CollisionTangent = -c.Params.RelativeVelocity + (c.Params.CollisionNormal * (c.Params.CollisionNormal * c.Params.RelativeVelocity));
                c.Params.CollisionTangent.Normalize();
                //unused for collisions of moving objects
                c.Params.RelativeAcceleration = new MyVector(0, 0, 0);
                if (OnCollision != null)
                {
                    bool st = OnCollision(c.Params); //czy badac to zderzenie dalej?     
                    if (st == false)
                        return amount;
                }
                simulator.ResolveCollision(c.Params);

                //B.Advance(dt-TimeOfCollision);

            }


            return amount;
        }
        private Collision CheckPossibleCollision(IPhysicalObject A, IPhysicalObject B)
        {
            if (A.Type == ObjectType.Static && B.Type == ObjectType.Static)
            {
                return null;
            }
            if (B.CollisionDataType == CollisionDataType.CollisionMesh && A.CollisionDataType != CollisionDataType.CollisionMesh &&
                A.CollisionDataType!= CollisionDataType.CollisionOctree)
            {
                IPhysicalObject temp = B;
                B = A;
                A = temp;
            }
            if (A.CollisionDataType == CollisionDataType.CollisionPoint)
            {
                IPhysicalObject a = A;
                A = B;
                B = a;

            }
            Collision c = new Collision();

            c.Params = new CollisionParameters();

            bool status = false;
            float TimeOfCollision = 0;
            float dt;


            int timeShift;
            float shiftAmount;

            if (A.CurrentTime > B.CurrentTime)
            {
                timeShift = 1; //Move B in time back later
                shiftAmount = A.CurrentTime - B.CurrentTime;
                dt = endTime - A.CurrentTime;
                B.MoveToTime(A.CurrentTime);
            }
            else if (B.CurrentTime > A.CurrentTime)
            {
                timeShift = 2; //Move A in time back later
                shiftAmount = B.CurrentTime - A.CurrentTime;

                dt = endTime - B.CurrentTime;
                A.MoveToTime(B.CurrentTime);
            }
            else
            {
                timeShift = 0;
                shiftAmount = 0;
                dt = endTime - A.CurrentTime;
            }




            MyVector collisionPoint = new MyVector();
            MyVector collisionNormal = new MyVector();


            //if (A.CollisionDataType == CollisionDataType.CollisionPoint)
            //{
            //    IPhysicalObject a = A;
            //    A = B;
            //    B = a;

            //}
            if (A.CollisionDataType == CollisionDataType.CollisionPoint)
            {
                if (timeShift == 1)
                {
                    B.Advance(-shiftAmount);
                }
                else if (timeShift == 2)
                {
                    A.Advance(-shiftAmount);
                }

                //no point to point collision
                return null;
            }

            //swap A and B for better performance
            //TODO

            if (A.CollisionDataType == CollisionDataType.CollisionSphere && B.CollisionDataType == CollisionDataType.CollisionMesh)
            {
                IPhysicalObject temp = B;
                B = A;
                A = temp;
            }
            if ((A.Position - B.Position).LengthSq > 2 * (A.BoundingSphereRadius + B.BoundingSphereRadius) * (A.BoundingSphereRadius + B.BoundingSphereRadius))
                return null;


            MyVector startPoint = A.PointToLocalCoords(B.Position);
            MyVector relativeVelocity = A.NormalToLocalCoords(B.Velocity - A.Velocity);

            MyVector endPoint = startPoint + relativeVelocity * dt;
            if (A.CollisionDataType == CollisionDataType.CollisionMesh)
            {
                MyVector axis = -A.AngularVelocity;
                axis.Normalize();
                MyQuaternion rot = MyQuaternion.FromAxisAngle((dt * A.AngularVelocity).Length, axis);
                endPoint.Rotate(rot);
            }
            else
            {

            }
            switch (B.CollisionDataType)
            {
                case CollisionDataType.CollisionPoint:
                    status = A.CollisionData.IntersectPoint(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt);
                    break;
                case CollisionDataType.CollisionPlane:
                    status = A.CollisionData.IntersectPlane(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt, (CollisionPlane)((CollisionPlane)B.CollisionData.Clone()).Rotate((~A.Orientation) * B.Orientation));
                    break;
                case CollisionDataType.CollisionSphere:
                    status = A.CollisionData.IntersectSphere(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt, (CollisionSphere)B.CollisionData);
                    break;
                case CollisionDataType.CollisionMesh:

                    CollisionMesh m = (CollisionMesh)B.CollisionData.Clone();
                    status = A.CollisionData.IntersectMesh(out collisionPoint, out collisionNormal, out TimeOfCollision,
                        startPoint, endPoint, dt, (CollisionMesh)(m).Rotate((~A.Orientation) * B.Orientation));
                    break;
                default:
                    status = false;
                    break;

            }


            if (status == true && TimeOfCollision > dt)
                status = false;

            if (status == true)
            {

                c.A = A;
                c.B = B;
                c.Params.A = A;
                c.Params.B = B;
                c.T = TimeOfCollision + A.CurrentTime;

                c.Params.CollisionPoint = A.PointToGlobalCoords(collisionPoint);
                c.Params.CollisionNormal = A.NormalToGlobalCoords(collisionNormal);
                if (A.Type == ObjectType.Static && B.Type == ObjectType.Static)
                {
                    status = false;
                }

                else
                {

                    MyVector v1, v2;
                    MyVector r1 = (c.Params.CollisionPoint - A.Position).Rotate(~A.Orientation);
                    v1 = A.Velocity +
                        (A.AngularVelocity ^ r1).Rotate(A.Orientation);



                    MyVector r2 = (c.Params.CollisionPoint - B.Position).Rotate(~B.Orientation);
                    v2 = B.Velocity +
                        (B.AngularVelocity ^ r2).Rotate(B.Orientation);


                    c.Params.RelativeVelocity = v1 - v2;

                }

                if (status)
                {

                    c.Params.CollisionTangent = -c.Params.RelativeVelocity + (c.Params.CollisionNormal * (c.Params.CollisionNormal * c.Params.RelativeVelocity));
                    c.Params.CollisionTangent.Normalize();
                    //unused for collisions of moving objects
                    c.Params.RelativeAcceleration = new MyVector(0, 0, 0);
                }
            }

            if (timeShift == 1)
            {
                B.Advance(-shiftAmount);
            }
            else if (timeShift == 2)
            {
                A.Advance(-shiftAmount);
            }

            if (status == true)
            {
                if (OnPossibleCollision != null)
                {
                    status = OnPossibleCollision(c.Params); //czy badac to zderzenie dalej?
                    if (status == true)
                        return c;
                    else
                        return null;
                }
                else
                {
                    return c;
                }

            }
            else
                return null;
        }

        public void ProcessFrame(float frameTime)
        {
            foreach (IPhysicalObject obj in this.objects)
            {
                obj.CurrentTime = 0;
            }

            //CollisionMesh mesh = (CollisionMesh)((CollisionMesh)m_objects[1].CollisionData.Clone()).Rotate(m_objects[0].Orientation);

            currentTime = -1;
            endTime = frameTime;
            //nf++;
            //if (nf > 132)
            //{
            //    nf--;
            //    nf++;
            //}


            for (int s = 0; s < objects.Count; s++)
            {
                for (int k = s + 1; k < objects.Count; k++)
                {
                    Collision c = CheckPossibleCollision(objects[s], objects[k]);
                    if (c != null && c.T < endTime && c.T >= currentTime)
                    {
                        //m_objects[s].Collisions++;
                        //m_objects[k].Collisions++;
                        collisions.Insert(c);
                    }
                }
            }

            while (collisions.Count > 0)
            {

                Collision c = collisions.PopHead();

                IPhysicalObject a, b;
                a = c.A;
                b = c.B;

                //a.Collisions--;
                //b.Collisions--;

                if (c.T > currentTime)
                    resolvedCollisions.Clear();
                else
                {
                    CollisionPair pair = new CollisionPair(a, b);
                    foreach (CollisionPair p in resolvedCollisions)
                    {
                        if (p.Equals(pair))
                        {
                            c = null;
                            break;

                        }
                    }
                }
                if (c != null)
                {
                    currentTime = c.T;

                    //perform collision between A and B
                    a.MoveToTime(c.T);
                    b.MoveToTime(c.T);

                    if (OnCollision != null)
                    {
                        bool status = OnCollision(c.Params); //czy badac to zderzenie dalej?     
                        if (status == false)
                            continue;
                    }


                    simulator.ResolveCollision(c.Params);

                    //resolvedCollisions.Add(new CollisionPair(a, b));
                }
                IPhysicalObject temp = null;
                if (a.Type == ObjectType.Static)
                {
                    temp = a;
                    a = null;
                }
                if (b.Type == ObjectType.Static)
                {
                    temp = b;
                    b = null;
                }
                //remove collisions that contain a or b
                for (int j = 0; j < collisions.Count; j++)
                {
                    if (collisions[j].A == a || collisions[j].A == b ||
                        collisions[j].B == a || collisions[j].B == b)
                    {
                        //m_collisions[j].A.Collisions--;
                        //m_collisions[j].B.Collisions--;
                        collisions.Remove(j);
                        j--;

                    }
                }
                if (a == null)
                    a = temp;
                if (b == null)
                    b = temp;
                //find new possible intersections
                foreach (IPhysicalObject s in objects)
                {
                    if (s == a || s == b)
                    {
                        //if (s != a && a != null)
                        //{
                        //    c = CheckPossibleCollision(a, s);
                        //    if (c != null && c.T < endTime && c.T > currentTime)
                        //    {
                        //        m_collisions.Insert(c);
                        //        //c.A.Collisions++;
                        //        //c.B.Collisions++;
                        //    }
                        //}
                        //if (s != b && b != null)
                        //{
                        //    c = CheckPossibleCollision(b, s);
                        //    if (c != null && c.T < endTime && c.T > currentTime)
                        //    {
                        //        m_collisions.Insert(c);
                        //        //c.A.Collisions++;
                        //        //c.B.Collisions++;
                        //    }
                        //}
                    }
                    else
                    {
                        if (a != null)
                        {
                            c = CheckPossibleCollision(a, s);
                            if (c != null && c.T < endTime && c.T >= currentTime)
                            {
                                if (c.T == currentTime)
                                {
                                    CollisionPair pair = new CollisionPair(a, s);
                                    foreach (CollisionPair p in resolvedCollisions)
                                    {
                                        if (p.Equals(pair))
                                        {
                                            c = null;
                                            break;

                                        }
                                    }
                                }
                                if (c != null)
                                    collisions.Insert(c);
                                //c.A.Collisions++;
                                //c.B.Collisions++;
                            }
                        }

                        if (b != null)
                        {
                            c = CheckPossibleCollision(b, s);
                            if (c != null && c.T < endTime && c.T >= currentTime)
                            {
                                if (c.T == currentTime)
                                {
                                    CollisionPair pair = new CollisionPair(b, s);
                                    foreach (CollisionPair p in resolvedCollisions)
                                    {
                                        if (p.Equals(pair))
                                        {
                                            c = null;
                                            break;

                                        }
                                    }
                                }
                                if (c != null)
                                    collisions.Insert(c);
                                //c.A.Collisions++;
                                //c.B.Collisions++;
                            }
                        }
                    }
                }


            }
            resolvedCollisions.Clear();
            //move all objects
            foreach (IPhysicalObject s in objects)
            {
                s.MoveToTime(endTime);
            }
        }


    }
}
