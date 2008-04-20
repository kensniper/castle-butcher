using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace SoundSystem
{
    /// <summary>
    /// Klasa dostarczajaca obiekt dzwiekowy oraz umozliwiajaca zagranie dzwieku,
    /// sprawdzenie statusu (czy jest grany) oraz usuniecie.
    /// 
    /// Wykorzystywana jest w klasie glownej SoundEngine jako obiekt trzymajacy 
    /// dzwiek w przestrzeni.
    /// </summary>
    public class SoundDescriptor
    {
        private SoundBuffer sound; //tutaj bez propertisa, obiekt z dzwiekiem

        private bool looping;

        /// <summary>
        /// Status okreslajacy, czy dzwiek ma byc grany ciagle
        /// </summary>
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }

        private Vector3 position;

        /// <summary>
        /// Wektor pozycji w przestrzeni 3D
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set {
                position = value;
                sound.Sound3D.Position = value; //przypisanie do obiektu sound
            }
        }

        /// <summary>
        /// Konstruktor parametrowy obiektu.
        /// Uruchamia konstruktor obiektu SoundBuffer, ustawia pozycje i opcje
        /// looping (czy grac dzwiek ciagle).
        /// </summary>
        /// <param name="device">Uchwyt do karty dzwiekowej</param>
        /// <param name="fileName">Sciezka do pliku z dzwiekiem</param>
        /// <param name="position">Pozycja dzwieku w przestrzeni 3D</param>
        /// <param name="looping">Status okreslajacy, czy dzwiek ma byc grany
        /// ciagle</param>
        public SoundDescriptor(Device device, String fileName, Vector3 position,
            bool looping)
        {
            sound = new SoundBuffer(device, fileName, position);
            Looping = looping;
            Position = position;
        }

        /// <summary>
        /// Funkcja pozbywajaca sie obiektu dzwiekowego z pamieci
        /// </summary>
        public void Dispose()
        {
            sound.Sound.Dispose();
        }

        /// <summary>
        /// Funkcja grajaca dzwiek
        /// </summary>
        public void Play()
        {
            sound.Sound.Play(0, BufferPlayFlags.Default);
        }

        /// <summary>
        /// Funkcja sprawdzajaca, czy dzwiek jest aktualnie grany
        /// </summary>
        /// <returns>Prawde iff jesli dzwiek jest aktualnie grany</returns>
        public bool IsPlaying()
        {
            return sound.Sound.Status.Playing;
        }
	
    }
}
