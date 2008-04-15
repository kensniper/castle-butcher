using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoundSystem.Enums;
using System.Reflection;
using System.Resources;

namespace SoundSystem
{
    public class SourceNameFinder
    {
        private String resourcesName;

        public String ResourcesName
        {
            get { return resourcesName; }
            set { resourcesName = value; }
        }

        private ResourceManager resourcesManager;

        public ResourceManager ResourcesManager
        {
            get { return resourcesManager; }
            set { resourcesManager = value; }
        }

        private String musicDirectoryPath;

        public String MusicDirectoryPath
        {
            get { return musicDirectoryPath; }
            set { musicDirectoryPath = value; }
        }

        private String soundDirectoryPath;

        public String SoundDirectoryPath
        {
            get { return soundDirectoryPath; }
            set { soundDirectoryPath = value; }
        }
	
	
        public SourceNameFinder()
        {
            resourcesName = "SoundSystem.Resources.SourceFileNames";
            resourcesManager = new ResourceManager(resourcesName, GetType().Assembly);
        }

        public SourceNameFinder(String resourcesName)
        {
            ResourcesName = resourcesName;
            resourcesManager = new ResourceManager(resourcesName, GetType().Assembly);
        }

        public SourceNameFinder(String resourcesName, String musicPath, String soundPath)
        {
            ResourcesName = resourcesName;
            resourcesManager = new ResourceManager(resourcesName, GetType().Assembly);
            musicDirectoryPath = musicPath;
            soundDirectoryPath = soundPath;
        }
	
        public String FindMusic(MusicTypes music)
        {
            return musicDirectoryPath + (String)resourcesManager.GetObject(music.ToString());
        }

        public String FindSound(SoundTypes sound)
        {
            return soundDirectoryPath + (String)resourcesManager.GetObject(sound.ToString());
        }
    }
}
