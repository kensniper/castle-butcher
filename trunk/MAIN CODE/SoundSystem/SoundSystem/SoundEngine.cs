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
    public static class SoundEngine
    {
        private static Audio backgroundMusic;

        public static Audio BackgroundMusic
        {
            get { return backgroundMusic; }
            set { backgroundMusic = value; }
        }

        private static double bgMusicLastPosition;

        public static double BGMusicLastPosition
        {
            get { return bgMusicLastPosition; }
            set { bgMusicLastPosition = value; }
        }

        private static DS.Device soundCard;

        public static DS.Device SoundCard
        {
            get { return soundCard; }
            set { soundCard = value; }
        }

        private static DS.Listener3D listener;

        public static DS.Listener3D Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        private static DS.Buffer primaryBuffer;

        public static DS.Buffer PrimaryBuffer
        {
            get { return primaryBuffer; }
            set { primaryBuffer = value; }
        }

        private static DS.BufferDescription primaryBufferDescription;

        public static DS.BufferDescription PrimaryBufferDescription
        {
            get { return primaryBufferDescription; }
            set { primaryBufferDescription = value; }
        }

        private static Vector3 listenerUpVector;

        public static Vector3 ListenerUpVector
        {
            get { return listenerUpVector; }
            set { listenerUpVector = value; }
        }

        private static ArrayList soundList;

        public static ArrayList SoundList
        {
            get { return soundList; }
            set { soundList = value; }
        }

        private static SourceNameFinder nameFinder;

        public static SourceNameFinder NameFinder
        {
            get { return nameFinder; }
            set { nameFinder = value; }
        }
	

        public static void InitializeEngine(Control owner)
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
            primaryBuffer = new DS.Buffer(primaryBufferDescription, soundCard);
            listener = new Listener3D(primaryBuffer);

            //muzyka w tle

            //SourceNameFinder
            nameFinder = new SourceNameFinder();


        }

        public static void MuteAllSounds()
        {
            primaryBuffer.Volume = 0;
        }

        public static void ClearPlayedSounds()
        {
            if (soundList.Count > 0)
            {
                for (int i = soundList.Count - 1; i > -1; i--)
                {
                    SoundDescriptor currentSound = (SoundDescriptor)soundList[i];
                    if (!currentSound.IsPlaying())
                    {
                        currentSound.Dispose();
                        soundList.RemoveAt(i);
                    }
                }
            }
        }

        public static void PlayMusic(MusicTypes music)
        {
            if (backgroundMusic != null)
            {
                StopMusic();
            }
            String musicName = nameFinder.FindMusic(music);
            backgroundMusic = new Audio(musicName, true);
        }

        public static void PlaySound(SoundTypes sound, Vector3 position)
        {
            String soundName = nameFinder.FindSound(sound);
            SoundDescriptor soundDesc = new SoundDescriptor(soundCard, soundName, position, false);
            soundDesc.Play();

        }

        public static void StopMusic()
        {     
            backgroundMusic.Stop();
            //backgroundMusic.Dispose();
        }

        public static void Update(Vector3 position, Vector3 direction)
        {
        }

        public static SoundDescriptor StartSteps()
        {
            return new SoundDescriptor(soundCard, "f", listenerUpVector, true);
        }

        public static void StopSteps(SoundDescriptor steps)
        {
        }
	
    }
}
