using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using DS = Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;
using SoundSystem.Enums;
using Microsoft.DirectX.DirectSound;
using System.Collections;


namespace SoundSystem
{
    /// <summary>
    /// Glowna klasa z SoundSystem odpowiedzialna za silnik dzwiekowy.
    /// Umozliwia granie dzwiekow w przestrzeni (z zadana pozycja w 3D),
    /// muzyki w tle, ustawienia pozycji listenera dzwiekow, etc.
    /// </summary>
    public static class SoundEngine 
    {
        private static Audio backgroundMusic;

        /// <summary>
        /// Tlo muzyczne do gry - plik mp3
        /// </summary>
        public static Audio BackgroundMusic
        {
            get { return backgroundMusic; }
            set { backgroundMusic = value; }
        }

        private static double bgMusicLastPosition;

        /// <summary>
        /// Pozycja pliku muzyka tla
        /// </summary>
        public static double BGMusicLastPosition
        {
            get { return bgMusicLastPosition; }
            set { bgMusicLastPosition = value; }
        }

        private static DS.Device soundCard;

        /// <summary>
        /// Uchwyt do karty dzwiekowej
        /// </summary>
        public static DS.Device SoundCard
        {
            get { return soundCard; }
            set { soundCard = value; }
        }

        private static DS.Listener3D listener;

        /// <summary>
        /// Obiekt sluchacza
        /// </summary>
        public static DS.Listener3D Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        private static DS.Buffer primaryBuffer;

        /// <summary>
        /// Glowny bufor dzwiekowy, do ktorego beda miksowane wszystkie
        /// dzwieki grane w grze
        /// </summary>
        public static DS.Buffer PrimaryBuffer
        {
            get { return primaryBuffer; }
            set { primaryBuffer = value; }
        }

        private static DS.BufferDescription primaryBufferDescription;

        /// <summary>
        /// Obiekt opisujacy glowny bufor
        /// </summary>
        public static DS.BufferDescription PrimaryBufferDescription
        {
            get { return primaryBufferDescription; }
            set { primaryBufferDescription = value; }
        }

        private static Vector3 listenerUpVector;

        /// <summary>
        /// Wektor kierunku patrzenia listenera
        /// </summary>
        public static Vector3 ListenerUpVector
        {
            get { return listenerUpVector; }
            set { listenerUpVector = value; }
        }

        private static ArrayList soundList;

        /// <summary>
        /// Lista dzwiekow do zagrania
        /// </summary>
        public static ArrayList SoundList
        {
            get { return soundList; }
            set { soundList = value; }
        }

        private static SourceNameFinder nameFinder;

        /// <summary>
        /// Obiekt znajdujacy pelne sciezki do dzwiekow oraz muzyki
        /// </summary>
        public static SourceNameFinder NameFinder
        {
            get { return nameFinder; }
            set { nameFinder = value; }
        }
	
        /// <summary>
        /// Funkcja inicjujaca dzialanie silnika dzwiekowego (laczy sie z karta
        /// dzwiekowa, tworzy niezbedne obiekty (tablica dzwiekow, listener,
        /// glowny bufor dzwiekowy, obiekt szukajacy sciezek do plikow)
        /// </summary>
        /// <param name="owner">Obiekt (Forms), w ktorym ma byc umieszczony 
        /// silnik dzwiekowy</param>
        /// <param name="distanceFactor">Czynnik odleglosci</param>
        /// <param name="roloffFactor">Czynnik roloff</param>
        /// <param name="volume">Glosnosc</param>
        public static void InitializeEngine(Control owner, int volume,
            float distanceFactor, float roloffFactor)
        {
            //karta dzwiekowa
            soundCard = new DS.Device();
            soundCard.SetCooperativeLevel(owner, CooperativeLevel.Normal);

            //lista dzwiekow
            soundList = new ArrayList();

            //listener
            primaryBufferDescription = new BufferDescription();
            primaryBufferDescription.ControlEffects = false;
            primaryBufferDescription.Control3D = true;
            primaryBufferDescription.PrimaryBuffer = true;
            primaryBufferDescription.ControlVolume = true;
            primaryBuffer = new DS.Buffer(primaryBufferDescription, soundCard);
            primaryBuffer.Volume = volume;

            listener = new Listener3D(primaryBuffer);
            listener.DistanceFactor = distanceFactor;
            listener.RolloffFactor = roloffFactor;

            //muzyka w tle
            
            //SourceNameFinder
            nameFinder = new SourceNameFinder();

        }

        /// <summary>
        /// Funkcja sciszajaca wszystkie dzwieki 
        /// </summary>
        public static void MuteAllSounds()
        {
            primaryBuffer.Volume = 0;
        }

        /// <summary>
        /// Funkcja aktualizujaca dzwieki w tablicy dzwiekow, tzn. usuwajaca dzwieki
        /// zagrane, ktore nie maja opcji looping oraz grajaca na nowy zagrane juz
        /// dzwieki z opcja looping
        /// </summary>
        public static void UpdateSoundList()
        {
            if (soundList.Count > 0)
            {
                for (int i = soundList.Count - 1; i > -1; i--)
                {
                    SoundDescriptor currentSound = (SoundDescriptor)soundList[i];
                    //jesli dzwiek zostal juz zagrany
                    if (!currentSound.IsPlaying())
                    {
                        //jesli nie mial byc ciagly - to jest usuniety
                        if (!currentSound.Looping)
                        {
                            currentSound.Dispose();
                            soundList.RemoveAt(i);
                        }
                        else //jesli mial byc ciagly - to powtarzamy
                        {
                            currentSound.Play();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja sprawdza, czy muzyka w tle dobiegla konca i jesli tak
        /// to odgrywa ja raz jeszcze
        /// </summary>
        public static void UpdateBackgroundMusic()
        {
            if (backgroundMusic.CurrentPosition == BGMusicLastPosition)
            {
                backgroundMusic.SeekCurrentPosition(0, SeekPositionFlags.AbsolutePositioning);
            }
            BGMusicLastPosition = backgroundMusic.CurrentPosition;
        }

        /// <summary>
        /// Funkcja grajaca zadana muzyke w tle gry
        /// </summary>
        /// <param name="music">Typ wyliczeniowy z typem muzyki do zagrania</param>
        public static void PlayMusic(MusicTypes music)
        {
            if (backgroundMusic != null)
            {
                StopMusic();
            }
            String musicName = nameFinder.FindMusic(music); //znalezienie sciezki
            backgroundMusic = new Audio(musicName, true);
        }

        /// <summary>
        /// Funkcja grajaca zadany dzwiek w przestrzeni 3D
        /// </summary>
        /// <param name="sound">Typ wyliczeniowy z typem dzwieko</param>
        /// <param name="position">Wektor z pozycja w przestrzeni 3D</param>
        public static void PlaySound(SoundTypes sound, Vector3 position)
        {
            String soundName = nameFinder.FindSound(sound); //znalezienie sciezki
            SoundDescriptor soundDesc =
                new SoundDescriptor(soundCard, soundName, position, false);
            soundDesc.Play();
            soundList.Add(soundDesc);
        }

        /// <summary>
        /// Funkcja stopujaca muzyke w tle gry
        /// </summary>
        public static void StopMusic()
        {
            if (backgroundMusic != null)
            {
                backgroundMusic.Stop();
            }
            
            //backgroundMusic.Dispose();
        }

        /// <summary>
        /// Funkcja aktualizujaca pozycje oraz kierunek patrzenia listenera
        /// </summary>
        /// <param name="position">Pozycja listenera</param>
        /// <param name="frontOrientation">Przod orientacji glowy listenera</param>
        /// <param name="topOrientation">Gora orientacji glowy listenera</param>
        public static void Update(Vector3 position, Vector3 frontOrientation,
            Vector3 topOrientation)
        {
            listener.Position = position;
            Listener3DOrientation orientation = new Listener3DOrientation(frontOrientation,
                topOrientation);
            listener.Orientation = orientation;
            //listener.Orientation.Top = topOrientation;
        }

        /// <summary>
        /// Funkcja grajaca dzwiek krokow i zwracajaca obiekt z dzwiekiem
        /// do manualnej modyfikacji przez klienta
        /// </summary>
        /// <returns>Obiekt z dzwiekiem krokow</returns>
        public static SoundDescriptor StartSteps(SoundTypes sound, Vector3 position)
        {
            String soundName = nameFinder.FindSound(sound); //znalezienie sciezki
            SoundDescriptor soundDesc =
                new SoundDescriptor(soundCard, soundName, position, true);
            soundDesc.Play();
            soundList.Add(soundDesc);
            return soundDesc;
        }

        /// <summary>
        /// Funkcja stopujaca zadany dzwiek krokow
        /// </summary>
        /// <param name="steps">Dzwiek krokow do zastopowania</param>
        public static void StopSteps(SoundDescriptor steps)
        {
            //ustawienie opcji looping na false - dzwiek nie jest juz ciagly
            //zostanie usuniety w funkcji UpdateSoundList
            steps.Looping = false;
        }
	
    }
}
