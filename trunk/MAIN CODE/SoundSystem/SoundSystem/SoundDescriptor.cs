using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace SoundSystem
{
    public class SoundDescriptor
    {
        private SoundBuffer sound; //tutaj bez propertisa

        private bool looping;

        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set {
                position = value;
                sound.Sound3D.Position = value; //przypisanie do obiektu sound
            }
        }

        public SoundDescriptor(Device device, String fileName, Vector3 position, bool looping)
        {
            sound = new SoundBuffer(device, fileName, position);
            Looping = looping;
            Position = position;
        }

        public void Dispose()
        {
            sound.Sound.Dispose();
        }

        public void Play()
        {
            sound.Sound.Play(0, BufferPlayFlags.Default);
        }

        public bool IsPlaying()
        {
            return sound.Sound.Status.Playing;
        }
	
    }
}
