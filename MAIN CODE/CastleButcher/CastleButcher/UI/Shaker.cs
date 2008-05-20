using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MyMath;
using System.Drawing;

namespace CastleButcher.UI
{
    class Shaker
    {
        private class Lissajous
        {
            float A, B;
            float a, b;
            public Lissajous(float A_new,float B_new,float a_new,float b_new)
            {
                A=A_new;
                B=B_new;
                a = a_new;
                b = b_new;
            }
            public float ComputeX(float t)
            {
                return  A*(float)Math.Sin(a * t);
                
            }
            public float ComputeY(float t)
            {
                return B * (float)Math.Sin(b * t);
            }

            
        }

        const float Max_X=2.5f;
        const float Max_Y=2.5f;
        const float Max_Z=2.5f;
        const float Max_Radius = 0.2f;

        float t; //zmienna parametryczna dla ruchu kuszy
        float t_jump;

        Lissajous CrossBowMove=new Lissajous(0.15f,0.05f,1,2);
        int direction;
        int jump_direction;
        float angle;
        int angle_direction;

        MyVector position;
        const float stop_speed=0.00035f;
        const float run_speed = 0.0035f;
        const float acc_mullti = 0.000001f;
        const float shot_speed = 0.01f;
        const float jump_speed = 0.001f;
        float speed;
        float acceleration;

        bool walking;
        bool shoot;
        bool jump;
        bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public void Power()
        {
            if(enabled) enabled=false;
            else enabled=true;
        }

        public Shaker()
        {
            t_jump = 0.1f;//tak musi nie zmieniac
            jump_direction = -1;
            jump = false;
            enabled = true;
            direction = 1;
            angle_direction = 1;
            speed = 0;
            acceleration=acc_mullti;
            t = 0;
        }
        public float Angle
        {
            get { return angle; }
        }
        

        public MyVector Position
        {
            get { return position; }
        }

        private void Reset()
        {
            
            direction = -direction;
           
            
        }



        

        private void NextPosition(float dt) //znajduje kolejną pozycję w kierunku destination
        {
            speed += acceleration;
            if (speed <= 0)
            {
                speed = 0;
                acceleration = 0;
            }
            if (walking && speed >= run_speed)
            {
                acceleration = 0;
                speed = run_speed;
            }
            t += speed * direction;
            if (acceleration == 0 && (position.X != 0 || position.Y != 0) && !walking)
            {
                t -= acc_mullti*10000 ;
                
                position.X = CrossBowMove.ComputeX(t);
                position.Y = CrossBowMove.ComputeY(t);// 2 * p * t + k;
                if (t <= 0)
                {
                    position.X = 0;
                    position.Y = 0;
                }
            }
            else
            {
             position.X = CrossBowMove.ComputeX(t);
             position.Y = CrossBowMove.ComputeY(t);// 2 * p * t + k;
            }
            if (Math.Abs(position.X) < 0.005 && Math.Abs(position.Y) < 0.005 && !walking)
            {
                position.X = 0;
                position.Y = 0;
            }
            if (Math.Abs(position.X) >= Max_X || (Math.Abs(position.Y) >= Max_Y && !jump) || Math.Abs(position.Z) >= Max_Z) direction = -direction;

            if (jump)
            {
                t_jump += jump_direction * jump_speed;
                position.Y += t_jump;
                if (t_jump < -Max_Y)
                {
                    jump_direction = 1;
                }
                if (t_jump <= 0.1)
                {
                    t_jump = 0.1f;
                    jump = false;
                }
            }

            if (shoot)
            {
                angle += shot_speed * angle_direction;
                if (angle >= Max_Radius)
                {
                    angle_direction = (-1);
                }
                if (angle <= 0)
                {
                    angle = 0;
                    angle_direction = 1;
                    shoot = false;
                }
            }
        }

        public void Update(float dt)
        {
         
            
            NextPosition(dt);
            if(!enabled) position=position.Subtract(position);
        }
        public void StartWalking() { walking = true; acceleration = acc_mullti*10; }
        public void StopWalking() { walking = false; acceleration = acc_mullti*(-30); }
        public void Jump()
        {
            jump = true;
        }
        public void FireCrossbow()
        {
   //         speed = 2;
            shoot = true;
        }
    }
}
