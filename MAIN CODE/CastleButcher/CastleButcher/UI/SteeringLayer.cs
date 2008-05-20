using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using CastleButcher.GameEngine;

namespace CastleButcher.UI
{
    class AverageQueueFloat
    {
        Queue<float> data;


        public AverageQueueFloat(int size)
        {
            data = new Queue<float>(size);
            Size = size;
        }

        public int Size
        {
            get { return data.Count; }
            set
            {
                if (data.Count > value)
                {
                    while (data.Count > value)
                        data.Dequeue();
                }
                else
                {
                    while (data.Count < value)
                        data.Enqueue(this.Value);
                }

            }
        }

        public float Value
        {
            get
            {
                if (data.Count == 0)
                    return 0;
                float avg = 0;
                foreach (float f in data)
                {
                    avg += f;
                }
                return avg / data.Count;
            }
        }

        public void Add(float f)
        {
            data.Dequeue();
            data.Enqueue(f);
        }
    }
    class SteeringLayer : GameLayer
    {
        float maxX = 50, maxY = 50;

        AverageQueueFloat xDeltaAvg, yDeltaAvg;

        Player player;
        Shaker cameraShaker;

        public Shaker CameraShaker
        {
            get { return cameraShaker; }
        }
        public CharacterController CharacterController
        {
            get { return sdev; }
        }

        public SteeringLayer(Player player)
        {
            this.player = player;
            xDeltaAvg = new AverageQueueFloat(3);
            yDeltaAvg = new AverageQueueFloat(3);

            RecievesEmptyMouse = true;
            cameraShaker = new Shaker();
        }

        private CharacterController sdev
        {
            get
            {
                if (player.CurrentCharacter != null)
                    return player.CurrentCharacter.CharacterController;
                else 
                    return null;
            }
        }
        public override void OnMouse(System.Drawing.Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            //this.xDelta = (float)Math.Min(xDelta, maxX) * kWX;
            //this.yDelta = (float)Math.Min(yDelta, maxY) * kWY;

            if (elapsedTime > 0.00001)
            {
                xDeltaAvg.Add((float)xDelta);
                yDeltaAvg.Add((float)yDelta);
            }

            if (sdev != null)
            {
                sdev.TurnX = 1 * xDeltaAvg.Value / maxX;
                sdev.TurnY = 1 * yDeltaAvg.Value / maxY;
            }


        }
        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            if (sdev != null)
            {
                if (pressedKeys.Contains(KeyMapping.Default.Forward))
                {
                    sdev.Velocity = 1;
                    cameraShaker.StartWalking();
                }
                else if (pressedKeys.Contains(KeyMapping.Default.Backward))
                {
                    sdev.Velocity = -1;
                    cameraShaker.StopWalking();
                }


                if (pressedKeys.Contains(KeyMapping.Default.StrafeLeft))
                {
                    if (sdev.StrafeX <= 0)
                        sdev.StrafeX = -1;
                }
                if (pressedKeys.Contains(KeyMapping.Default.StrafeRight))
                {
                    if (sdev.StrafeX >= 0)
                        sdev.StrafeX = 1;
                }
                if (pressedKeys.Contains(KeyMapping.Default.Jump))
                {
                    //ground contact
                    sdev.Jump();
                }


                //podnoszenie kalwiszy
                if (releasedKeys.Contains(KeyMapping.Default.Forward))
                {
                    if (sdev.Velocity > 0)
                    {
                        sdev.Velocity = 0;
                        cameraShaker.StopWalking();
                    }
                    if (pressedKeys.Contains(KeyMapping.Default.Backward))
                    {
                        sdev.Velocity = -1;
                        cameraShaker.StartWalking();
                    }
                }
                if (releasedKeys.Contains(KeyMapping.Default.Backward))
                {
                    if (sdev.Velocity < 0)
                    {
                        sdev.Velocity = 0;
                        cameraShaker.StopWalking();
                    }
                    if (pressedKeys.Contains(KeyMapping.Default.Forward))
                    {
                        sdev.Velocity = 1;
                        cameraShaker.StartWalking();
                    }
                }

                if (releasedKeys.Contains(KeyMapping.Default.StrafeLeft))
                {
                    if (sdev.StrafeX < 0)
                        sdev.StrafeX = 0;
                    if (pressedKeys.Contains(KeyMapping.Default.StrafeRight))
                        sdev.StrafeX = 1;

                }
                if (releasedKeys.Contains(KeyMapping.Default.StrafeRight))
                {
                    if (sdev.StrafeX > 0)
                        sdev.StrafeX = 0;
                    if (pressedKeys.Contains(KeyMapping.Default.StrafeLeft))
                        sdev.StrafeX = -1;
                }
            }

        }
        public override void OnUpdateFrame(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.OnUpdateFrame(device, elapsedTime);
            if (player.IsAlive)
            {
                cameraShaker.Update(elapsedTime);
            }
        }
    }
}
