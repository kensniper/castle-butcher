using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Framework;
using Microsoft.DirectX.Direct3D;
using CastleButcher.GameEngine.Weapons;
using System.IO;
using CastleButcher.Content;

namespace CastleButcher.GameEngine
{

    class WeaponEntry
    {
        public string FileName;
        public WeaponClass Weapon;
    }
    class PowerupEntry
    {
        public string FileName;
        public Powerup Powerup;
    }
    class CharacterEntry
    {
        public string FileName;
        public CharacterClass CharacterClass;
    }
    /// <summary>
    /// £aduje i przechowuje obiekty wystepuj¹ce w grze takie jak bronie, asteroidy statki
    /// </summary>
    public class ObjectCache : IDeviceRelated
    {
        public static ObjectCache Instance
        {
            get
            {
                return Singleton<ObjectCache>.Instance;
            }
        }
        Dictionary<string, WeaponEntry> weapons;
        Dictionary<string, PowerupEntry> powerups;
        Dictionary<string, CharacterEntry> characters;
        //Hashtable collisionMeshes;

        Device device;

        private ObjectCache()
        {
            weapons = new Dictionary<string, WeaponEntry>();
            powerups = new Dictionary<string, PowerupEntry>();
            characters = new Dictionary<string, CharacterEntry>();

            characters["Assassin"] = LoadCharacter("Assassin");
            characters["Knight"] = LoadCharacter("Knight");
        }

        private WeaponEntry LoadWeapon(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");
            WeaponEntry entry = new WeaponEntry();
            entry.FileName = fileName;
            if (fileName == "Crossbow")
            {
                entry.Weapon = new CrossbowClass();
            }
            return entry;
        }
        private PowerupEntry LoadPowerup(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");
            PowerupEntry entry = new PowerupEntry();
            entry.FileName = fileName;
            entry.Powerup = Powerup.FromFile(AppConfig.ObjectPath + fileName);
            return entry;
        }

        private CharacterEntry LoadCharacter(string fileName)
        {
            CharacterEntry entry = new CharacterEntry();
            if (fileName == "Assassin")
            {
                entry.FileName = "Assassin";
                entry.CharacterClass = new CastleButcher.Content.AssassinClass();
            }
            else if (fileName == "Knight")
            {
                entry.FileName = "Knight";
                entry.CharacterClass = new CastleButcher.Content.KnightClass();
            }
            return entry;
        }

        public void LoadData()
        {
            DirectoryInfo di = new DirectoryInfo(AppConfig.ObjectPath);
            FileInfo[] files = di.GetFiles();
            //foreach (FileInfo fi in files)
            //{
            //    //wczytaj plik
            //    if (fi.Name.EndsWith(".sd"))
            //    {
            //        CharacterEntry entry= LoadCharacter(fi.Name);
            //        Characters[entry.FileName] = entry;
            //    }
            //}

        }

        public WeaponClass GetWeapon(string fileName)
        {
            WeaponEntry result;
            try
            {
                result = weapons[fileName];
                return result.Weapon;
            }
            catch (KeyNotFoundException)
            {
                result = LoadWeapon(fileName);
                weapons[fileName] = result;
                return result.Weapon;
            }
        }
        public Powerup GetPowerup(string fileName)
        {
            PowerupEntry result;
            try
            {
                result = powerups[fileName];
                return result.Powerup;
            }
            catch (KeyNotFoundException)
            {
                result = LoadPowerup(fileName);
                powerups[fileName] = result;
                return result.Powerup;
            }

        }

        public CharacterClass GetAssassinClass()
        {
            return characters["Assassin"].CharacterClass;
        }
        public CharacterClass GetKnightClass()
        {
            return characters["Knight"].CharacterClass;
        }

        //public IEnumerable<KeyValuePair<string,string>> ShipClasses
        //{
        //    get
        //    {
        //        foreach(KeyValuePair<string,ShipEntry> entry in ships)
        //        {
        //            yield return new KeyValuePair<string, string>(entry.Value.ShipClass.Name, entry.Key);
        //        }
        //    }
        //}



        #region IDeviceRelated Members

        public void OnCreateDevice(Device device)
        {
            this.device = device;
            //IDictionaryEnumerator en = meshes.GetEnumerator();
            //while (en.MoveNext())
            //{
            //    DictionaryEntry entry = (DictionaryEntry)en.Current;
            //}
            //for (int i = 0; i < meshes.Count; i++)
            //{
            //    meshes.Values[i] = LoadMesh((string)meshes.Keys[i]);
            //}
            //foreach (DictionaryEntry entry in meshes)
            //{
            //    meshes[entry.Key] = LoadMesh((string)entry.Key);
            //}
            //throw new Exception("The method or operation is not implemented.");
            this.LoadData();

        }

        public void OnResetDevice(Device device)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void OnLostDevice(Device device)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void OnDestroyDevice(Device device)
        {
            this.device = null;
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
