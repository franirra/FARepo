using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace iFuckingAwesomeGraves
{
    class Program
    {

        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Notifications.AddNotification(new Notification("THIS IS NOT DONE", 2));

            Config = new Menu("iFuckingAwesomeGraves", "ifag", true);

            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));
            
            Config.AddToMainMenu();
        }
    }
}
