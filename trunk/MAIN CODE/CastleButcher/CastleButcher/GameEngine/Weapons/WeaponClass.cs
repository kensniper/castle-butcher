using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Framework.Physics;
using CastleButcher.GameEngine.Resources;
using Framework.MyMath;

namespace CastleButcher.GameEngine.Weapons
{

    public enum WeaponType { Laser, Rocket };
    /// <summary>
    /// Klasa broni jako takiej, z niej powstaj¹:InstalledWeapon oraz FloatingWeapon
    /// </summary>
    public class WeaponClass
    {
        WeaponInfo weaponParams=new WeaponInfo();
        RenderingData pickupRD;

        public RenderingData FloatingRenderingData
        {
            get { return pickupRD; }
        }
        //RenderingData installedRD;

        //public RenderingData InstalledRenderingData
        //{
        //    get { return installedRD; }
        //}

        RenderingData missileRD;
        public RenderingData MissileRenderingData
        {
            get { return missileRD; }
        }
        ICollisionData collisionData;

        public ICollisionData CollisionData
        {
            get { return collisionData; }
        }
        CollisionDataType collisionDataType;

        public CollisionDataType CollisionDataType
        {
            get { return collisionDataType; }
        }
        string name;

        public string Name
        {
            get { return name; }
        }

        public WeaponInfo WeaponParameters
        {
            get { return weaponParams; }
            //set { params = value; }
        }

        int ammoInBox;

        public int AmmoInBox
        {
            get { return ammoInBox; }
        }

        PlayerMovementParameters flyingObjectParameters;

        public PlayerMovementParameters FlyingObjectParameters
        {
            get { return flyingObjectParameters; }
        }


        WeaponType weaponType;

        public WeaponType WeaponType
        {
            get { return weaponType; }
        }

        public static WeaponClass FromFile(string fileName)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            string result;
            WeaponClass current;
            reader.WhitespaceHandling = WhitespaceHandling.None;
            //try
            //{
            //    while (reader.Read())
            //    {
            //        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Weapon")
            //        {
            //            current = new WeaponClass();
            //            result = reader.GetAttribute("name");
            //            current.name = result;
            //            if (result == null)
            //                throw new FileFormatException("Nie znaleziono nazwy broni w pliku " + fileName);

            //            result = reader.GetAttribute("type");
            //            if (result == null)
            //                throw new FileFormatException("Nie znaleziono typu broni w pliku " + fileName);
            //            if (result == "Laser")
            //                current.weaponType = WeaponType.Laser;
            //            else if (result == "Rocket")
            //                current.weaponType = WeaponType.Rocket;
            //            else
            //                throw new FileFormatException("Nie znaleziono typu broni w pliku " + fileName);


            //            while (reader.NodeType != XmlNodeType.EndElement)
            //            {
            //                reader.Read();
            //                if (reader.Name == "FloatingRenderingData")
            //                {
            //                    //TODO
            //                    result=reader.GetAttribute("file");
            //                    if (result == null)
            //                        throw new FileFormatException("Nie znaleziono nazwy mesha w pliku " + fileName);

            //                    current.floatingRD = ResourceCache.Instance.GetRenderingData(result);
            //                }
            //                //else if (reader.Name == "InstalledRenderingData")
            //                //{
            //                //    //TODO
            //                //    result = reader.GetAttribute("file");
            //                //    if (result == null)
            //                //        throw new FileFormatException("Nie znaleziono nazwy mesha w pliku " + fileName);

            //                //    current.installedRD = ResourceCache.Instance.GetRenderingData(result);
            //                //}
            //                else if (reader.Name == "MissileRenderingData")
            //                {
            //                    //TODO
            //                    result = reader.GetAttribute("file");
            //                    if (result == null)
            //                        throw new FileFormatException("Nie znaleziono nazwy mesha w pliku " + fileName);

            //                    current.missileRD = ResourceCache.Instance.GetRenderingData(result);
            //                }
            //                else if (reader.Name == "CollisionData")
            //                {
            //                    current.collisionData = XmlNodeReader.GetCollisionNode(reader, out current.collisionDataType);
            //                }
            //                else if (reader.Name == "WeaponInfo")
            //                {
            //                    current.weaponParams = XmlNodeReader.GetWeaponInfo(reader);

            //                }
            //                else if (reader.Name == "EngineParameters")
            //                {
            //                    current.flyingObjectParameters = XmlNodeReader.GetEngineParameters(reader);
            //                }
            //                else if (current.WeaponType == WeaponType.Rocket && reader.Name=="Ammo")
            //                {
            //                    string s = reader.GetAttribute("amount");
            //                    current.ammoInBox = int.Parse(s);
            //                }

            //            }
            //            reader.Close();
            //            return current;
            //        }
            //    }
            //}
            //finally
            //{
            //    if (reader != null)
            //        reader.Close();
            //}
            return null;
        }

        public WeaponPickup GetWeaponPickup(MyVector position,MyQuaternion orientation)
        {
            WeaponPickup w = new WeaponPickup(this,position,orientation);
            return w;
        }
    }
}
