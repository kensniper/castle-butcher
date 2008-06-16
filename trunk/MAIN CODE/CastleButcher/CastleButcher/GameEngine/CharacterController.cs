using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Framework.MyMath;
using Framework;
using Microsoft.DirectX;

namespace CastleButcher.GameEngine
{

    public class CharacterController
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
        Character character;
        int id;

        SoundSystem.SoundDescriptor footsoundDescriptor;

        protected bool updateDirections = false;

        public bool UpdateDirections
        {
            get { return updateDirections; }
            set { updateDirections = value; }
        }

        public virtual bool Flying
        {
            get { return flying; }
            set { flying = value; }
        }

        public virtual float StrafeX
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


        public virtual float TurnX
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

        public virtual float TurnY
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

        public virtual float Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                if (value > 1)
                {
                    value = 1;

                }
                if (value < -1)
                    value = -1;
                velocity = value;

                if (velocity != 0 && footsoundDescriptor == null)
                {
                    footsoundDescriptor = SoundSystem.SoundEngine.StartSteps(SoundSystem.Enums.SoundTypes.stepsInside1, (Vector3)character.Position);

                }
                if (velocity == 0 && footsoundDescriptor != null)
                {
                    SoundSystem.SoundEngine.StopSteps(footsoundDescriptor);
                    footsoundDescriptor = null;
                }
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


        public CharacterController(Character character)
        {
            this.character = character;
            initialOrientation = character.Orientation;

            Reset(character);
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
                return character.MovementParameters;
            }
        }

        public virtual void Jump()
        {
            if (character.HasGroundContact)
            {

                if (footsoundDescriptor != null)
                {
                    SoundSystem.SoundEngine.StopSteps(footsoundDescriptor);
                    footsoundDescriptor = null;
                }

                MyVector v = character.Velocity;
                v.Y += GameSettings.Default.JumpSpeed;
                character.Velocity = v;
                character.HasGroundContact = false;
                if (character.CharacterClass.GameTeam == GameTeam.Assassins)
                    SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.assassinJah, (Vector3)this.character.Position);
                else if (character.CharacterClass.GameTeam == GameTeam.Knights)
                    SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.knightHou, (Vector3)this.character.Position);
            }
        }
        public virtual float SetVelocity
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
            lookVector.Normalize();
            //            character.LookOrientation = initialOrientation * (qY * qX);
            if (updateDirections)
            {
                character.LookOrientation = initialOrientation * (qY * qX);
                character.WalkOrientation = initialOrientation * qY;
            }

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
            if (updateDirections)
            {
                if (Flying)
                {


                    this.character.Velocity = SetVelocity * (lookVector) + strafeSpeedX * rightVector;
                }
                else
                {
                    float yVelocity = character.Velocity.Dot(upVector);


                    this.character.Velocity = yVelocity * upVector + strafeSpeedX * rightVector -
                        SetVelocity * (rightVector ^ upVector);
                }
            }


        }


        public virtual void Update(float dt)
        {
            //footsteps
            try
            {
                if (footsoundDescriptor != null)
                {
                    footsoundDescriptor.Position = (Vector3)character.Position;
                }
            }
            catch (NullReferenceException)
            {
                //zdarza sie...
            }
            if (character.HasGroundContact && Velocity != 0 && footsoundDescriptor == null)
            {
                footsoundDescriptor = SoundSystem.SoundEngine.StartSteps(SoundSystem.Enums.SoundTypes.stepsInside1, (Vector3)character.Position);
            }

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

        public virtual void Reset()
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

        public virtual void Reset(Character character)
        {
            this.character = character;
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

            initialOrientation = character.Orientation;
            initialLook = (new MyVector(0, 0, -1)).Rotate(initialOrientation);
            initialUp = (new MyVector(0, 1, 0)).Rotate(initialOrientation);
            initialRight = (new MyVector(1, 0, 0)).Rotate(initialOrientation);

            upVector = initialUp;
        }



    }
}
