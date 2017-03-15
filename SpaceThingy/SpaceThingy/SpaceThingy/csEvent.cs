using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceThingy
{
    public static class csEvent
    {
        public enum events { None, ReleasePlayerFromStation, RefreshRocketAmmo, OpenShop };

        public static void executeEvent(events item)
        {
            switch (item)
            {
                case events.ReleasePlayerFromStation:
                    Game1.player.station = null;
                    csAnimation.execute(csAnimation.animationType.ZoomIn);
                    break;
                case events.RefreshRocketAmmo:
                    Game1.player.CurrRocketCount = Game1.player.MaxRockets;
                    break;
            }
            
        }
    }
}
