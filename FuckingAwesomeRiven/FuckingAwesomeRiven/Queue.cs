using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using C = FuckingAwesomeRiven.CheckHandler;
using S = FuckingAwesomeRiven.SpellHandler;

namespace FuckingAwesomeRiven
{
    internal class Queuer
    {
        public static List<String> Queue = new List<string>();
        public static Obj_AI_Base R2Target;
        public static Vector3 EPos;
        public static Vector3 FlashPos;
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static readonly int QDelay = (int)(ObjectManager.Player.AttackCastDelay * 1000 - (Game.Ping * 0.5));

        public static void DoQueue()
        {
            if (Queue.Count == 0)
            {
                return;
            }

            switch (Queue[0])
            {
                case "AA":
                    Aa();
                    break;
                case "Q":
                    Qq();
                    break;
                case "W":
                    Qw();
                    break;
                case "E":
                    Qe();
                    break;
                case "R":
                    Qr();
                    break;
                case "R2":
                    Qr2();
                    break;
                case "Flash":
                    Flash();
                    break;
                case "Hydra":
                    Hydra();
                    break;
            }
        }

        public static void Add(String spell)
        {
            Queue.Add(spell);
        }

        public static void Add(String spell, Obj_AI_Base target)
        {
            Queue.Add(spell);
            R2Target = target;
        }

        public static void Add(String spell, Vector3 pos, bool isFlash = false)
        {
            Queue.Add(spell);
            if (isFlash)
            {
                FlashPos = pos;
                return;
            }

            EPos = pos;
        }

        public static void Remove(String spell)
        {
            if (Queue.Count == 0 || Queue[0] != spell)
            {
                return;
            }

            Queue.RemoveAt(0);
        }

        private static void Aa()
        {
            if (Environment.TickCount - CheckHandler.LastAa < Game.Ping + 80 || Player.IsWindingUp)
                return;
            if (StateHandler.Target == null || StateHandler.Target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 40) || !Player.IsWindingUp && CheckHandler.LastAa + 80 < Environment.TickCount)
            {
                Remove("AA");
                return;
            }
            Player.IssueOrder(GameObjectOrder.AttackUnit, StateHandler.Target);
        }

        private static void Qq()
        {
            if (Player.IsWindingUp || CheckHandler.LastAa + 60 + Game.Ping > Environment.TickCount)
            {
                return;
            }

            if (!S.Spells[SpellSlot.Q].IsReady())
            {
                Queue.Remove("Q");
                return;
            }

            if (S.Spells[SpellSlot.Q].IsReady())
            {
                S.CastQ(StateHandler.Target);
            }
        }

        private static void Qw()
        {
            if (Player.IsWindingUp || CheckHandler.LastAa + 60 + Game.Ping > Environment.TickCount)
            {
                return;
            }

            if (!S.Spells[SpellSlot.W].IsReady())
            {
                Queue.Remove("W");
                return;
            }

            if (S.Spells[SpellSlot.W].IsReady())
            {
                S.CastW(StateHandler.Target);
            }
        }

        private static void Qe()
        {
            if (Player.IsWindingUp || CheckHandler.LastAa + 60 + Game.Ping > Environment.TickCount)
            {
                return;
            }

            if (!S.Spells[SpellSlot.E].IsReady() || !EPos.IsValid())
            {
                Queue.Remove("E");
                return;
            }

            if (S.Spells[SpellSlot.E].IsReady())
            {
                S.CastE(StateHandler.Target.IsValidTarget() ? StateHandler.Target.Position : EPos);
            }
        }

        private static void Qr()
        {
            if (Player.IsWindingUp || CheckHandler.LastAa + 60 + Game.Ping > Environment.TickCount)
            {
                return;
            }

            if (!S.Spells[SpellSlot.Q].IsReady() || C.RState)
            {
                Queue.Remove("R");
                return;
            }

            if (S.Spells[SpellSlot.R].IsReady())
            {
                S.CastR();
            }
        }

        private static void Qr2()
        {
            if (Player.IsWindingUp || CheckHandler.LastAa + 60 + Game.Ping > Environment.TickCount)
            {
                return;
            }

            if (!S.Spells[SpellSlot.R].IsReady() || R2Target == null)
            {
                Queue.Remove("R2");
                return;
            }

            if (!S.Spells[SpellSlot.R].IsReady() || !C.RState || !R2Target.IsValidTarget())
            {
                return;
            }

            var r2 = new Spell(SpellSlot.R, 900);
            r2.SetSkillshot(0.25f, 45, 1200, false, SkillshotType.SkillshotCone);
            r2.Cast(R2Target);
        }

        private static void Hydra()
        {
            if (Player.IsWindingUp || CheckHandler.LastAa + 60 + Game.Ping > Environment.TickCount)
            {
                return;
            }

            if (!ItemData.Tiamat_Melee_Only.GetItem().IsReady() &&
                !ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                Queue.Remove("Hydra");
                return;
            }

            if (!ItemData.Tiamat_Melee_Only.GetItem().IsReady() &&
                !ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                return;
            }

            ItemData.Tiamat_Melee_Only.GetItem().Cast();
            ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }

        private static void Flash()
        {
            if (!S.SummonerDictionary[SpellHandler.SummonerSpell.Flash].IsReady() || !FlashPos.IsValid())
            {
                Queue.Remove("Flash");
                return;
            }

            SpellHandler.CastFlash(FlashPos);
        }
    }
}