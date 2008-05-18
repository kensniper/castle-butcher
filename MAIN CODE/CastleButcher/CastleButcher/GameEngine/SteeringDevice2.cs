using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Framework.MyMath;
using Framework;

namespace CastleButcher.GameEngine
{
    //public class SteeringDevice2
    //{


    //    //float strafeX, strafeY;
    //    float strafeSpeedX, strafeSpeedY;
    //    float rollSpeed;


    //    float strafeX, strafeY;
    //    float roll;
    //    float velocity;

    //    float turnX, turnY;
    //    public float turnSpeedX, turnSpeedY;

    //    public float StrafeX
    //    {
    //        get
    //        {
    //            return strafeX;
    //        }
    //        set
    //        {
    //            if (value > 1)
    //                value = 1;
    //            if (value < -1)
    //                value = -1;
    //            strafeX = value;
    //        }
    //    }

    //    public float StrafeY
    //    {
    //        get
    //        {
    //            return strafeY;
    //        }
    //        set
    //        {
    //            if (value > 1)
    //                value = 1;
    //            if (value < -1)
    //                value = -1;
    //            strafeY = value;
    //        }
    //    }

    //    public float TurnX
    //    {
    //        get
    //        {
    //            return turnX;
    //        }
    //        set
    //        {
    //            if (value > 1)
    //                value = 1;
    //            if (value < -1)
    //                value = -1;
    //            turnX = value;
    //        }
    //    }

    //    public float TurnY
    //    {
    //        get
    //        {
    //            return turnY;
    //        }
    //        set
    //        {
    //            if (value > 1)
    //                value = 1;
    //            if (value < -1)
    //                value = -1;
    //            turnY = value;
    //        }
    //    }
    //    public float Roll
    //    {
    //        get
    //        {
    //            return roll;
    //        }
    //        set
    //        {
    //            if (value > 1)
    //                value = 1;
    //            if (value < -1)
    //                value = -1;
    //            roll = value;
    //        }
    //    }

    //    public float Velocity
    //    {
    //        get
    //        {
    //            return velocity;
    //        }
    //        set
    //        {
    //            if (value > 1)
    //                value = 1;
    //            if (value < -1)
    //                value = -1;
    //            velocity = value;
    //            //ship.AutoPilot.SetVelocity = velocity * ship.EngineParameters.MaxVelocity;
    //        }
    //    }



    //    float strafeAcc
    //    {
    //        get
    //        {
    //            return maxStrafeSpeed / 0.4f;
    //        }
    //    }


    //    float maxStrafeSpeed
    //    {
    //        get
    //        {
    //            return body.MovementParameters.MaxVelocity;
    //        }
    //    }

    //    float maxTurnSpeed
    //    {
    //        get
    //        {
    //            return 10;
    //        }
    //    }

    //    float turnAcc
    //    {
    //        get
    //        {
    //            return maxTurnSpeed * 4;
    //        }
    //    }


    //    public SteeringDevice2(IFlyingObject body)
    //    {
    //        this.body = body;
    //        Update(0, 0, 0, 0, 0);

    //    }

    //    //public MyQuaternion Rotation(float dt,float x,float y)
    //    //{
    //    //    float WX = Math.Min(maxWX, x * kWX);
    //    //    float WY = Math.Min(maxWY, x * kWY);
    //    //    float rotX = dt * WX;
    //    //    float rotY = dt * WY;

    //    //    //jak model sie nie sprawdzi trzeba bedzie dodac oparty na przyspieszeniu
    //    //    //MyQuaternion qX = new MyQuaternion(rotY, 0, 1, 0);
    //    //    //MyQuaternion qY = new MyQuaternion(rotX, 1, 0, 0);
    //    //    MyQuaternion q = MyQuaternion.FromEulerAngles(rotY, rotX,0);
    //    //    return q;
    //    //    //ship.Orientation *= q;



    //    //}
    //    MyVector lookVector;

    //    public MyVector LookVector
    //    {
    //        get { return lookVector; }
    //    }

    //    MyVector upVector;

    //    public MyVector UpVector
    //    {
    //        get { return upVector; }
    //    }
    //    MyVector rightVector;

    //    public MyVector RightVector
    //    {
    //        get { return rightVector; }
    //    }

    //    IFlyingObject body;



    //    private MyVector Drag(MyVector velocity)
    //    {
    //        return -2 * velocity;
    //    }

    //    private PlayerMovementParameters objectParameters
    //    {
    //        get
    //        {
    //            return body.MovementParameters;
    //        }
    //    }
    //    public float SetVelocity
    //    {
    //        get
    //        {
    //            return Velocity * objectParameters.MaxVelocity; ;
    //        }
    //    }
    //    /// <summary>
    //    /// Uaktualnia kierunek/predkosc na podstawie danych z myszki
    //    /// </summary>
    //    /// <param name="dt">Czas od ostatniej klatki</param>
    //    /// <param name="dx">obrot w kierunku x</param>
    //    /// <param name="dy">obrot w kierunku y</param>
    //    /// <param name="dz">obort w kierunku z</param>
    //    /// <param name="strafeX">Strafe x</param>
    //    /// <param name="strafeY">Strade y</param>
    //    private void Update(float dt, float dx, float dy, float strafeSpeedX, float strafeSpeedY)
    //    {
    //        //xDeltaAvg.Add(dx);
    //        //yDeltaAvg.Add(dy);
    //        //GM.GeneralLog.Write("xDelta:" + dx.ToString() + " yDelta:" + dy.ToString() + " dt:" + dt.ToString());
    //        float rotX = dy * dt;

    //        float rotY = dx * dt;

    //        MyQuaternion q = MyQuaternion.FromEulerAngles(rotX, rotY, 0);

    //        body.Orientation = body.Orientation * (~q);
    //        lookVector = (new MyVector(0, 0, -1)).Rotate(body.Orientation);
    //        upVector = (new MyVector(0, 1, 0)).Rotate(body.Orientation);
    //        rightVector = (new MyVector(1, 0, 0)).Rotate(body.Orientation);

    //        //MyVector zVelocity = lookVector * (ship.Velocity.Dot(lookVector));
    //        ////MyVector xVelocity = rightVector * (ship.Velocity.Dot(rightVector));
    //        ////MyVector yVelocity = upVector * (ship.Velocity.Dot(upVector));
    //        //MyVector xVelocity = rightVector * strafeSpeedX * dt;
    //        //MyVector yVelocity = upVector * strafeSpeedY * dt;
    //        //MyVector tVelocity = ship.Velocity - zVelocity-xVelocity-yVelocity;


    //        //this.ship.Forces += Drag(tVelocity);
    //        ////this.ship.Velocity -= tVelocity;

    //        //if (zVelocity.Length > SetVelocity)
    //        //{
    //        //    this.ship.Velocity -= lookVector * Math.Min(zVelocity.Length - SetVelocity, dt * objectParameters.Acceleration);
    //        //}
    //        //else if (zVelocity.Length < SetVelocity)
    //        //{
    //        //    this.ship.Velocity += lookVector * Math.Min(SetVelocity - zVelocity.Length, dt * objectParameters.Acceleration); ;
    //        //}
    //        //if (xVelocity.Length > strafeSpeedX)
    //        //{

    //        //}
    //        float zVelocity = body.Velocity.Dot(lookVector);
    //        //MyVector xVelocity = rightVector * (ship.Velocity.Dot(rightVector));
    //        //MyVector yVelocity = upVector * (ship.Velocity.Dot(upVector));
    //        float xVelocity = body.Velocity.Dot(rightVector);
    //        float yVelocity = body.Velocity.Dot(upVector);
    //        MyVector nVelocity = body.Velocity - lookVector * zVelocity - rightVector * strafeSpeedX - upVector * strafeSpeedY;



    //        //this.ship.Velocity -= tVelocity;

    //        if (zVelocity > SetVelocity)
    //        {
    //            this.body.Velocity -= lookVector * Math.Min(zVelocity - SetVelocity, dt * objectParameters.Acceleration);
    //        }
    //        else if (zVelocity < SetVelocity)
    //        {
    //            this.body.Velocity += lookVector * Math.Min(SetVelocity - zVelocity, dt * objectParameters.Acceleration); ;
    //        }

    //        this.body.Velocity += Drag(nVelocity) * dt;
    //        //if (xVelocity > strafeSpeedX)
    //        //{
    //        //    this.ship.Velocity -= rightVector * Math.Min(xVelocity - SetVelocity, dt * objectParameters.Acceleration);
    //        //}
    //        //else if(xVelocity<strafeSpeed
    //        //this.ship.Velocity+=rightVector

    //        //this.ship.Position += strafeX * RightVector;
    //        //this.ship.Position += strafeY * UpVector;
    //    }


    //    public void Update(float dt)
    //    {



    //        //strafing
    //        if (strafeX*maxStrafeSpeed != strafeSpeedX)
    //        {
    //            strafeSpeedX -= Math.Sign(strafeSpeedX - strafeX * maxStrafeSpeed) *
    //                Math.Min(dt * strafeAcc, Math.Abs(strafeSpeedX - strafeX * maxStrafeSpeed));


    //        }
    //        if (strafeY * maxStrafeSpeed != strafeSpeedY)
    //        {
    //            strafeSpeedY -= Math.Sign(strafeSpeedY - strafeY * maxStrafeSpeed) *
    //                Math.Min(dt * strafeAcc, Math.Abs(strafeSpeedY - strafeY * maxStrafeSpeed));


    //        }


    //        if (turnX * maxTurnSpeed != turnSpeedX)
    //        {
    //            turnSpeedX -= Math.Sign(turnSpeedX - turnX * maxTurnSpeed) *
    //                Math.Min(dt * turnAcc, Math.Abs(turnSpeedX - turnX * maxTurnSpeed));

    //        }
    //        if (turnY * maxTurnSpeed != turnSpeedY)
    //        {
    //            turnSpeedY -= Math.Sign(turnSpeedY - turnY * maxTurnSpeed) *
    //                Math.Min(dt * turnAcc, Math.Abs(turnSpeedY - turnY * maxTurnSpeed));

    //        }


    //        //if (strafeX != 0)
    //        //{
    //        //    strafeSpeedX += Math.Sign(strafeX) * dt * strafeAcc;

    //        //    if (strafeX > 0 && strafeSpeedX > maxStrafeSpeed)
    //        //        strafeSpeedX = maxStrafeSpeed;
    //        //    else if (strafeX < 0 && strafeSpeedX < -maxStrafeSpeed)
    //        //        strafeSpeedX = -maxStrafeSpeed;
    //        //}
    //        //else
    //        //{
    //        //    strafeSpeedX -= Math.Sign(strafeSpeedX) * Math.Min(dt * strafeAcc, strafeSpeedX);
    //        //}
    //        //if (strafeY != 0)
    //        //{
    //        //    strafeSpeedY += Math.Sign(strafeY) * dt * strafeAcc;

    //        //    if (strafeY > 0 && strafeSpeedY > maxStrafeSpeed)
    //        //        strafeSpeedY = maxStrafeSpeed;
    //        //    else if (strafeY < 0 && strafeSpeedY < -maxStrafeSpeed)
    //        //        strafeSpeedY = -maxStrafeSpeed;
    //        //}
    //        //else
    //        //{
    //        //    strafeSpeedY -= Math.Sign(strafeSpeedY) * Math.Min(dt * strafeAcc, strafeSpeedY);
    //        //}

    //        //if (turnX != 0)
    //        //{
    //        //    turnSpeedX += Math.Sign(turnX) * dt * turnAcc;

    //        //    if (turnX > 0 && turnSpeedX > turnX * objectParameters.Steering)
    //        //        turnSpeedX = objectParameters.Steering;
    //        //    else if (turnX < 0 && turnSpeedX < -objectParameters.Steering)
    //        //        turnSpeedX = -objectParameters.Steering;
    //        //}
    //        //else
    //        //{
    //        //    turnSpeedX -= Math.Sign(turnSpeedX) * Math.Min(dt * turnAcc, turnSpeedX);
    //        //}

    //        //if (turnY != 0)
    //        //{
    //        //    turnSpeedY += Math.Sign(turnY) * dt * turnAcc;

    //        //    if (turnY > 0 && turnSpeedY > objectParameters.Steering)
    //        //        turnSpeedY = objectParameters.Steering;
    //        //    else if (turnY < 0 && turnSpeedY < -objectParameters.Steering)
    //        //        turnSpeedY = -objectParameters.Steering;
    //        //}
    //        //else
    //        //{
    //        //    turnSpeedY -= Math.Sign(turnSpeedY) * Math.Min(dt * turnAcc, turnSpeedY);
    //        //}

    //        //if (roll != 0)
    //        //{
    //        //    rollSpeed += Math.Sign(roll) * dt * turnAcc;
    //        //    if (roll > 0 && rollSpeed > objectParameters.Steering)
    //        //        rollSpeed = objectParameters.Steering;
    //        //    else if (roll < 0 && rollSpeed < -objectParameters.Steering)
    //        //        rollSpeed = -objectParameters.Steering;
    //        //}
    //        //else
    //        //{
    //        //    rollSpeed -= Math.Sign(rollSpeed) * Math.Min(dt * turnAcc, rollSpeed);
    //        //}

    //        Update(dt, turnSpeedX, turnSpeedY, strafeSpeedX, strafeSpeedY);
    //    }

    //    public void Reset()
    //    {
    //        //this.xDeltaAvg
    //        this.strafeSpeedX = 0;
    //        this.strafeSpeedY = 0;
    //        strafeX = 0;
    //        strafeY = 0;
    //        turnSpeedX = 0;
    //        turnSpeedY = 0;

    //        turnX = 0;
    //        turnY = 0;

    //        rollSpeed = 0;
    //        velocity = 0;
    //    }

    //    public void Reset(IFlyingObject body)
    //    {
    //        this.body = body;
    //        //this.xDeltaAvg
    //        this.strafeSpeedX = 0;
    //        this.strafeSpeedY = 0;
    //        strafeX = 0;
    //        strafeY = 0;
    //        turnSpeedX = 0;
    //        turnSpeedY = 0;

    //        turnX = 0;
    //        turnY = 0;

    //        rollSpeed = 0;
    //        velocity = 0;
    //    }



    //}
    public class SteeringDevice
    {


        //float strafeX, strafeY;
        float strafeSpeedX;
        float rollSpeed;


        float strafeX;
        float roll;
        float velocity;

        float turnX, turnY;
        public float turnSpeedX, turnSpeedY;

        bool flying = true;

        float angleX, angleY;
        MyQuaternion initialOrientation;
        MyVector initialUp, initialRight, initialLook;
        Player player;

        public bool Flying
        {
            get { return flying; }
            set { flying = value; }
        }

        public float StrafeX
        {
            get
            {
                return strafeX;
            }
            set
            {
                if (value > 1)
                    value = 1;
                if (value < -1)
                    value = -1;
                strafeX = value;
            }
        }


        public float TurnX
        {
            get
            {
                return turnX;
            }
            set
            {
                if (value > 1)
                    value = 1;
                if (value < -1)
                    value = -1;
                turnX = value;
            }
        }

        public float TurnY
        {
            get
            {
                return turnY;
            }
            set
            {
                if (value > 1)
                    value = 1;
                if (value < -1)
                    value = -1;
                turnY = value;
            }
        }

        public float Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                if (value > 1)
                    value = 1;
                if (value < -1)
                    value = -1;
                velocity = value;
                //ship.AutoPilot.SetVelocity = velocity * ship.EngineParameters.MaxVelocity;
            }
        }



        float strafeAcc
        {
            get
            {
                return maxStrafeSpeed / 0.4f;
            }
        }


        float maxStrafeSpeed
        {
            get
            {
                return this.objectParameters.MaxVelocity / 2;
            }
        }

        float maxTurnSpeed
        {
            get
            {
                return GameSettings.Default.MouseSensitivity;
            }
        }

        float turnAcc
        {
            get
            {
                return maxTurnSpeed * 4;
            }
        }


        public SteeringDevice(Player player)
        {
            this.player = player;
            initialOrientation = player.CurrentCharacter.Orientation;

            Reset(player);
            Update(0, 0, 0, 0, 0);

        }

        //public MyQuaternion Rotation(float dt,float x,float y)
        //{
        //    float WX = Math.Min(maxWX, x * kWX);
        //    float WY = Math.Min(maxWY, x * kWY);
        //    float rotX = dt * WX;
        //    float rotY = dt * WY;

        //    //jak model sie nie sprawdzi trzeba bedzie dodac oparty na przyspieszeniu
        //    //MyQuaternion qX = new MyQuaternion(rotY, 0, 1, 0);
        //    //MyQuaternion qY = new MyQuaternion(rotX, 1, 0, 0);
        //    MyQuaternion q = MyQuaternion.FromEulerAngles(rotY, rotX,0);
        //    return q;
        //    //ship.Orientation *= q;



        //}
        MyVector lookVector;

        public MyVector LookVector
        {
            get { return lookVector; }
        }

        MyVector upVector;

        public MyVector UpVector
        {
            get { return upVector; }
        }
        MyVector rightVector;

        public MyVector RightVector
        {
            get { return rightVector; }
        }




        private MyVector Drag(MyVector velocity)
        {
            return -2 * velocity;
        }

        private PlayerMovementParameters objectParameters
        {
            get
            {
                if (player.IsAlive)
                    return player.CurrentCharacter.MovementParameters;
                else
                    return player.CurrentCharacter.MovementParameters;
            }
        }

        public void Jump()
        {
            if (player.CurrentCharacter.HasGroundContact)
            {
                MyVector v = player.CurrentCharacter.Velocity;
                v.Y += GameSettings.Default.JumpSpeed;
                player.CurrentCharacter.Velocity = v;
                player.CurrentCharacter.HasGroundContact = false;
            }
        }
        public float SetVelocity
        {
            get
            {
                return Velocity * objectParameters.MaxVelocity; ;
            }
        }
        /// <summary>
        /// Uaktualnia kierunek/predkosc na podstawie danych z myszki
        /// </summary>
        /// <param name="dt">Czas od ostatniej klatki</param>
        /// <param name="dx">obrot w kierunku x</param>
        /// <param name="dy">obrot w kierunku y</param>
        /// <param name="dz">obort w kierunku z</param>
        /// <param name="strafeX">Strafe x</param>
        /// <param name="strafeY">Strade y</param>
        private void Update(float dt, float dx, float dy, float strafeSpeedX, float strafeSpeedY)
        {
            //xDeltaAvg.Add(dx);
            //yDeltaAvg.Add(dy);
            //GM.GeneralLog.Write("xDelta:" + dx.ToString() + " yDelta:" + dy.ToString() + " dt:" + dt.ToString());
            float rotX = dy * dt;

            float rotY = dx * dt;

            angleX += rotX;
            angleY += rotY;

            if (angleX > 0.9 * Math.PI / 2)
            {
                angleX = (float)(0.9 * Math.PI / 2);
            }
            if (angleX < -0.9 * Math.PI / 2)
            {
                angleX = (float)(-0.9 * Math.PI / 2);
            }

            MyQuaternion qY = MyQuaternion.FromAxisAngle(-angleY, initialUp);
            MyQuaternion qX = MyQuaternion.FromAxisAngle(-angleX, initialRight);

            //player.CurrentCharacter.Orientation = initialOrientation * (qY);
            //orientation zostanie zmienione przez fizyke, jeœli ustawie odpowiednie AngularVelocity
            if (dt > 0)
            {
                //float currentRotY = player.CurrentCharacter.Orientation.EulerAngles().Z/2;
                ////currentRotY *= (float)Math.PI / 180f;
                //player.CurrentCharacter.AngularVelocity = upVector * ((currentRotY-angleY) / 0.03f);
                //player.CurrentCharacter.AngularVelocity = upVector * ((-rotY) / 0.03f);

            }
            lookVector = initialLook;
            rightVector = initialRight;
            lookVector.Rotate(qX);
            lookVector.Rotate(qY);
            rightVector.Rotate(qY);

            //MyVector zVelocity = lookVector * (ship.Velocity.Dot(lookVector));
            ////MyVector xVelocity = rightVector * (ship.Velocity.Dot(rightVector));
            ////MyVector yVelocity = upVector * (ship.Velocity.Dot(upVector));
            //MyVector xVelocity = rightVector * strafeSpeedX * dt;
            //MyVector yVelocity = upVector * strafeSpeedY * dt;
            //MyVector tVelocity = ship.Velocity - zVelocity-xVelocity-yVelocity;


            //this.ship.Forces += Drag(tVelocity);
            ////this.ship.Velocity -= tVelocity;

            //if (zVelocity.Length > SetVelocity)
            //{
            //    this.ship.Velocity -= lookVector * Math.Min(zVelocity.Length - SetVelocity, dt * objectParameters.Acceleration);
            //}
            //else if (zVelocity.Length < SetVelocity)
            //{
            //    this.ship.Velocity += lookVector * Math.Min(SetVelocity - zVelocity.Length, dt * objectParameters.Acceleration); ;
            //}
            //if (xVelocity.Length > strafeSpeedX)
            //{

            //}
            if (Flying)
            {
                //float zVelocity = player.CurrentCharacter.Velocity.Dot(lookVector);
                ////MyVector xVelocity = rightVector * (ship.Velocity.Dot(rightVector));
                ////MyVector yVelocity = upVector * (ship.Velocity.Dot(upVector));
                //float xVelocity = player.CurrentCharacter.Velocity.Dot(rightVector);
                float yVelocity = player.CurrentCharacter.Velocity.Dot(upVector);
                //MyVector nVelocity = player.CurrentCharacter.Velocity - lookVector * zVelocity - rightVector * strafeSpeedX - upVector * strafeSpeedY;
                //if (zVelocity > SetVelocity)
                //{
                //    this.player.CurrentCharacter.Velocity -= lookVector * Math.Min(zVelocity - SetVelocity, dt * objectParameters.Acceleration);
                //}
                //else if (zVelocity < SetVelocity)
                //{
                //    this.player.CurrentCharacter.Velocity += lookVector * Math.Min(SetVelocity - zVelocity, dt * objectParameters.Acceleration); ;
                //}

                //this.player.CurrentCharacter.Velocity += Drag(nVelocity) * dt;

                this.player.CurrentCharacter.Velocity = yVelocity * upVector + strafeSpeedX * rightVector +
                    SetVelocity * (lookVector);
            }
            else
            {
                float yVelocity = player.CurrentCharacter.Velocity.Dot(upVector);
                //MyVector nVelocity = player.CurrentCharacter.Velocity - lookVector * zVelocity - rightVector * strafeSpeedX - upVector * strafeSpeedY;
                //if (zVelocity > SetVelocity)
                //{
                //    this.player.CurrentCharacter.Velocity -= lookVector * Math.Min(zVelocity - SetVelocity, dt * objectParameters.Acceleration);
                //}
                //else if (zVelocity < SetVelocity)
                //{
                //    this.player.CurrentCharacter.Velocity += lookVector * Math.Min(SetVelocity - zVelocity, dt * objectParameters.Acceleration); ;
                //}

                //this.player.CurrentCharacter.Velocity += Drag(nVelocity) * dt;

                this.player.CurrentCharacter.Velocity = yVelocity * upVector + strafeSpeedX * rightVector -
                    SetVelocity * (rightVector ^ upVector);
            }



            //this.ship.Velocity -= tVelocity;


            //if (xVelocity > strafeSpeedX)
            //{
            //    this.ship.Velocity -= rightVector * Math.Min(xVelocity - SetVelocity, dt * objectParameters.Acceleration);
            //}
            //else if(xVelocity<strafeSpeed
            //this.ship.Velocity+=rightVector

            //this.ship.Position += strafeX * RightVector;
            //this.ship.Position += strafeY * UpVector;
        }


        public void Update(float dt)
        {



            //strafing
            if (strafeX * maxStrafeSpeed != strafeSpeedX)
            {
                //strafeSpeedX -= Math.Sign(strafeSpeedX - strafeX * maxStrafeSpeed) *
                //    Math.Min(dt * strafeAcc, Math.Abs(strafeSpeedX - strafeX * maxStrafeSpeed));
                strafeSpeedX = maxStrafeSpeed * strafeX;


            }


            if (turnX * maxTurnSpeed != turnSpeedX)
            {
                //turnSpeedX -= Math.Sign(turnSpeedX - turnX * maxTurnSpeed) *
                //    Math.Min(dt * turnAcc, Math.Abs(turnSpeedX - turnX * maxTurnSpeed));
                turnSpeedX = turnX * maxTurnSpeed;

            }
            if (turnY * maxTurnSpeed != turnSpeedY)
            {
                //turnSpeedY -= Math.Sign(turnSpeedY - turnY * maxTurnSpeed) *
                //    Math.Min(dt * turnAcc, Math.Abs(turnSpeedY - turnY * maxTurnSpeed));
                turnSpeedY = turnY * maxTurnSpeed;

            }

            Update(dt, turnSpeedX, turnSpeedY, strafeSpeedX, 0);
        }

        public void Reset()
        {
            //this.xDeltaAvg
            this.strafeSpeedX = 0;
            strafeX = 0;
            turnSpeedX = 0;
            turnSpeedY = 0;

            turnX = 0;
            turnY = 0;

            rollSpeed = 0;
            velocity = 0;
            angleX = 0;
            angleY = 0;

            initialOrientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            initialLook = (new MyVector(0, 0, -1)).Rotate(initialOrientation);
            initialUp = (new MyVector(0, 1, 0)).Rotate(initialOrientation);
            initialRight = (new MyVector(1, 0, 0)).Rotate(initialOrientation);
        }

        public void Reset(Player player)
        {
            this.player = player;
            //this.xDeltaAvg
            this.strafeSpeedX = 0;
            strafeX = 0;
            turnSpeedX = 0;
            turnSpeedY = 0;

            turnX = 0;
            turnY = 0;

            rollSpeed = 0;
            velocity = 0;

            angleX = 0;
            angleY = 0;

            initialOrientation = player.CurrentCharacter.Orientation;
            initialLook = (new MyVector(0, 0, -1)).Rotate(initialOrientation);
            initialUp = (new MyVector(0, 1, 0)).Rotate(initialOrientation);
            initialRight = (new MyVector(1, 0, 0)).Rotate(initialOrientation);

            upVector = initialUp;
        }



    }
}
