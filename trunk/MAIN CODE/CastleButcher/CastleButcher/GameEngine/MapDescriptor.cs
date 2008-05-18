using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CastleButcher.GameEngine
{
    public class MapDescriptor
    {
        private string mapName;

        public string MapName
        {
            get { return mapName; }
            //set { name = value; }
        }
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            //set { name = value; }
        }
        private int maxPlayers;

        public int MaxPlayers
        {
            get { return maxPlayers; }
            //set { maxPlayers = value; }
        }
        /// <summary>
        /// Wczytuje opis mapy z pliku
        /// </summary>
        /// <param name="fileName">nazwa plku z map¹</param>
        public static MapDescriptor FromFile(string fileName)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            string result;
            MapDescriptor data = new MapDescriptor();
            data.fileName = fileName;
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Map")
                    {
                        data.mapName = reader.GetAttribute("name");
                        if (data.MapName == null)
                            throw new FileFormatException("Nie ma nazwy mapy w pliku " + fileName);

                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Map"))
                        {
                            reader.Read();
                            if (reader.Name == "Object")
                            {
                                result = reader.GetAttribute("type");
                                //ObjectType type;
                                switch (result)
                                {
                                    case "respawn":
                                        data.maxPlayers++;
                                        break;

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
    }
}
