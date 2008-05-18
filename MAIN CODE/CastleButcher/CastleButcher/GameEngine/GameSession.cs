using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Klasa zarządzająca "sesją" gry. Przechowuje informację o graczaach i drużynach. Jej klasy pochodne to
    /// GameServer, która reprezentuję lokalną grę z podłączonymi zdalbymi graczami. 
    /// </summary>
    public abstract class GameSession:IUpdateable
    {
        List<Player> players;

        public List<Player> Players
        {
            get { return players; }
            set { players = value; }
        }
        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
