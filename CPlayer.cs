using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace CawAIO
{
    public class CPlayers
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public int RandomMapTeleportCooldown { get; set; }
        public int RandomPlayerTeleport { get; set; }
        public int GambleCooldown { get; set; }
        public int MonsterGambleCooldown { get; set; }
        public int WarningCount { get; set; }

        public CPlayers(int index)
        {
            this.Index = index;
        }
    }
}