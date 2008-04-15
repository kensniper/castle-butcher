using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace SoundSystem
{
    class SoundBuffer
    {
        private SecondaryBuffer sound;

        public SecondaryBuffer Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        private Buffer3D sound3D;

        public Buffer3D Sound3D
        {
            get { return sound3D; }
            set { sound3D = value; }
        }

        private String fileName;

        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public SoundBuffer(Device device, String fileName, Vector3 position)
        {
            BufferDescription description3D = new BufferDescription();
            description3D.ControlEffects = false;
            description3D.Control3D = true;

            sound = new SecondaryBuffer(fileName, description3D, device);
            sound3D = new Buffer3D(sound);
            sound3D.Position = position;
        }
      
    }
}
