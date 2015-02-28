using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SH = FuckingAwesomeRiven.SpellHandler;
using CH = FuckingAwesomeRiven.CheckHandler;

namespace FuckingAwesomeRiven
{
    internal class StateHandler
    {
        public static Obj_AI_Hero Target;
        public static Obj_AI_Hero Player;
        public static bool castedFlash = false;
        public static bool castedTia;

        public static void tick()
        {
            Target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            Player = ObjectManager.Player;
        }

        public static void lastHit()
        {
            var minion = MinionManager.GetMinions(Player.Position, SH.QRange).FirstOrDefault();

            if (Queuer.Queue.Count > 0)
            {
                return;
            }

            if (minion == null)
            {
                return;
            }

            if (SH._spells[SpellSlot.W].IsReady() && MenuHandler.getMenuBool("WLH") && CH.CanW &&
                Environment.TickCount - CH.LastE >= 250 && minion.IsValidTarget(SH._spells[SpellSlot.W].Range) &&
                SH._spells[SpellSlot.W].GetDamage(minion) > minion.Health)
            {
                SH.CastW();
            }

            if (SH._spells[SpellSlot.Q].IsReady() && MenuHandler.getMenuBool("QLH") &&
                Environment.TickCount - CH.LastE >= 250 && (SH._spells[SpellSlot.Q].GetDamage(minion) > minion.Health))
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

            if (!minion.IsValidTarget())
            {
                return;
            }

            if (Queuer.Queue.Count > 0)
            {
                return;
            }

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

            if (SH._spells[SpellSlot.W].IsReady() && MenuHandler.getMenuBool("WWC") && CH.CanW &&
                Environment.TickCount - CH.LastE >= 250 && minion.IsValidTarget(SH._spells[SpellSlot.W].Range) &&
                SH._spells[SpellSlot.W].GetDamage(minion) > minion.Health)
            {
                SH.CastW();
            }

            if (SH._spells[SpellSlot.Q].IsReady() && MenuHandler.getMenuBool("QWC") &&
                Environment.TickCount - CH.LastE >= 250 &&
                (SH._spells[SpellSlot.Q].GetDamage(minion) + Player.GetAutoAttackDamage(minion) > minion.Health &&
                 MenuHandler.getMenuBool("QWC-AA")) ||
                (SH._spells[SpellSlot.Q].GetDamage(minion) > minion.Health && MenuHandler.getMenuBool("QWC-LH")))
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
                MinionManager.GetMinions(
                    Player.Position, 600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault();

            if (!minion.IsValidTarget())
            {
                return;
            }

            if (Queuer.Queue.Count > 0)
            {
                return;
            }

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

            if (SH._spells[SpellSlot.W].IsReady() && CH.CanW && Environment.TickCount - CH.LastE >= 250 &&
                minion.IsValidTarget(SH._spells[SpellSlot.W].Range) && MenuHandler.getMenuBool("WJ"))
            {
                SH.CastW();
            }
            SH.castItems(minion);
            if (SH._spells[SpellSlot.Q].IsReady() && Environment.TickCount - CH.LastE >= 250 &&
                MenuHandler.getMenuBool("QJ"))
            {
                if (minion.IsValidTarget(SH.QRange) && CH.CanQ)
                {
                    SH.CastQ(minion);
                }
            }
        }

        public static void addQAA(bool qFirst = false)
        {
            var qAA = MenuHandler.Config.Item("QAA").GetValue<StringList>().SelectedIndex == 1;
            if (qFirst)
            {
                Queuer.add("Q");
            }
            if (qAA)
            {
                Queuer.add("AA");
                Queuer.add("Q");
                return;
            }
            Queuer.add("Q");
            Queuer.add("AA");
        }

        public static void mainCombo()
        {
            SH.Orbwalk(Target);

            if (Queuer.Queue.Count > 0)
            {
                return;
            }

            if (!Target.IsValidTarget())
            {
                return;
            }

            var comboRDmg = DamageHandler.getComboDmg(true, Target);
            var comboNoR = DamageHandler.getComboDmg(false, Target);

            if (MenuHandler.getMenuBool("CR") &&
                (MenuHandler.Config.Item("forcedR").GetValue<KeyBind>().Active ||
                 comboNoR < Target.Health && comboRDmg > Target.Health) && SH._spells[SpellSlot.R].IsReady() &&
                !CH.RState)
            {
                if (MenuHandler.getMenuBool("CREWHQ") && SH._spells[SpellSlot.Q].IsReady() &&
                    SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.E].IsReady() &&
                    Target.IsValidTarget(325 + SpellHandler.QRange))
                {
                    Queuer.add("R");
                    Queuer.add("E", Target.Position);
                    Queuer.add("W");
                    Queuer.add("Hydra");
                    addQAA(true);
                    return;
                }

                if (MenuHandler.getMenuBool("CREWH") && SH._spells[SpellSlot.E].IsReady() &&
                    SH._spells[SpellSlot.W].IsReady() && Target.IsValidTarget(325 + SpellHandler.WRange))
                {
                    Queuer.add("R");
                    Queuer.add("E", Target.Position);
                    Queuer.add("W");
                    Queuer.add("Hydra");
                    return;
                }

                if (MenuHandler.getMenuBool("CREAAHQ") && SH._spells[SpellSlot.Q].IsReady() &&
                    SH._spells[SpellSlot.E].IsReady() && Target.IsValidTarget(325 + SpellHandler.QRange))
                {
                    Queuer.add("R");
                    Queuer.add("E", Target.Position);
                    Queuer.add("AA");
                    Queuer.add("Hydra");
                    addQAA(true);
                    return;
                }

                if (MenuHandler.getMenuBool("CRWAAHQ") && SH._spells[SpellSlot.Q].IsReady() &&
                    SH._spells[SpellSlot.W].IsReady() && Target.IsValidTarget(SpellHandler.QRange))
                {
                    Queuer.add("R");
                    Queuer.add("W");
                    Queuer.add("AA");
                    Queuer.add("Hydra");
                    addQAA(true);
                    return;
                }

                if (MenuHandler.getMenuBool("CR1CC") && SH._spells[SpellSlot.R].IsReady())
                {
                    Queuer.add("R");
                    return;
                }
            }

            if (MenuHandler.getMenuBool("CR2") && SH._spells[SpellSlot.R].IsReady() && CH.RState)
            {
                if (MenuHandler.getMenuBool("CR2WQ") && SH._spells[SpellSlot.Q].IsReady() &&
                    SH._spells[SpellSlot.W].IsReady() &&
                    SpellHandler._spells[SpellSlot.R].GetDamage(Target) +
                    SpellHandler._spells[SpellSlot.W].GetDamage(Target) +
                    SpellHandler._spells[SpellSlot.Q].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("R2", Target);
                    Queuer.add("W");
                    addQAA(true);
                    return;
                }
                if (MenuHandler.getMenuBool("CR2W") && SH._spells[SpellSlot.W].IsReady() &&
                    SpellHandler._spells[SpellSlot.R].GetDamage(Target) +
                    SpellHandler._spells[SpellSlot.W].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("R2", Target);
                    Queuer.add("W");
                    return;
                }
                if (MenuHandler.getMenuBool("CR2Q") && SH._spells[SpellSlot.Q].IsReady() &&
                    SpellHandler._spells[SpellSlot.R].GetDamage(Target) +
                    SpellHandler._spells[SpellSlot.Q].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("R2", Target);
                    addQAA(true);
                    return;
                }
                if (MenuHandler.getMenuBool("CR2CC") &&
                    SpellHandler._spells[SpellSlot.R].GetDamage(Target) > Target.Health)
                {
                    Queuer.add("R2", Target);
                    return;
                }
            }

            // skills based on cds / engages
            if (MenuHandler.getMenuBool("CEWHQ") && SH._spells[SpellSlot.Q].IsReady() &&
                SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.E].IsReady() &&
                Target.IsValidTarget(325 + SpellHandler.QRange))
            {
                Queuer.add("E", Target.Position);
                Queuer.add("W");
                Queuer.add("Hydra");
                addQAA();
                return;
            }

            if (MenuHandler.getMenuBool("CQWH") && SH._spells[SpellSlot.Q].IsReady() &&
                SH._spells[SpellSlot.W].IsReady() && CH.QCount == 2 && Target.IsValidTarget(SpellHandler.QRange))
            {
                Queuer.add("Q");
                Queuer.add("W");
                Queuer.add("Hydra");
                addQAA();
                return;
            }

            if (MenuHandler.getMenuBool("CEHQ") && SH._spells[SpellSlot.Q].IsReady() &&
                SH._spells[SpellSlot.E].IsReady() && Target.IsValidTarget(SH.QRange + 325))
            {
                Queuer.add("E", Target.Position);
                Queuer.add("Hydra");
                Queuer.add("Q");
                addQAA();
                return;
            }

            if (MenuHandler.getMenuBool("CEW") && SH._spells[SpellSlot.E].IsReady() &&
                Target.IsValidTarget(325 + SH.WRange) && !Orbwalking.InAutoAttackRange(Target))
            {
                Queuer.add("E", Target.Position);
                Queuer.add("W");
                return;
            }
            //end

            // when only one skill is up
            if (MenuHandler.getMenuBool("CW") && SH._spells[SpellSlot.W].IsReady() && Target.IsValidTarget(SH.WRange))
            {
                Queuer.add("W");
                Queuer.add("Hydra");
                return;
            }

            if (MenuHandler.getMenuBool("CE") && SH._spells[SpellSlot.E].IsReady() &&
                Target.IsValidTarget(325 + Orbwalking.GetRealAutoAttackRange(Player)) &&
                !Orbwalking.InAutoAttackRange(Target))
            {
                Queuer.add("E", Target.Position);
                return;
            }

            if (MenuHandler.getMenuBool("CQ") && SH._spells[SpellSlot.Q].IsReady() && Target.IsValidTarget(SH.QRange))
            {
                addQAA();
            }
            // end
        }

        public static void burstCombo()
        {
            SH.Orbwalk(Target);

            if (!Target.IsValidTarget())
            {
                return;
            }

            if (Queuer.Queue.Count > 0)
            {
                return;
            }

            if (MenuHandler.getMenuBool("flashlessBurst") && Target.IsValidTarget(325 + SH.WRange) &&
                SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() &&
                SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && Queuer.Queue.Count == 0)
            {
                Queuer.add("E", Target.Position);
                Queuer.add("R");
                Queuer.add("W");
                Queuer.add("Hydra");
                Queuer.add("AA");
                Queuer.add("R2", Target);
                Queuer.add("Q");
                return;
            }

            //kyzer 3rd q combo
            if (MenuHandler.getMenuBool("kyzerCombo") && Target.IsValidTarget(400 + 325 + (SH.WRange / 2)) &&
                SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() &&
                SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && CH.QCount == 2 &&
                Queuer.Queue.Count == 0)
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
                return;
            }

            // Shy combo
            if (MenuHandler.getMenuBool("shyCombo") && Target.IsValidTarget(400 + 325 + (SH.WRange / 2)) &&
                SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() &&
                SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && Queuer.Queue.Count == 0)
            {
                Queuer.add("E", Target.Position);
                Queuer.add("R");
                Queuer.add("Flash", Target.Position, true);
                Queuer.add("AA");
                Queuer.add("Hydra");
                Queuer.add("W");
                Queuer.add("R2", Target);
                Queuer.add("Q");
                return;
            }

            mainCombo();
        }

        public static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (SH._spells[SpellSlot.E].IsReady() && CH.LastQ + 250 < Environment.TickCount &&
                MenuHandler.getMenuBool("EFlee"))
            {
                SH.CastE(Game.CursorPos);
            }

            if ((SH._spells[SpellSlot.Q].IsReady() && CH.LastE + 250 < Environment.TickCount &&
                 MenuHandler.getMenuBool("QFlee")))
            {
                if ((MenuHandler.Config.Item("Ward Mechanic").GetValue<bool>() && CheckHandler.QCount == 2))
                {
                    return;
                }
                SH.CastQ();
            }
        }
    }
}