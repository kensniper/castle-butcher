using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CastleButcher.GameEngine
{
    public class MapData
    {
        private string name;

        public string Name
        {
            get { return name; }
            //set { name = value; }
        }
        private int maxPlayers;

        public int MaxPlayers
        {
            get { return maxPlayers; }
            //set { maxPlayers = value; }
        }

        List<IGameObject> gameObjects;

        public List<IGameObject> GameObjects
        {
            get { return gameObjects; }
            //set { gameObjects = value; }
        }

        /// <summary>
        /// Wczytuje opis mapy z pliku
        /// </summary>
        /// <param name="fileName">nazwa plku z map¹</param>
        public static MapData FromFile(string fileName)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            //string result;
            MapData data = new MapData();
            data.gameObjects = new List<IGameObject>();
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Map")
                    {
                        data.name = reader.GetAttribute("name");
                        if (data.Name == null)
                            throw new FileFormatException("Nie ma nazwy mapy w pliku " + fileName);

                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Map"))
                        {
                            reader.Read();
                            if (reader.Name == "Object")
                            {
                                ObjectType type;
                                IGameObject obj;
                                obj = XmlNodeReader.GetObjectNode(reader, out type);
                                data.GameObjects.Add(obj);
                            }
                            else if (reader.Name == "RespawnPointList")
                            {
                                
                                RespawnPoint resp;
                                ObjectType type;
                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "RespawnPointList"))
                                {
                                    reader.Read();
                                    if (reader.Name == "Assassins")
                                    {                                        
                                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Assassins"))
                                        {
                                            reader.Read();
                                            resp = (RespawnPoint)XmlNodeReader.GetObjectNode(reader, out type);
                                            resp.Team = GameTeam.Assassins;
                                            data.GameObjects.Add(resp);

                                        }
                                    }
                                    else if (reader.Name == "Knights")
                                    {
                                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Assassins"))
                                        {
                                            reader.Read();
                                            resp = (RespawnPoint)XmlNodeReader.GetObjectNode(reader, out type);
                                            resp.Team = GameTeam.Knights;
                                            data.GameObjects.Add(resp);

                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return data;
        }

        /// <summary>
        /// Wczytuje opis mapy z pliku
        /// </summary>
        /// <param name="fileName">nazwa plku z map¹</param>
        /// <param name="reporter">Klasa do powiadamiania o postêpie</param>
        public static void FromFileBkg(string fileName,ProgressReporter reporter)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            //string result;
            MapData data = new MapData();
            data.gameObjects = new List<IGameObject>();

            reporter.Info = "Otwarto plik";
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Map")
                    {
                        data.name = reader.GetAttribute("name");
                        if (data.Name == null)
                            throw new FileFormatException("Nie ma nazwy mapy w pliku " + fileName);

                        reporter.Info = "Wczytuje mapê: " + data.name;
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Map"))
                        {
                            reader.Read();
                            if (reader.Name == "Object")
                            {
                                ObjectType type;
                                IGameObject obj;
                                obj = XmlNodeReader.GetObjectNode(reader, out type);
                                data.GameObjects.Add(obj);
                                reporter.Info = "Wczytano obiekt: " + obj.Name;
                            }
                            else if (reader.Name == "RespawnPointList")
                            {

                                RespawnPoint resp;
                                ObjectType type;
                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "RespawnPointList"))
                                {
                                    reader.Read();
                                    if (reader.Name == "Assassins")
                                    {
                                        reader.Read();
                                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Assassins"))
                                        {
                                            
                                            resp = (RespawnPoint)XmlNodeReader.GetObjectNode(reader, out type);
                                            resp.Team = GameTeam.Assassins;
                                            data.GameObjects.Add(resp);
                                            reporter.Info = "Wczytano obiekt: " + resp.Name;
                                            reader.Read();

                                        }
                                    }
                                    else if (reader.Name == "Knights")
                                    {
                                        reader.Read();
                                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Knights"))
                                        {
                                            resp = (RespawnPoint)XmlNodeReader.GetObjectNode(reader, out type);
                                            resp.Team = GameTeam.Knights;
                                            data.GameObjects.Add(resp);
                                            reporter.Info = "Wczytano obiekt: " + resp.Name;
                                            reader.Read();

                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            reporter.Data = data;
            //reporter.Complete = true;
        }
    }
}
