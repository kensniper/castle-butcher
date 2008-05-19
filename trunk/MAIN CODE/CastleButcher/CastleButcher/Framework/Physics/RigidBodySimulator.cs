using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{
    public class RigidBodySimulator
    {
        private List<IPhysicalObject> objects = new List<IPhysicalObject>();
        private Dictionary<IPhysicalObject, bool> walkingEnabled = new Dictionary<IPhysicalObject, bool>();
        Dictionary<IPhysicalObject, float> walkAmount = new Dictionary<IPhysicalObject, float>();
        Dictionary<IPhysicalObject, ICollisionData> walkData = new Dictionary<IPhysicalObject, ICollisionData>();

        public Dictionary<IPhysicalObject, ICollisionData> WalkData
        {
            get { return walkData; }
            set { walkData = value; }
        }
        float simulationTick = 0.03f;

        public float SimulationTick
        {
            get { return simulationTick; }
            set { simulationTick = value; }
        }
        float timeToProcess = 0;

        public Dictionary<IPhysicalObject, bool> WalkingEnabled
        {
            get { return walkingEnabled; }
            set { walkingEnabled = value; }
        }
        private float maxStepHeight = 0;

        public float MaxStepHeight
        {
            get { return maxStepHeight; }
            set { maxStepHeight = value; }
        }

        public List<IPhysicalObject> Objects
        {
            get { return objects; }
        }
        private CollisionDetector collisionDetector;
        private MyVector gravity = new MyVector(0, -10, 0);

        public MyVector Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }
        protected float air_density = 0.01f;
        protected float friction_coefficient = 0.3f;
        bool doGravity = false;
        bool doLinearDrag = false;
        bool doAngularDrag = false;
        bool paused = false;

        //public RigidBodySimulator(CollisionDetector detector)
        //{
        //    m_collisionDetector = detector;
        //}
        public RigidBodySimulator()
        {
            collisionDetector = new CollisionDetector(this);
        }

        public ICollisionDetector CollisionDetector
        {
            get { return collisionDetector; }
            set { collisionDetector = (CollisionDetector)value; }
        }

        public void EnableWalking(IPhysicalObject obj)
        {
            for (int i = 0; i < walkingEnabled.Count; i++)
            {
                if (objects[i] == obj)
                    walkingEnabled[objects[i]] = true;
            }
        }
        public void DisableWalking(IPhysicalObject obj)
        {
            for (int i = 0; i < walkingEnabled.Count; i++)
            {
                if (objects[i] == obj)
                {
                    walkingEnabled[objects[i]] = false;
                    //walkData.Remove(objects[i]);
                }
            }
        }
        public void AddObject(IPhysicalObject obj)
        {
            objects.Add(obj);
            walkingEnabled[obj] = false;
            if (obj.CollisionDataType != CollisionDataType.None)
                CollisionDetector.AddObject(obj);
        }
        public void RemoveObject(IPhysicalObject obj)
        {
            int i = objects.IndexOf(obj);
            if (i >= 0)
            {
                
                walkingEnabled.Remove(objects[i]);
                walkData.Remove(objects[i]);
                objects.RemoveAt(i);
            }
            if (obj.CollisionDataType != CollisionDataType.None)
                CollisionDetector.RemoveObject(obj);

        }
        private void CalculatePointMassForces(PointMass o)
        {
            PointMassData data = o.PointMassData;
            //data.Forces = new MyVector(0, 0, 0);
            if (doGravity)
                data.Forces = o.Mass * gravity;

            if (doLinearDrag)
            {
                data.Forces -= data.Velocity * air_density * (data.Velocity.Length * o.LinearDragCoefficient *
                    o.BoundingSphereRadius * o.BoundingSphereRadius);

            }
            o.PointMassData = data;
        }

        private void CalculateRigidBodyForces(RigidBody o)
        {
            RigidBodyData data = o.RigidBodyData;
            //data.Forces = new MyVector(0, 0, 0);
            //data.Moments = new MyVector(0, 0, 0);
            if (doGravity)
                data.Forces = o.Mass * gravity;

            if (doLinearDrag)
            {
                data.Forces -= data.Velocity * air_density * (data.Velocity.Length * o.LinearDragCoefficient *
                    o.BoundingSphereRadius * o.BoundingSphereRadius);

            }

            if (doAngularDrag)
            {
                data.Moments -= data.AngularVelocity * air_density * (data.AngularVelocity.Length * o.AngularDragCoefficient *
                    o.BoundingSphereRadius * o.BoundingSphereRadius);
                data.Moments -= data.AngularVelocity * air_density * (o.AngularDragCoefficient *
                    o.BoundingSphereRadius * o.BoundingSphereRadius);
                //constantDrag
                if (data.AngularVelocity.Length > 0)
                {
                    data.Moments -= data.AngularVelocity * air_density * (data.AngularVelocity.Length * o.AngularDragCoefficient *
                        o.BoundingSphereRadius * o.BoundingSphereRadius) / (float)Math.Sqrt(data.AngularVelocity.Length);
                }
                if (data.AngularVelocity.LengthSq < 0.00001f)
                    data.AngularVelocity.X = data.AngularVelocity.Y = data.AngularVelocity.Z = 0;
            }

            o.RigidBodyData = data;
        }
        private void CalculateObjectForces(IPhysicalObject o)
        {
            switch (o.Type)
            {
                case ObjectType.PointMass:
                    CalculatePointMassForces((PointMass)o);
                    break;
                case ObjectType.RigidBody:
                    CalculateRigidBodyForces((RigidBody)o);
                    break;
            }
        }

        private void UpdatePointMassParameters(PointMass o, float dt)
        {
            PointMassData data = o.PointMassData;

            if (data.Mass != 0)
            {
                data.Acceleration = data.Forces / data.Mass;
            }
            else
            {
                data.Acceleration = new MyVector(0, 0, 0);
            }

            data.Velocity.Add(data.Acceleration * dt);

            data.VelocityBody = data.Velocity;
            data.VelocityBody.Rotate(~data.Orientation);
            data.Forces = new MyVector(0, 0, 0);

            o.PointMassData = data;
        }

        private void UpdateRigidBodyParameters(RigidBody o, float dt)
        {
            RigidBodyData data = o.RigidBodyData;
            if (data.Mass != 0)
            {
                data.Acceleration = data.Forces / data.Mass;
            }
            else
            {
                data.Acceleration = new MyVector(0, 0, 0);
            }
            data.Velocity.Add(data.Acceleration * dt);

            data.AngularAcceleration = data.InertiaInverse * (data.Moments -
                    (data.AngularVelocity ^ (data.Inertia * data.AngularVelocity)));

            data.AngularVelocity += data.AngularAcceleration * dt;


            //data.Orientation += data.Orientation * (data.AngularVelocity * (dt / 2));
            //data.Orientation.Normalize();


            MyMatrix rot = data.Orientation.RotationMatrix;
            MyMatrix rotinv = rot.Inverse;
            data.InertiaInverseGlobal = rot * data.InertiaInverse * rotinv;

            data.VelocityBody = data.Velocity;
            data.VelocityBody.Rotate(~data.Orientation);
            data.Forces = new MyVector(0, 0, 0);
            data.Moments = new MyVector(0, 0, 0);

            o.RigidBodyData = data;
        }

        private void UpdateObjectParameters(IPhysicalObject o, float dt)
        {
            switch (o.Type)
            {
                case ObjectType.PointMass:
                    UpdatePointMassParameters((PointMass)o, dt);
                    break;
                case ObjectType.RigidBody:
                    UpdateRigidBodyParameters((RigidBody)o, dt);
                    break;
            }
            if (o.CollisionDataType == CollisionDataType.CollisionMesh)
            {
                ((CollisionMesh)o.CollisionData).MeshRotation = o.AngularVelocity;
            }
        }
        public bool DoLinearDrag
        {
            get
            {
                return doLinearDrag;
            }
            set
            {
                doLinearDrag = value;
            }
        }
        public bool DoAngularDrag
        {
            get
            {
                return doAngularDrag;
            }
            set
            {
                doAngularDrag = value;
            }
        }
        public bool DoGravity
        {
            get
            {
                return doGravity;
            }
            set
            {
                doGravity = value;
            }
        }
        public bool Paused
        {
            get
            {
                return paused;
            }
            set
            {
                if (value != paused && value == false)
                {
                    timeToProcess = 0;
                }

                paused = value;

            }
        }

        public void AdvanceSimulation(float dt)
        {
            if (paused) return;

            timeToProcess += dt;
            if (timeToProcess >= simulationTick)
            {
                timeToProcess -= simulationTick;
                dt = simulationTick;
                //dt = 0.01f;
                foreach (IPhysicalObject obj in objects)
                {

                    if (obj.Type == ObjectType.Static)
                        continue;

                    CalculateObjectForces(obj);
                    UpdateObjectParameters(obj, dt);

                    if (obj.CollisionDataType == CollisionDataType.None)
                    {
                        //normalnie przemieszczanie nastepuje w CollisionDetector, ale jesli dany obiekt
                        //nie uczestniczy w zdezeniach to trzeba go zaktualizowac tutaj
                        obj.Advance(dt);
                    }
                }
                for (int i = 1; i < objects.Count; i++)
                {
                    if (walkingEnabled[objects[i]])
                    {
                        walkAmount[objects[i]] = collisionDetector.RiseObject(objects[i], walkData[objects[i]], objects[0], MaxStepHeight);
                    }
                }


                CollisionDetector.ProcessFrame(dt);

                for (int i = 1; i < objects.Count; i++)
                {
                    if (walkingEnabled[objects[i]])
                    {
                        collisionDetector.LowerObject(objects[i], walkData[objects[i]], objects[0], walkAmount[objects[i]]);
                    }
                }
            }
        }

        private void ResolveStaticToPointMass(CollisionParameters parameters)
        {
            IPhysicalObject A = parameters.A;
            PointMass B = (PointMass)parameters.B;
            float cr = A.CoefficientOfRestitution * B.CoefficientOfRestitution;
            float cf = A.FrictionCoefficient * B.FrictionCoefficient;



            float j, Vrt;

            PointMassData b2 = B.PointMassData;
            bool tempMass = false;
            if (b2.Mass == 0)
            {
                b2.Mass = 1;
                tempMass = true;
            }

            j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            (1 / b2.Mass);

            Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;
            if (Vrt != 0)
            {
                cf *= Vrt / parameters.RelativeVelocity.Length;
                b2.Velocity -= ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b2.Mass;

            }
            else
            {
                b2.Velocity -= (j * parameters.CollisionNormal) / b2.Mass;
            }
            if (tempMass)
            {
                b2.Mass = 0;
            }
            B.PointMassData = b2;
        }

        private void ResolveStaticToRigidBody(CollisionParameters parameters)
        {
            IPhysicalObject A = parameters.A;
            RigidBody B = (RigidBody)parameters.B;
            float cr = A.CoefficientOfRestitution * B.CoefficientOfRestitution;
            float cf = A.FrictionCoefficient * B.FrictionCoefficient;

            MyVector r2;
            float j, Vrt;

            r2 = parameters.CollisionPoint - B.Position;

            RigidBodyData b2 = B.RigidBodyData;

            cr *= (0.1f + 0.9f * Math.Abs(r2 * parameters.CollisionNormal) / (r2.Length));
            cf *= (0.1f + 0.9f * Math.Abs(r2 * parameters.CollisionNormal) / (r2.Length));

            j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            ((1 / b2.Mass) +
            (parameters.CollisionNormal * (((r2 ^ parameters.CollisionNormal) * b2.InertiaInverseGlobal) ^ r2)));

            Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;
            if (Vrt != 0)
            {
                cf *= Vrt / parameters.RelativeVelocity.Length;
                b2.Velocity -= ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b2.Mass;
                b2.AngularVelocity -= ((r2 ^ ((j * parameters.CollisionNormal) +
                    ((cf * j) * parameters.CollisionTangent))) * b2.InertiaInverseGlobal).Rotate(~b2.Orientation);
            }
            else
            {
                b2.Velocity -= (j * parameters.CollisionNormal) / b2.Mass;
                b2.AngularVelocity -= ((r2 ^ ((j * parameters.CollisionNormal))) *
                    b2.InertiaInverseGlobal).Rotate(~b2.Orientation);
            }
            B.RigidBodyData = b2;
        }

        private void ResolvePointMassToRigidBody(CollisionParameters parameters)
        {
            PointMass A = (PointMass)parameters.A;
            RigidBody B = (RigidBody)parameters.B;

            float cr = A.CoefficientOfRestitution * B.CoefficientOfRestitution;
            float cf = A.FrictionCoefficient * B.FrictionCoefficient;

            MyVector r2;
            float j, Vrt;
            //r1 = parameters.CollisionPoint - A.Position;
            r2 = parameters.CollisionPoint - B.Position;
            PointMassData b1 = A.PointMassData;
            RigidBodyData b2 = B.RigidBodyData;

            j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            ((1 / b1.Mass + 1 / b2.Mass) +
            (parameters.CollisionNormal * (((r2 ^ parameters.CollisionNormal) * b2.InertiaInverseGlobal) ^ r2)));

            Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;

            if (Vrt != 0)
            {
                //MyVector frictionImpulse;
                cf *= Vrt / parameters.RelativeVelocity.Length;
                b1.Velocity += ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b1.Mass;


                b2.Velocity -= ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b2.Mass;
                b2.AngularVelocity -= ((r2 ^ ((j * parameters.CollisionNormal) +
                    ((cf * j) * parameters.CollisionTangent))) * b2.InertiaInverseGlobal).Rotate(~b2.Orientation);

            }
            else
            {
                b1.Velocity += (j * parameters.CollisionNormal) / b1.Mass;

                b2.Velocity -= ((j * parameters.CollisionNormal)) / b2.Mass;
                b2.AngularVelocity -= ((r2 ^ (j * parameters.CollisionNormal)) *
                    b2.InertiaInverseGlobal).Rotate(~b2.Orientation);
            }
            A.PointMassData = b1;
            B.RigidBodyData = b2;
        }

        private void ResolvePointMassToPointMass(CollisionParameters parameters)
        {
            PointMass A = (PointMass)parameters.A;
            PointMass B = (PointMass)parameters.B;
            float cr = A.CoefficientOfRestitution * B.CoefficientOfRestitution;
            float cf = A.FrictionCoefficient * B.FrictionCoefficient;

            MyVector r1, r2;
            float j, Vrt;

            r1 = parameters.CollisionPoint - A.Position;
            r2 = parameters.CollisionPoint - B.Position;
            PointMassData b1 = A.PointMassData;
            PointMassData b2 = B.PointMassData;

            j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            (1 / b1.Mass + 1 / b2.Mass);

            Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;

            if (Vrt != 0)
            {
                //MyVector frictionImpulse;
                cf *= Vrt / parameters.RelativeVelocity.Length;
                b1.Velocity += ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b1.Mass;

                b2.Velocity -= ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b2.Mass;
            }
            else
            {
                b1.Velocity += (j * parameters.CollisionNormal) / b1.Mass;

                b2.Velocity -= ((j * parameters.CollisionNormal)) / b2.Mass;
            }
            A.PointMassData = b1;
            B.PointMassData = b2;
        }
        private void ResolveRigidBodyToRigidBody(CollisionParameters parameters)
        {
            RigidBody A = (RigidBody)parameters.A;
            RigidBody B = (RigidBody)parameters.B;
            float cr = A.CoefficientOfRestitution * B.CoefficientOfRestitution;
            float cf = A.FrictionCoefficient * B.FrictionCoefficient;

            MyVector r1, r2;
            float j, Vrt;

            r1 = parameters.CollisionPoint - A.Position;
            r2 = parameters.CollisionPoint - B.Position;
            RigidBodyData b1 = A.RigidBodyData;
            RigidBodyData b2 = B.RigidBodyData;

            j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            ((1 / b1.Mass + 1 / b2.Mass) +
            (parameters.CollisionNormal * (((r1 ^ parameters.CollisionNormal) * b1.InertiaInverseGlobal) ^ r1)) +
            (parameters.CollisionNormal * (((r2 ^ parameters.CollisionNormal) * b2.InertiaInverseGlobal) ^ r2)));

            Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;

            if (Vrt != 0)
            {
                //MyVector frictionImpulse;
                cf *= Vrt / parameters.RelativeVelocity.Length;
                b1.Velocity += ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b1.Mass;
                b1.AngularVelocity += ((r1 ^ ((j * parameters.CollisionNormal) +
                    ((cf * j) * parameters.CollisionTangent))) * b1.InertiaInverseGlobal).Rotate(~b1.Orientation);

                b2.Velocity -= ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b2.Mass;
                b2.AngularVelocity -= ((r2 ^ ((j * parameters.CollisionNormal) +
                    ((cf * j) * parameters.CollisionTangent))) * b2.InertiaInverseGlobal).Rotate(~b2.Orientation);

            }
            else
            {
                b1.Velocity += (j * parameters.CollisionNormal) / b1.Mass;
                b1.AngularVelocity += ((r1 ^ (j * parameters.CollisionNormal)) *
                    b1.InertiaInverseGlobal).Rotate(~b1.Orientation);

                b2.Velocity -= ((j * parameters.CollisionNormal)) / b2.Mass;
                b2.AngularVelocity -= ((r2 ^ (j * parameters.CollisionNormal)) *
                    b2.InertiaInverseGlobal).Rotate(~b2.Orientation);
            }
            A.RigidBodyData = b1;
            B.RigidBodyData = b2;
        }

        public void ResolveCollision(CollisionParameters parameters)
        {
            IPhysicalObject A = parameters.A;
            IPhysicalObject B = parameters.B;
            //if (parameters.CollisionNormal * parameters.RelativeVelocity < 0)
            //    return;

            if (A.Type == ObjectType.Static && B.Type == ObjectType.Static)
                return;
            if (A.Type == ObjectType.Static && B.Type == ObjectType.PointMass)
            {
                ResolveStaticToPointMass(parameters);
                return;
            }
            if (B.Type == ObjectType.Static && A.Type == ObjectType.PointMass)
            {

                parameters.A = B;
                parameters.B = A;

                parameters.CollisionNormal = -parameters.CollisionNormal;
                parameters.RelativeAcceleration = -parameters.RelativeAcceleration;
                parameters.RelativeVelocity = -parameters.RelativeVelocity;

                ResolveStaticToPointMass(parameters);
                return;
            }
            if (A.Type == ObjectType.Static && B.Type == ObjectType.RigidBody)
            {
                ResolveStaticToRigidBody(parameters);
                return;
            }
            if (B.Type == ObjectType.Static && A.Type == ObjectType.RigidBody)
            {
                parameters.A = B;
                parameters.B = A;

                parameters.CollisionNormal = -parameters.CollisionNormal;
                parameters.RelativeAcceleration = -parameters.RelativeAcceleration;
                parameters.RelativeVelocity = -parameters.RelativeVelocity;

                ResolveStaticToRigidBody(parameters);
                return;
            }
            if (A.Type == ObjectType.PointMass && B.Type == ObjectType.RigidBody)
            {
                ResolvePointMassToRigidBody(parameters);
                return;
            }
            if (B.Type == ObjectType.PointMass && A.Type == ObjectType.RigidBody)
            {
                parameters.A = B;
                parameters.B = A;

                parameters.CollisionNormal = -parameters.CollisionNormal;
                parameters.RelativeAcceleration = -parameters.RelativeAcceleration;
                parameters.RelativeVelocity = -parameters.RelativeVelocity;

                ResolvePointMassToRigidBody(parameters);
                return;
            }
            if (A.Type == ObjectType.PointMass && B.Type == ObjectType.PointMass)
            {
                ResolvePointMassToPointMass(parameters);
                return;
            }
            if (A.Type == ObjectType.RigidBody && B.Type == ObjectType.RigidBody)
            {
                ResolveRigidBodyToRigidBody(parameters);
                return;
            }

            //float cr = A.CoefficientOfRestitution * B.CoefficientOfRestitution;

            //float cf = A.FrictionCoefficient * B.FrictionCoefficient;

            ////in case of still-moving collision we assume that B is still
            //if (A.Type == ObjectType.Static)
            //{
            //    CollisionParameters p=parameters;
            //    p.A = parameters.B;
            //    p.B = parameters.A;
            //    p.CollisionNormal = -p.CollisionNormal;
            //    p.RelativeAcceleration = -p.RelativeAcceleration;
            //    p.RelativeVelocity = -p.RelativeVelocity;
            //    //ResolveCollision(p);
            //    //return;
            //}
            //MyVector r1,r2;
            //float j,Vrt;
            //if (B.Type == ObjectType.RigidBody)
            //{
            //    //moving-moving collision

            //    r1 = parameters.CollisionPoint - A.Position;
            //    r2 = parameters.CollisionPoint - B.Position;
            //    RigidBodyData b1 = A.RigidBodyData;
            //    RigidBodyData b2 = B.RigidBodyData;

            //    j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            //    ((1 / b1.Mass + 1 / b2.Mass) +
            //    (parameters.CollisionNormal * (((r1 ^ parameters.CollisionNormal) * b1.InertiaInverseGlobal) ^ r1)) +
            //    (parameters.CollisionNormal * (((r2 ^ parameters.CollisionNormal) * b2.InertiaInverseGlobal) ^ r2)));

            //    Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;

            //    if (Vrt != 0)
            //    {
            //        MyVector frictionImpulse;
            //        cf *= Vrt / parameters.RelativeVelocity.Length;
            //        b1.Velocity += ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b1.Mass;
            //        b1.AngularVelocity += ((r1 ^ ((j * parameters.CollisionNormal) +
            //            ((cf * j) * parameters.CollisionTangent))) * b1.InertiaInverseGlobal).Rotate(~b1.Orientation);

            //        b2.Velocity -= ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b2.Mass;
            //        b2.AngularVelocity -= ((r2 ^ ((j * parameters.CollisionNormal) +
            //            ((cf * j) * parameters.CollisionTangent))) * b2.InertiaInverseGlobal).Rotate(~b2.Orientation);

            //    }
            //    else
            //    {
            //        b1.Velocity += (j * parameters.CollisionNormal) / b1.Mass;
            //        b1.AngularVelocity += ((r1 ^ (j * parameters.CollisionNormal))*
            //            b1.InertiaInverseGlobal).Rotate(~b1.Orientation);

            //        b2.Velocity -= ((j * parameters.CollisionNormal)) / b2.Mass;
            //        b2.AngularVelocity -= ((r2 ^ (j * parameters.CollisionNormal)) * 
            //            b2.InertiaInverseGlobal).Rotate(~b2.Orientation);
            //    }
            //    A.RigidBodyData = b1;
            //    B.RigidBodyData = b2;
            //}
            //else
            //{
            //    //moving-still collision
            //    r1 = parameters.CollisionPoint - A.Position;

            //    RigidBodyData b1 = A.RigidBodyData;

            //    j = (-(1f + cr) * (parameters.RelativeVelocity * parameters.CollisionNormal)) /
            //    ((1 / b1.Mass) +
            //    (parameters.CollisionNormal * (((r1 ^ parameters.CollisionNormal) * b1.InertiaInverseGlobal) ^ r1)));

            //    Vrt = parameters.RelativeVelocity * parameters.CollisionTangent;
            //    if (Vrt != 0)
            //    {
            //        cf *= Vrt / parameters.RelativeVelocity.Length;
            //        b1.Velocity += ((j * parameters.CollisionNormal) + ((cf * j) * parameters.CollisionTangent)) / b1.Mass;
            //        b1.AngularVelocity += ((r1 ^ ((j * parameters.CollisionNormal) +
            //            ((cf * j) * parameters.CollisionTangent))) * b1.InertiaInverseGlobal).Rotate(~b1.Orientation);
            //    }
            //    else
            //    {
            //        b1.Velocity += (j * parameters.CollisionNormal) / b1.Mass;
            //        b1.AngularVelocity += ((r1 ^ ((j * parameters.CollisionNormal))) *
            //            b1.InertiaInverseGlobal).Rotate(~b1.Orientation);
            //    }
            //    A.RigidBodyData = b1;
            //}

        }
    }
}
