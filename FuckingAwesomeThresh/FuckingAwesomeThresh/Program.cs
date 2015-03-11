using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace FuckingAwesomeThresh
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameStart;
        }

        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        private static void Game_OnGameStart(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Thresh")
            {
                Notifications.AddNotification(new Notification("not thresh sin huh? wanna go m9?", 2));
                return;
            }
            Config = new Menu("Fucking Awesome Thresh", "you-stealing-me-src-m29?", true);

            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));

            var combo = Config.AddSubMenu(new Menu("Combo", "Combo"));
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(false));

            var harass = Config.AddSubMenu(new Menu("Harass", "Harass"));
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HE", "Use E").SetValue(false));

            var draw = Config.AddSubMenu(new Menu("Draw", "Draw"));
            draw.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(false, Color.White)));
            draw.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(false, Color.White)));
            draw.AddItem(new MenuItem("DE", "Draw E Range").SetValue(new Circle(false, Color.White)));
            draw.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(false, Color.White)));
            draw.AddItem(new MenuItem("DES", "Draw Escape Spots").SetValue(true));

            var escape = Config.AddSubMenu(new Menu("Misc", "Misc"));
            escape.AddItem(new MenuItem("LanternAlly", "Throw Lantern At Ally").SetValue(true));
            escape.AddItem(new MenuItem("Escape", "Escape").SetValue(new KeyBind('Z', KeyBindType.Press)));

            var info = Config.AddSubMenu(new Menu("Information", "info"));
            info.AddItem(new MenuItem("Msddsds", "if you would like to donate via paypal"));
            info.AddItem(new MenuItem("Msdsddsd", "you can do so by sending money to:"));
            info.AddItem(new MenuItem("Msdsadfdsd", "jayyeditsdude@gmail.com"));

            Config.AddItem(new MenuItem("Mgdgdfgsd", "Version: 0.0.1-2 BETA"));
            Config.AddItem(new MenuItem("Msd", "Made By FluxySenpai"));


            Config.AddToMainMenu();


            Notifications.AddNotification(new Notification("Fucking Awesome Thresh", 2));

        }
    }
}
