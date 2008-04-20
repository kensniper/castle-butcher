using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoundSystem.Enums;
using System.Reflection;
using System.Resources;

namespace SoundSystem
{
    /// <summary>
    /// Klasa odpowiedzialna za znajdowanie sciezek do plikow dzwiekowych, zarowno
    /// wav jak i mp3.
    /// 
    /// Wykorzystywana jest w klasie glownej SoundEngine do tworzenia obiektow 
    /// dzwiekowych (z zadana sciezka do pliku) tylko wedlug zadanego typu zdarzenia
    /// dzwiekowego (muzyka/dzwiek) okreslonego w publicznych typach wyliczeniowych.
    /// 
    /// Do inicjacji potrzeba pliku resources, w ktorym znajduje sie mapowanie typow
    /// dzwiekow na nazwy plikow; okresla sie rowniez sciezke katalogowa
    /// do mp3 i wav.
    /// </summary>
    public class SourceNameFinder
    {
        private String resourcesName;

        /// <summary>
        /// Plik z resources enum -> nazwa_pliku 
        /// </summary>
        public String ResourcesName
        {
            get { return resourcesName; }
            set { resourcesName = value; }
        }

        private ResourceManager resourcesManager;

        /// <summary>
        /// Manager pliku resources
        /// </summary>
        public ResourceManager ResourcesManager
        {
            get { return resourcesManager; }
            set { resourcesManager = value; }
        }

        private String musicDirectoryPath;

        /// <summary>
        /// Sciezka do katalogu z muzyka (mp3)
        /// </summary>
        public String MusicDirectoryPath
        {
            get { return musicDirectoryPath; }
            set { musicDirectoryPath = value; }
        }

        private String soundDirectoryPath;

        /// <summary>
        /// Sciezka do katalogu z dzwiekami (wav)
        /// </summary>
        public String SoundDirectoryPath
        {
            get { return soundDirectoryPath; }
            set { soundDirectoryPath = value; }
        }

        /// <summary>
        /// Konstruktor bezparametrowy okresla plik resources na
        /// Resources.SourceFileNames oraz sciezki katalogow na katalog wykonywalny.
        /// </summary>
        public SourceNameFinder()
        {
            resourcesName = "SoundSystem.Resources.SourceFileNames";
            resourcesManager = new ResourceManager(resourcesName, GetType().Assembly);
        }

        /// <summary>
        /// Konstruktor parametrowy z zadanym stringiem do pliku resources
        /// </summary>
        /// <param name="resourcesName">Pelna nazwa pliku resources, np.
        /// ProjectName.ResourcesFile</param>
        public SourceNameFinder(String resourcesName)
        {
            ResourcesName = resourcesName;
            resourcesManager = new ResourceManager(resourcesName, GetType().Assembly);
        }

        /// <summary>
        /// Konstruktor parametrowy z zadanym stringiem do pliku resources oraz
        /// sciezkami do plikow muzyki i dzwieku
        /// </summary>
        /// <param name="resourcesName">Pelna nazwa pliku resources, np.
        /// ProjectName.ResourcesFile</param>
        /// <param name="musicPath">Sciezka do katalogu z muzyka (mp3)</param>
        /// <param name="soundPath">Sciezka do katalogu z dzwiekiem (wav)</param>
        public SourceNameFinder(String resourcesName, String musicPath,
            String soundPath)
        {
            ResourcesName = resourcesName;
            resourcesManager = new ResourceManager(resourcesName, GetType().Assembly);
            musicDirectoryPath = musicPath;
            soundDirectoryPath = soundPath;
        }
	
        /// <summary>
        /// Funkcja zwracajaca pelna sciezke do pliku z muzyka (mp3) po zadanym
        /// typie muzyki do zagrania
        /// </summary>
        /// <param name="music">Typ muzyki</param>
        /// <returns>Pelna sciezka do pliku mp3 z muzyka</returns>
        public String FindMusic(MusicTypes music)
        {
            return musicDirectoryPath 
                + (String)resourcesManager.GetObject(music.ToString());
        }

        /// <summary>
        /// Funkcja zwracajaca pelna sciezke do pliku dzwiekowego (mp3) po zadanym
        /// typie dzwieku do zagrania
        /// </summary>
        /// <param name="sound">Typ dzwieku</param>
        /// <returns>Pelna sciezka do pliku wav z dzwiekiem</returns>
        public String FindSound(SoundTypes sound)
        {
            return soundDirectoryPath 
                + (String)resourcesManager.GetObject(sound.ToString());
        }
    }
}
