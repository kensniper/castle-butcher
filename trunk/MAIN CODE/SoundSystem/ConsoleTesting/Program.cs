using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoundSystem;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //odpalenie testu na resosrach i SourceNameFinderze
            SourceNameFinder finder = new SourceNameFinder();
            Console.WriteLine(finder.FindMusic(SoundSystem.Enums.MusicTypes.welcomeMusic));
            Console.In.Read();
        }
    }
}
