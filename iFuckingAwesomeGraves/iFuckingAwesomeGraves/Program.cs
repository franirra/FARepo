// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace iFuckingAwesomeGraves
{
    internal class Program
    {
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu _menu;
        private static Obj_AI_Hero _player;

        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<SpellSlot, Spell> _spells = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 800f) },
            { SpellSlot.W, new Spell(SpellSlot.W, 950f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 425f) },
            { SpellSlot.R, new Spell(SpellSlot.R, 1100f) }
        };

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #region Events

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            Notifications.AddNotification(new Notification("THIS IS NOT DONE", 2));

            LoadSpells();
            CreateMenu();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (_player.IsDead)
            {
                return;
            }
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    DoCombo();
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            //TODO drawing
        }

        #endregion

        #region ActiveModes

        private static void DoCombo()
        {
            //TODO combo
        }

        #endregion

        #region menu and spells

        private static void LoadSpells()
        {
            _spells[SpellSlot.Q].SetSkillshot(
                0.26f, 10f * 2 * (float) Math.PI / 180, 1950, false, SkillshotType.SkillshotCone);
            _spells[SpellSlot.W].SetSkillshot(0.30f, 250f, 1650f, false, SkillshotType.SkillshotCircle);
            _spells[SpellSlot.R].SetSkillshot(0.22f, 150f, 2100, true, SkillshotType.SkillshotLine);
        }

        private static void CreateMenu()
        {
            _menu = new Menu("iFuckingAwesomeGraves", "ifag", true);
            //LOL iJabba = iFag = iFuckingAwesomeGraves, your a CUNT L0L

            TargetSelector.AddToMenu(_menu.AddSubMenu(new Menu("Target Selector", "Target Selector")));

            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));

            var comboMenu = new Menu("Graves - Combo", "com.ifag.combo");
            {
                comboMenu.AddItem(new MenuItem("useQC", "Use Q Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("useWC", "Use W Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("useEC", "Use E Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("useRC", "Use R Combo").SetValue(true));
                _menu.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Graves - Harass", "com.ifag.harass");
            {
                harassMenu.AddItem(new MenuItem("useQH", "Use Q Harass").SetValue(true));
                harassMenu.AddItem(new MenuItem("useWH", "Use W Harass").SetValue(true));
                _menu.AddSubMenu(harassMenu);
                //TODO harass mana
            }

            var farmMenu = new Menu("Graves - Farm", "com.ifag.farm");
            {
                var laneclearMenu = new Menu("Laneclear", "com.ifag.farm.lc");
                {
                    laneclearMenu.AddItem(new MenuItem("useQLC", "Use Q Laneclear").SetValue(false));
                }
                var lasthitMenu = new Menu("Lasthit", "com.ifag.farm.lh");
                {
                    lasthitMenu.AddItem(new MenuItem("useQLH", "Use Q Last hit").SetValue(false));
                }
                farmMenu.AddSubMenu(laneclearMenu);
                farmMenu.AddSubMenu(lasthitMenu);
                _menu.AddSubMenu(farmMenu);
            }


            _menu.AddToMainMenu();
        }

        #endregion
    }
}