using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Windows.Forms;

namespace SoundSystem
{
    /// <summary>
    /// Klasa dostarczajaca obiektu odpowiedzialnego za reprezentacje dzwieku 
    /// w przestrzeni (z okreslona pozycja w 3D).
    /// 
    /// Wykorzystywana jest w SoundDescriptor jako obiekt trzymajacy dzwiek.
    /// </summary>
    class SoundBuffer
    {
        private SecondaryBuffer sound;

        /// <summary>
        /// Bufor przechowujacy prosty dzwiek
        /// </summary>
        public SecondaryBuffer Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        private Buffer3D sound3D;

        /// <summary>
        /// Bufor przechowujacy przestrzenny dzwiek
        /// </summary>
        public Buffer3D Sound3D
        {
            get { return sound3D; }
            set { sound3D = value; }
        }

        private String fileName;

        /// <summary>
        /// Sciezka do pliku z dzwiekiem (wav)
        /// </summary>
        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Konstruktor parametrowy obiektu
        /// </summary>
        /// <param name="device">Uchwyt do karty dzwiekowej</param>
        /// <param name="fileName">Sciezka do pliku wav</param>
        /// <param name="position">Pozycja dzwieku w przestrzeni</param>
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
