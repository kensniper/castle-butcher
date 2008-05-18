using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Framework.Physics;
using Framework.MyMath;
using System.Globalization;
using CastleButcher.GameEngine.Weapons;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Pomocnicza klasa do wczytywania powtarzaj¹cych siê fragmentów w plikach xml
    /// </summary>
    public static class XmlNodeReader
    {
        static NumberFormatInfo _nfi = new NumberFormatInfo();
        static XmlNodeReader()
        {
            _nfi.NumberDecimalSeparator = ".";
        }

        public static void GetTransformNode(XmlTextReader reader, out MyVector position, out MyVector rotation)
        {
            string result = reader.GetAttribute("position");
            if (!MyVector.FromString(result, out position))
                throw new FileFormatException("Nieprawid³owy opis position w pliku ");

            result = reader.GetAttribute("rotation");
            if (!MyVector.FromString(result, out rotation))
                throw new FileFormatException("Nieprawid³owy opis position w pliku ");
        }
        public static ICollisionData GetCollisionNode(XmlTextReader reader, out CollisionDataType type)
        {
            type = CollisionDataType.None;
            ICollisionData data = null;
            string result = reader.GetAttribute("type");
            if (result == null)
                throw new FileFormatException("Nieprawid³owy opis collision data w pliku ");
            switch (result)
            {
                case "BS":
                    type = CollisionDataType.CollisionSphere;
                    result = reader.GetAttribute("radius");
                    if (result == null)
                        throw new FileFormatException("Nieprawid³owy opis collision data w pliku ");
                    data = new CollisionSphere(float.Parse(result, _nfi));
                    break;
                case "Mesh":
                    type = CollisionDataType.CollisionMesh;
                    result = reader.GetAttribute("file");
                    if (result == null)
                        throw new FileFormatException("Nieprawid³owy opis collision data w pliku ");
                    data = CollisionMesh.FromFile(AppConfig.MeshPath + result, false);
                    break;
                case "Tree":
                    type = CollisionDataType.CollisionOctree;
                    result = reader.GetAttribute("file");
                    if (result == null)
                        throw new FileFormatException("Nieprawid³owy opis collision data w pliku ");
                    CollisionMesh m = CollisionMesh.FromFile(AppConfig.MeshPath + result, false);
                    data = CollisionOctree.FromMesh(m, 0, 0, 0, 30, 2, 200);
                    break;

            }
            return data;
        }
        public static IGameObject GetObjectNode(XmlTextReader reader, out ObjectType type)
        {
            string result = reader.GetAttribute("type");

            switch (result)
            {
                case "mesh":
                    type = ObjectType.Mesh;
                    break;
                case "respawn":
                    type = ObjectType.Respawn;
                    break;
                case "weapon":
                    type = ObjectType.Weapon;
                    break;
                case "powerup":
                    type = ObjectType.Powerup;
                    break;
                default:
                    type = ObjectType.Mesh;
                    return null;
            }
            if (type == ObjectType.Mesh)
            {
                result = reader.GetAttribute("position");
                MyVector pos;
                if (!MyVector.FromString(result, out pos))
                    return null;

                result = reader.GetAttribute("rotation");
                MyVector angles;
                if (!MyVector.FromString(result, out angles))
                    angles = new MyVector(0, 0, 0);
                ICollisionData cdata = null;
                RenderingData rdata = null;
                CollisionDataType cdataType = CollisionDataType.None;
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Object"))
                {
                    if (reader.Name == "RenderingData")
                    {
                        result = reader.GetAttribute("file");
                        rdata = ResourceCache.Instance.GetRenderingData(result);//RenderingData.FromFile(AppConfig.MeshPath + result);

                    }
                    else if (reader.Name == "CollisionData")
                    {
                        cdata = XmlNodeReader.GetCollisionNode(reader, out cdataType);
                    }
                    reader.Read();
                }
                return new StaticMesh(cdata, cdataType, rdata, pos, angles);
            }
            else if (type == ObjectType.Powerup)
            {
                result = reader.GetAttribute("position");
                MyVector pos;
                if (!MyVector.FromString(result, out pos))
                    return null;
                RenderingData rdata = null;

                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Object"))
                {
                    if (reader.Name == "RenderingData")
                    {
                        result = reader.GetAttribute("file");
                        rdata = ResourceCache.Instance.GetRenderingData(result);//RenderingData.FromFile(AppConfig.ObjectPath + result);

                    }
                    reader.Read();
                }
                return null;

            }
            else if (type == ObjectType.Respawn)
            {
                result = reader.GetAttribute("position");
                MyVector pos;
                if (!MyVector.FromString(result, out pos))
                    return null;

                result = reader.GetAttribute("rotation");
                MyVector angles;
                if (!MyVector.FromString(result, out angles))
                    angles = new MyVector(0, 0, 0);

                return new RespawnPoint(pos, angles);
            }
            else if (type == ObjectType.Weapon)
            {
                result = reader.GetAttribute("position");
                MyVector pos;
                if (!MyVector.FromString(result, out pos))
                    return null;
                MyVector rot;
                result = reader.GetAttribute("rotation");
                if (!MyVector.FromString(result, out rot))
                    return null;
                result = reader.GetAttribute("name");
                WeaponClass weapon = ObjectCache.Instance.GetWeapon(result);
                WeaponPickup wp = weapon.GetWeaponPickup(pos, MyQuaternion.FromEulerAngles(rot.X, rot.Y, rot.Z));
                return wp;
            }
            return null;
        }
        //public static PlayerMovementParameters GetEngineParameters(XmlTextReader reader)
        //{
        //    PlayerMovementParameters engineParams = new PlayerMovementParameters();
        //    string result = reader.GetAttribute("velocity");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    engineParams.MaxVelocity = float.Parse(result,_nfi);

        //    result = reader.GetAttribute("acceleration");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    engineParams.Acceleration = float.Parse(result,_nfi);

        //    result = reader.GetAttribute("steering");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    engineParams.Steering = float.Parse(result,_nfi);
        //    return engineParams;
        //}

        //public static ShipParameters GetShipParameters(XmlTextReader reader)
        //{
        //    ShipParameters shipParameters = new ShipParameters();
        //    string result = reader.GetAttribute("hull");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    shipParameters.HullAmount = int.Parse(result);

        //    result = reader.GetAttribute("shield");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    shipParameters.ShieldAmount = int.Parse(result);

        //    result = reader.GetAttribute("shieldgen");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    shipParameters.ShieldEnergyGenerator = int.Parse(result);

        //    result = reader.GetAttribute("weapon");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    shipParameters.WeaponEnergy = int.Parse(result);

        //    result = reader.GetAttribute("weapongen");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu silnika w pliku ");
        //    shipParameters.WeaponEnergyGenerator = int.Parse(result);
        //    return shipParameters;
        //}

        //public static WeaponMountingData GetWeaponMount(XmlTextReader reader)
        //{
        //    WeaponMountingData data = new WeaponMountingData();
        //    string result = reader.GetAttribute("lposition");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu punktu mocowania w pliku ");
        //    if (!MyVector.FromString(result, out data.LeftPosition))
        //    {
        //        throw new FileFormatException("Nie znaleziono opisu punktu mocowania w pliku ");
        //    }

        //    result = reader.GetAttribute("rposition");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu punktu mocowania w pliku ");
        //    if (!MyVector.FromString(result, out data.RightPosition))
        //    {
        //        throw new FileFormatException("Nie znaleziono opisu punktu mocowania w pliku ");
        //    }

        //    result = reader.GetAttribute("rocket");
        //    if (result == null)
        //        throw new FileFormatException("Nie znaleziono opisu punktu mocowania w pliku ");
        //    if (!MyVector.FromString(result, out data.RocketPosition))
        //    {
        //        throw new FileFormatException("Nie znaleziono opisu punktu mocowania w pliku ");
        //    }
        //    return data;
        //}
    }
}
