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
using System.Linq;
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
            CastBuckshot();
            CastSmokeScreen();
            CastQuickdraw();
            CastCollateralDamage();
        }

        #endregion

        #region spell casting

        private static void CastBuckshot()
        {
            var qTarget = TargetSelector.GetTarget(_spells[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);
            if (_menu.Item("useQC").GetValue<bool>() && qTarget.IsValidTarget(_spells[SpellSlot.Q].Range))
            {
                if (_spells[SpellSlot.Q].IsReady() && _spells[SpellSlot.Q].IsInRange(qTarget))
                {
                    _spells[SpellSlot.Q].CastIfHitchanceEquals(qTarget, HitChance.High); // TODO custom hitchance.
                }
            }
        }

        private static void CastSmokeScreen()
        {
            var wTarget = TargetSelector.GetTarget(_spells[SpellSlot.W].Range, TargetSelector.DamageType.Physical);
            if (_menu.Item("useWC").GetValue<bool>() && wTarget.IsValidTarget(_spells[SpellSlot.W].Range))
            {
                if (_spells[SpellSlot.W].IsReady() && _spells[SpellSlot.W].IsInRange(wTarget))
                {
                    _spells[SpellSlot.W].CastIfHitchanceEquals(wTarget, HitChance.High); // TODO custom hitchance
                }
            }
        }

        private static void CastQuickdraw()
        {
            //TODO
        }

        /// <summary>
        ///     TODO: IDK if this takes into account the damage after the initial range or if a unit is hit
        ///     TODO: if a unit is hit then the shell will explode in a 800 - range cone behind the inital unit.
        /// </summary>
        private static void CastCollateralDamage()
        {
            var rTarget = TargetSelector.GetTarget(_spells[SpellSlot.R].Range, TargetSelector.DamageType.Physical);
            if (_menu.Item("useRC").GetValue<bool>() && rTarget.IsValidTarget(_spells[SpellSlot.R].Range))
            {
                if (_spells[SpellSlot.R].IsReady() && _spells[SpellSlot.R].IsInRange(rTarget))
                {
                    if (_spells[SpellSlot.R].GetDamage(rTarget) > rTarget.Health + 10)
                    {
                        _spells[SpellSlot.R].CastIfHitchanceEquals(rTarget, HitChance.High); // TODO custom hitchance
                    }
                    else
                    {
                        foreach (Obj_AI_Hero source in
                            from source in
                                HeroManager.Enemies.Where(hero => hero.IsValidTarget(_spells[SpellSlot.R].Range))
                            let prediction = _spells[SpellSlot.R].GetPrediction(source, true)
                            where
                                _player.Distance(source) <= _spells[SpellSlot.R].Range &&
                                prediction.AoeTargetsHitCount >= 3
                            select source)
                        {
                            _spells[SpellSlot.R].CastIfHitchanceEquals(source, HitChance.High);
                        }
                    }
                }
            }
        }

        #endregion

        #region menu and spells

        private static void LoadSpells()
        {
            _spells[SpellSlot.Q].SetSkillshot(
                0.26f, 10f * 2 * (float) Math.PI / 180, 1950f, false, SkillshotType.SkillshotCone);
            _spells[SpellSlot.W].SetSkillshot(0.30f, 250f, 1650f, false, SkillshotType.SkillshotCircle);
            _spells[SpellSlot.R].SetSkillshot(0.22f, 150f, 2100f, true, SkillshotType.SkillshotLine);
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