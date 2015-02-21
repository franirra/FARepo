using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using SH = FuckingAwesomeRiven.SpellHandler;
using CH = FuckingAwesomeRiven.CheckHandler;

namespace FuckingAwesomeRiven
{
    class StateHandler
    {

        public static Obj_AI_Hero Target;

        public static Obj_AI_Hero Player;

        public static void tick()
        {
            Target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            Player = ObjectManager.Player;
        }

        public static void lastHit()
        {
            var minion = MinionManager.GetMinions(Player.Position, SH.QRange).FirstOrDefault();

            if (Queuer.Queue.Count > 0) return;

            if (minion == null) return;

            if (SH._spells[SpellSlot.W].IsReady() && MenuHandler.getMenuBool("WLH") && CH.CanW && Environment.TickCount - CH.LastE >= 250 && minion.IsValidTarget(SH._spells[SpellSlot.W].Range) && SH._spells[SpellSlot.W].GetDamage(minion) > minion.Health)
            {
                SH.CastW();
            }

            if (SH._spells[SpellSlot.Q].IsReady() && MenuHandler.getMenuBool("QLH") && Environment.TickCount - CH.LastE >= 250 && (SH._spells[SpellSlot.Q].GetDamage(minion) > minion.Health))
            {
                if (minion.IsValidTarget(SH.QRange) && CH.CanQ)
                {
                    SH.CastQ(minion);
                }
            }
        }

        public static void laneclear()
        {
            var minion = MinionManager.GetMinions(Player.Position, SH.QRange).FirstOrDefault();

            if (!minion.IsValidTarget()) return;

            if (Queuer.Queue.Count > 0) return;

            if (CH.LastTiamatCancel < Environment.TickCount)
            {
                CH.LastTiamatCancel = int.MaxValue;
                SH.castItems(Target);
            }

            if (HealthPrediction.GetHealthPrediction(minion, (int) (ObjectManager.Player.AttackCastDelay * 1000)) > 0 &&
                Player.GetAutoAttackDamage(minion) >
                HealthPrediction.GetHealthPrediction(minion, (int) (ObjectManager.Player.AttackCastDelay * 1000)))
            {
                SH.Orbwalk(minion);
            }

            if (SH._spells[SpellSlot.W].IsReady() && MenuHandler.getMenuBool("WWC") && CH.CanW && Environment.TickCount - CH.LastE >= 250 && minion.IsValidTarget(SH._spells[SpellSlot.W].Range) && SH._spells[SpellSlot.W].GetDamage(minion) > minion.Health)
            {
                SH.CastW();
            }

            if (SH._spells[SpellSlot.Q].IsReady() && MenuHandler.getMenuBool("QWC") && Environment.TickCount - CH.LastE >= 250 && (SH._spells[SpellSlot.Q].GetDamage(minion) + Player.GetAutoAttackDamage(minion) > minion.Health && MenuHandler.getMenuBool("QWC-AA")) || (SH._spells[SpellSlot.Q].GetDamage(minion) > minion.Health && MenuHandler.getMenuBool("QWC-LH")))
            {
                if (minion.IsValidTarget(SH.QRange) && CH.CanQ)
                {
                    SH.CastQ(minion);
                }
            }
        }

        public static void JungleFarm()
        {
            var minion =
                MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!minion.IsValidTarget())
                return;

            if (Queuer.Queue.Count > 0) return;

            if (CH.LastTiamatCancel < Environment.TickCount)
            {
                CH.LastTiamatCancel = int.MaxValue;
                SH.castItems(Target);
            }

            SH.Orbwalk(minion);

            if (SH._spells[SpellSlot.E].IsReady() && CH.CanE && MenuHandler.getMenuBool("EJ"))
            {
                if (minion.IsValidTarget(SH._spells[SpellSlot.E].Range))
                {
                    SH._spells[SpellSlot.E].Cast(minion.Position);
                }
            }

            if (SH._spells[SpellSlot.W].IsReady() && CH.CanW && Environment.TickCount - CH.LastE >= 250 && minion.IsValidTarget(SH._spells[SpellSlot.W].Range) && MenuHandler.getMenuBool("WJ"))
            {
                SH.CastW();
            }
            SH.castItems(minion);
            if (SH._spells[SpellSlot.Q].IsReady() && Environment.TickCount - CH.LastE >= 250 && MenuHandler.getMenuBool("QJ"))
            {
                if (minion.IsValidTarget(SH.QRange) && CH.CanQ)
                {
                    SH.CastQ(minion);
                    return;
                }
            }
        }

        public static void mainCombo()
        {

            SH.Orbwalk(Target);

            if (Queuer.Queue.Count > 0) return;

            if(!Target.IsValidTarget()) return;


            var comboRDmg = DamageHandler.getComboDmg(true, Target);
            var comboNoR = DamageHandler.getComboDmg(false, Target);

            if (CH.LastECancelSpell < Environment.TickCount && MenuHandler.getMenuBool("autoCancelE"))
            {
                CH.LastECancelSpell = int.MaxValue;
                SH.CastE(Target.Position);
            }
            if (CH.LastTiamatCancel < Environment.TickCount && MenuHandler.getMenuBool("autoCancelT"))
            {
                CH.LastTiamatCancel = int.MaxValue;
               SH.castItems(Target);
            }

            if (MenuHandler.getMenuBool("CR"))
            {
                if (SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() &&
                    SH._spells[SpellSlot.Q].IsReady() && comboNoR < Target.Health && comboRDmg > Target.Health)
                {
                    Queuer.add("E", Target.Position);
                    Queuer.add("Hydra");
                    Queuer.add("R");
                    Queuer.add("Q");
                    return;
                }
            }

            if (CH.RState)
            {

                if (MenuHandler.getMenuBool("QWR2KS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.Q].IsReady() &&
                    SH._spells[SpellSlot.Q].GetDamage(Target) + (CH.RState && SH._spells[SpellSlot.R].IsReady() ? SH._spells[SpellSlot.R].GetDamage(Target) : 0) >
                    Target.Health)
                {
                    Queuer.add("Q");
                    Queuer.add("W");
                    Queuer.add("R2", Target);
                }
                else if (MenuHandler.getMenuBool("QR2KS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.Q].IsReady() &&
                         SH._spells[SpellSlot.Q].GetDamage(Target) + (CH.RState && SH._spells[SpellSlot.R].IsReady() ? SH._spells[SpellSlot.R].GetDamage(Target) : 0) > Target.Health)
                {
                    Queuer.add("Q");
                    Queuer.add("R2", Target);
                }
                else if (MenuHandler.getMenuBool("WR2KS") && Target.IsValidTarget(SH.WRange) && CH.CanW && SH._spells[SpellSlot.W].IsReady() &&
                         SH._spells[SpellSlot.W].GetDamage(Target) + (CH.RState && SH._spells[SpellSlot.R].IsReady() ? SH._spells[SpellSlot.R].GetDamage(Target) : 0) > Target.Health)
                {
                    Queuer.add("W");
                    Queuer.add("R2", Target);
                }
            }
            else
            {
                if (MenuHandler.getMenuBool("QWKS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.Q].GetDamage(Target) + SH._spells[SpellSlot.W].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("Q");
                    Queuer.add("W");
                }
                else if (MenuHandler.getMenuBool("QKS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.Q].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("Q");
                }
                else if (MenuHandler.getMenuBool("WKS") && Target.IsValidTarget(SH.WRange) && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.W].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("W");
                }
            }

            var BonusRange = Orbwalking.GetRealAutoAttackRange(Player) + (Target.BoundingRadius / 2) - 50;

            if (Target == null) return;

            if (MenuHandler.getMenuBool("CE") && SH._spells[SpellSlot.E].IsReady() && CH.CanE)
            {
                if (MenuHandler.getMenuBool("UseE-GC"))
                {
                    if (!Target.IsValidTarget(SH._spells[SpellSlot.E].Range - BonusRange + 50) &&
                        Target.IsValidTarget(SH._spells[SpellSlot.E].Range + BonusRange))
                    {
                        SH.CastE(Target.Position);
                    }
                    else if (SH._spells[SpellSlot.Q].IsReady() &&
                             !Target.IsValidTarget(SH._spells[SpellSlot.E].Range + BonusRange) &&
                             Target.IsValidTarget(SH._spells[SpellSlot.E].Range + SH._spells[SpellSlot.Q].Range - 50))
                    {
                        SH.CastE(Target.Position);
                    }
                }
                else if (Vector3.Distance(Player.Position, Target.Position) > Orbwalking.GetRealAutoAttackRange(Player))
                {
                    SH.CastE(Target.Position);
                }
            }

            if (MenuHandler.getMenuBool("CW") && SH._spells[SpellSlot.W].IsReady() && CH.CanW && Environment.TickCount - CH.LastE >= 100 && Target.IsValidTarget(SH._spells[SpellSlot.W].Range))
            {
                SH.CastW();
            }

            if (SH._spells[SpellSlot.Q].IsReady() && Environment.TickCount - CH.LastE >= 100 && MenuHandler.getMenuBool("CQ"))
            {
                if (Target.IsValidTarget(SH.QRange) && CH.CanQ && MenuHandler.Config.Item("QAA").GetValue<StringList>().SelectedIndex == 0)
                {
                    SH.CastQ(Target);
                    return;
                }
                if (!Target.IsValidTarget(SH.QRange + Orbwalking.GetRealAutoAttackRange(Player)) && !Orbwalking.InAutoAttackRange(Target) && MenuHandler.getMenuBool("UseQ-GC2"))
                {
                    SH.CastQ(Target);
                }
            }
        }

        public static bool castedFlash = false;
        public static bool castedTia;

        public static void burstCombo()
        {
            SH.Orbwalk(Target);

            if (!Target.IsValidTarget()) return;

            //kyzer 3rd q combo
            if (MenuHandler.getMenuBool("shyCombo") && Target.IsValidTarget(600) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && CH.QCount == 2 && Queuer.Queue.Count == 0))
            {
                Queuer.add("E", Target.Position);
                Queuer.add("R");
                Queuer.add("Flash", Target.Position, true);
                Queuer.add("Q");
                Queuer.add("AA");
                Queuer.add("Hydra");
                Queuer.add("W");
                Queuer.add("AA");
                Queuer.add("R2", Target);
                Queuer.add("Q");
            }

            // Shy combo
            if (MenuHandler.getMenuBool("kyzerCombo") && Target.IsValidTarget(600) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && Queuer.Queue.Count == 0)
            {
                Queuer.add("E", Target.Position);
                Queuer.add("R");
                Queuer.add("Flash", Target.Position, true);
                Queuer.add("AA");
                Queuer.add("Hydra");
                Queuer.add("W");
                Queuer.add("R2", Target);
                Queuer.add("Q");
            }

            //e>r>w>hydra>q>aa>q>aa>q>aa>r

            if (Queuer.Queue.Count > 0) return;
            mainCombo();

        }

        public static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (SH._spells[SpellSlot.E].IsReady() && CH.LastQ + 250 < Environment.TickCount && MenuHandler.getMenuBool("EFlee"))
            {
                SH.CastE(Game.CursorPos);
            }

            if (SH._spells[SpellSlot.Q].IsReady() && CH.LastE + 250 < Environment.TickCount && MenuHandler.getMenuBool("QFlee"))
            {
                SH.CastQ();
            }
        }
    }
}
