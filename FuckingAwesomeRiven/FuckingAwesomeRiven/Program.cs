using System;
using System.Collections.Generic;
using FuckingAwesomeRiven.EvadeUtils;
using LeagueSharp;
using LeagueSharp.Common;
using SH = FuckingAwesomeRiven.SpellHandler;
using SpellSlot = LeagueSharp.SpellSlot;

namespace FuckingAwesomeRiven
{
    internal class Program
    {
        public static Obj_AI_Hero Player;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameStart;
        }

        private static void Game_OnGameStart(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }
            MenuHandler.initMenu();
            CheckHandler.init();
            Player = ObjectManager.Player;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameUpdate += eventArgs => StateHandler.tick();
            Obj_AI_Base.OnProcessSpellCast += CheckHandler.Obj_AI_Hero_OnProcessSpellCast;
            Obj_AI_Base.OnProcessSpellCast += AutoE.autoE;
            Drawing.OnDraw += DrawHandler.Draw;
            JumpHandler.load();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Queuer.Queue.Count > 0)
            {
                Queuer.doQueue();
            }
            if (MenuHandler.Config.Item("logPos").GetValue<bool>())
            {
                JumpHandler.addPos();
                MenuHandler.Config.Item("logPos").SetValue(false);
            }
            if (MenuHandler.Config.Item("printPos").GetValue<bool>())
            {
                JumpHandler.printToConsole();
                MenuHandler.Config.Item("printPos").SetValue(false);
            }
            if (MenuHandler.Config.Item("clearCurrent").GetValue<bool>())
            {
                JumpHandler.clearCurrent();
                MenuHandler.Config.Item("clearCurrent").SetValue(false);
            }
            if (MenuHandler.Config.Item("clearPrevious").GetValue<bool>())
            {
                JumpHandler.clearPrevious();
                MenuHandler.Config.Item("clearPrevious").SetValue(false);
            }

            CheckHandler.Checks();
            var Config = MenuHandler.Config;

            if (Config.Item("jungleCombo").GetValue<KeyBind>().Active)
            {
                StateHandler.JungleFarm();
            }
            if (MenuHandler.getMenuBool("keepQAlive") && SH._spells[SpellSlot.Q].IsReady() && CheckHandler.QCount >= 1 &&
                Environment.TickCount - CheckHandler.LastQ > 3650 && !Player.IsRecalling())
            {
                SH.CastQ();
            }

            if (Config.Item("normalCombo").GetValue<KeyBind>().Active)
            {
                StateHandler.mainCombo();
            }
            if (Config.Item("burstCombo").GetValue<KeyBind>().Active)
            {
                StateHandler.burstCombo();
            }
            else if (Config.Item("waveClear").GetValue<KeyBind>().Active)
            {
                StateHandler.laneclear();
            }
            else if (Config.Item("lastHit").GetValue<KeyBind>().Active)
            {
                StateHandler.lastHit();
            }
            else if (Config.Item("flee").GetValue<KeyBind>().Active)
            {
                StateHandler.flee();
            }
            else
            {
                MenuHandler.Orbwalker.SetAttack(true);
                MenuHandler.Orbwalker.SetMovement(true);
                Utility.DelayAction.Add(
                    2000, () =>
                    {
                        if (
                            !(Config.Item("flee").GetValue<KeyBind>().Active ||
                              Config.Item("lastHit").GetValue<KeyBind>().Active ||
                              Config.Item("waveClear").GetValue<KeyBind>().Active ||
                              Config.Item("burstCombo").GetValue<KeyBind>().Active ||
                              Config.Item("normalCombo").GetValue<KeyBind>().Active ||
                              Config.Item("jungleCombo").GetValue<KeyBind>().Active))
                        {
                            Queuer.Queue = new List<string>();
                        }
                    });
            }
        }
    }
}