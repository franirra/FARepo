using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;

namespace FuckingAwesomeRiven
{
    class MenuHandler
    {

        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        public static void initMenu()
        {
            Config = new Menu("FuckingAwesomeRiven", "KappaChino", true);

            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalking", "OW")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));

            var combo = Config.AddSubMenu(new Menu("Combo", "Combo"));
            combo.AddItem(new MenuItem("xdxdxdxd", "-- Normal Combo"));

            var enabledCombos = combo.AddSubMenu(new Menu("Killable Combos", "Killable Combos"));
            enabledCombos.AddItem(new MenuItem("dfsjknfsj", "With R2"));
            enabledCombos.AddItem(new MenuItem("QWR2KS", "Q - W - R2").SetValue(true));
            enabledCombos.AddItem(new MenuItem("QR2KS", "Q - R2").SetValue(true));
            enabledCombos.AddItem(new MenuItem("WR2KS", "W - R2").SetValue(true));
            enabledCombos.AddItem(new MenuItem("dfdgdgdfggfdsj", ""));
            enabledCombos.AddItem(new MenuItem("dfgdhnfdsf", "Without R2"));
            enabledCombos.AddItem(new MenuItem("QWKS", "Q - W").SetValue(true));
            enabledCombos.AddItem(new MenuItem("QKS", "Q").SetValue(true));
            enabledCombos.AddItem(new MenuItem("WKS", "W").SetValue(true));

            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("UseQ-GC", "   Use Q for GapClose").SetValue(true));
            combo.AddItem(new MenuItem("Use R2", "Use R2").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("UseE-AA", "   Only E if out of AA range").SetValue(true));
            combo.AddItem(new MenuItem("UseE-GC", "   Use E to Gapclose").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("CR2", "Use R2").SetValue(true));
            combo.AddItem(new MenuItem("magnet", "Magnet Target").SetValue(false));
            combo.AddItem(new MenuItem("bdsfdfffsf", ""));
            combo.AddItem(new MenuItem("bdsfdsf", "-- Burst Combo"));
            combo.AddItem(new MenuItem("BFl", "Use Flash").SetValue(false));
            combo.AddItem(new MenuItem("bdsfdsff", "E - R - Flash - W - Q"));
            combo.AddItem(new MenuItem("bdsfdsfddd", "E - R - W - Q"));

            var farm = Config.AddSubMenu(new Menu("Farming", "Farming"));
            farm.AddItem(new MenuItem("fnjdsjkn", "          Last Hit"));
            farm.AddItem(new MenuItem("QLH", "Use Q").SetValue(true));
            farm.AddItem(new MenuItem("WLH", "Use W").SetValue(true));
            farm.AddItem(new MenuItem("10010321223", "          Jungle Clear"));
            farm.AddItem(new MenuItem("QJ", "Use Q").SetValue(true));
            farm.AddItem(new MenuItem("WJ", "Use W").SetValue(true));
            farm.AddItem(new MenuItem("EJ", "Use E").SetValue(true));
            farm.AddItem(new MenuItem("5622546001", "          Wave Clear"));
            farm.AddItem(new MenuItem("QWC", "Use Q").SetValue(true));
            farm.AddItem(new MenuItem("QWC-LH", "   Q Lasthit").SetValue(true));
            farm.AddItem(new MenuItem("QWC-AA", "   Q -> AA").SetValue(true));
            farm.AddItem(new MenuItem("WWC", "Use W").SetValue(true));


            var draw = Config.AddSubMenu(new Menu("Draw", "Draw"));

            draw.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(false, System.Drawing.Color.White)));
            draw.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(false, System.Drawing.Color.White)));
            draw.AddItem(new MenuItem("DE", "Draw E Range").SetValue(new Circle(false, System.Drawing.Color.White)));
            draw.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(false, System.Drawing.Color.White)));
            draw.AddItem(new MenuItem("DD", "Draw Damage [soon]").SetValue(new Circle(false, System.Drawing.Color.White)));

            var misc = Config.AddSubMenu(new Menu("Misc", "Misc"));
            misc.AddItem(new MenuItem("bonusCancelDelay", "Bonus Cancel Delay (ms)").SetValue(new Slider(0,0,500)));
            misc.AddItem(new MenuItem("keepQAlive", "Keep Q Alive").SetValue(true));
            misc.AddItem(new MenuItem("QFlee", "Q Flee").SetValue(true));
            misc.AddItem(new MenuItem("EFlee", "E Flee").SetValue(true));

            var Keybindings = Config.AddSubMenu(new Menu("Key Bindings", "KB"));
            Keybindings.AddItem(new MenuItem("normalCombo", "Normal Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            Keybindings.AddItem(new MenuItem("burstCombo", "Burst Combo").SetValue(new KeyBind('M', KeyBindType.Press)));
            Keybindings.AddItem(new MenuItem("jungleCombo", "Jungle Clear").SetValue(new KeyBind('C', KeyBindType.Press)));
            Keybindings.AddItem(new MenuItem("waveClear", "WaveClear").SetValue(new KeyBind('C', KeyBindType.Press)));
            Keybindings.AddItem(new MenuItem("lastHit", "LastHit").SetValue(new KeyBind('X', KeyBindType.Press)));
            Keybindings.AddItem(new MenuItem("flee", "Flee").SetValue(new KeyBind('Z', KeyBindType.Press)));

            var Info = Config.AddSubMenu(new Menu("Information", "info"));
            Info.AddItem(new MenuItem("Msddsds", "if you would like to donate via paypal"));
            Info.AddItem(new MenuItem("Msdsddsd", "you can do so by sending money to:"));
            Info.AddItem(new MenuItem("Msdsadfdsd", "jayyeditsdude@gmail.com"));
            Info.AddItem(new MenuItem("debug", "Debug Mode")).SetValue(false);

            Config.AddItem(new MenuItem("Mgdgdfgsd", "Version: 0.0.3 BETA"));
            Config.AddItem(new MenuItem("Msd", "Made By FluxySenpai"));

            Config.AddToMainMenu();
        }

        public static bool getMenuBool(String s)
        {
            return Config.Item(s).GetValue<bool>();
        }
    }
}
