using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MyMath;

namespace CastleButcher.UI
{
    class Shaker
    {
        const float Max_X=2;
        const float Max_Y=2;
        const float Max_Z=2;
        float angle;
        MyVector position;
        MyVector destination;
        float speed;
        bool walking;
        float a, b;
        bool destination_chaged;
        public Shaker()
        {
            Reset();
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
            Random rand = new Random();
            destination.X=(float)rand.NextDouble()*2;
            destination.Y = (float)rand.NextDouble() * 2;
            destination.Z = (float)rand.NextDouble() * 2;
            speed = 0.5f;
        }



        private float MojaFunkcja(float x)
        {
            return a * x * x * +b * x;
        }

        private void NextPosition(float dt) //znajduje kolejną pozycję w kierunku destination
        {
            if (destination_chaged)
            {
                if (destination.X != 0 && position.X != 0)
                {
                    destination_chaged = false;
                    a = position.Y * destination.X - destination.Y * position.X;
                    b = position.Y / position.X - a * position.X;
                }
                else
                {
                    a = 2;
                    b = 3;
                }
            }

            float x = destination.X - position.Y;
            x = x / dt;
            x*= speed;

            position.X += x;
            position.Y = MojaFunkcja(x);
            
        }

        public void Update(float dt)
        {
            if (position.X >= Max_X || position.Y >= Max_Y || position.Z >= Max_Z) Reset();
            NextPosition(dt);
        }
        public void StartWalking() { walking = true; speed = 1.5f; }
        public void StopWalking() { walking = false; speed = 0.5f; }
        public void Jump()
        {
            destination = position;
            destination.Y -= 2;
            speed = 1.4f;
            destination_chaged = true;
        }
        public void FireCrossbow()
        {
            destination = position;
            destination.Y += 2;
            speed = 2;
            destination_chaged = true;
        }
    }
}
