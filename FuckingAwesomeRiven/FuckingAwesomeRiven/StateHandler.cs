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
    public class comboCheck
    {
        public comboCheck(int ResetDelay = 1000)
        {
            resetDelay = ResetDelay;
            resetTick = 0;
        }

        public void setTick()
        {
            if (resetTick == 0)
            {
                resetTick = Environment.TickCount;
            }
        }

        public bool state = false;
        public int resetTick = 0;
        public int resetDelay = 1000;
    }
    class StateHandler
    {
        public static List<comboCheck> resetChecks = new List<comboCheck>();

        public static Obj_AI_Hero Target;

        public static Obj_AI_Hero Player;

        public static void tick()
        {
            Target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            Player = ObjectManager.Player;


            foreach (var a in resetChecks)
            {
                if (!a.state && a.resetTick + a.resetDelay <= Environment.TickCount)
                {
                    a.resetTick = 0;
                    a.state = false;
                }
            }
        }

        public static void lastHit()
        {
            var minion = MinionManager.GetMinions(Player.Position, SH.QRange).FirstOrDefault();

            SH.animCancel(minion);

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

            if (minion == null) return;

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

            if (CH.LastTiamatCancel < Environment.TickCount)
            {
                CH.LastTiamatCancel = int.MaxValue;
                SH.castItems(Target);
            }

            SH.Orbwalk(minion);

            SH.animCancel(minion);

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

        public static comboCheck startedRCombo = new comboCheck();
        public static comboCheck startedR2Combo = new comboCheck();

        public static void mainCombo()
        {

            SH.Orbwalk(Target);

            

            if (Target == null)
            {
                startedR2Combo.state = false;
                startedRCombo.state = false;
                return;
            }

            var comboRDmg = DamageHandler.getComboDmg(true, Target);
            var comboNoR = DamageHandler.getComboDmg(false, Target);

            if (CH.LastECancelSpell < Environment.TickCount && MenuHandler.getMenuBool("CE"))
            {
                CH.LastECancelSpell = int.MaxValue;
                SH.CastE(Target.Position);
            }
            if (CH.LastTiamatCancel < Environment.TickCount)
            {
                CH.LastTiamatCancel = int.MaxValue;
               SH.castItems(Target);
            }

            if (MenuHandler.getMenuBool("CR"))
            {
                if (SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() &&
                    SH._spells[SpellSlot.Q].IsReady() && comboNoR < Target.Health && comboRDmg > Target.Health ||
                    startedRCombo.state)
                {
                    startedRCombo.state = true;
                    SH.CastE(Target.Position);
                    if (Environment.TickCount - CH.LastE >= 100)
                    {
                        if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                        {
                            ItemData.Tiamat_Melee_Only.GetItem().Cast();
                            CH.LastTiamat = Environment.TickCount;
                        }
                        if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                        {
                            ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                            CH.LastTiamat = Environment.TickCount;
                        }
                        CH.LastTiamat = Environment.TickCount;
                    }
                    if (Environment.TickCount - CH.LastTiamat >= 100)
                    {
                        SH.CastR();
                        CH.LastFr = Environment.TickCount;
                    }
                    if (Environment.TickCount - CH.LastFr >= 100)
                    {
                        SH.CastQ(Target);
                        CH.LastQ = Environment.TickCount;
                        startedRCombo.state = false;
                    }
                    return;
                }

                if (comboNoR < Target.Health && comboRDmg > Target.Health && SH._spells[SpellSlot.R].IsReady() && SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.W].IsReady())
                {
                    if (SH._spells[SpellSlot.E].IsReady())
                    {
                        SH.CastE(Target.Position);
                    }
                    if 
                        (SH._spells[SpellSlot.R].IsReady() && Environment.TickCount - CH.LastE >= 200)
                    {
                        SH.CastR();
                    }
                }
            }

            if (SH._spells[SpellSlot.R].IsReady() && CH.RState && MenuHandler.getMenuBool("CR2") && SH._spells[SpellSlot.R].GetDamage(Target) + (SH._spells[SpellSlot.Q].IsReady() && Target.IsValidTarget(SH.QRange) ? SH._spells[SpellSlot.Q].GetDamage(Target) : 0) > Target.Health || startedR2Combo.state)
            {
                startedR2Combo.state = true;
                if (CH.RState)
                {
                    SH.CastR2(Target);
                    if (!SH._spells[SpellSlot.Q].IsReady() || !Target.IsValidTarget(SH.QRange))
                    {
                        startedR2Combo.state = false;
                    }
                }
                if (SH._spells[SpellSlot.Q].IsReady() && Environment.TickCount - CH.LastR2 >= 100)
                {
                    SH.CastQ(Target);
                    startedR2Combo.state = false;
                }
                return;
            }

            if (CH.RState)
            {

                if (MenuHandler.getMenuBool("QWR2KS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.Q].IsReady() &&
                    SH._spells[SpellSlot.Q].GetDamage(Target) + (CH.RState && SH._spells[SpellSlot.R].IsReady() ? SH._spells[SpellSlot.R].GetDamage(Target) : 0) >
                    Target.Health)
                {
                    SH.CastQ(Target);
                    Utility.DelayAction.Add(250, () => SH.CastW());
                    Utility.DelayAction.Add(515, () => SH.CastR2(Target));
                }
                else if (MenuHandler.getMenuBool("QR2KS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.Q].IsReady() &&
                         SH._spells[SpellSlot.Q].GetDamage(Target) + (CH.RState && SH._spells[SpellSlot.R].IsReady() ? SH._spells[SpellSlot.R].GetDamage(Target) : 0) > Target.Health)
                {
                    SH.CastQ(Target);
                    Utility.DelayAction.Add(515, () => SH.CastR2(Target));
                }
                else if (MenuHandler.getMenuBool("WR2KS") && Target.IsValidTarget(SH.WRange) && CH.CanW && SH._spells[SpellSlot.W].IsReady() &&
                         SH._spells[SpellSlot.W].GetDamage(Target) + (CH.RState && SH._spells[SpellSlot.R].IsReady() ? SH._spells[SpellSlot.R].GetDamage(Target) : 0) > Target.Health)
                {
                    SH.CastW();
                    Utility.DelayAction.Add(515, () => SH.CastR2(Target));
                }
            }
            else
            {
                if (MenuHandler.getMenuBool("QWKS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.Q].GetDamage(Target) + SH._spells[SpellSlot.W].GetDamage(Target) > Target.Health)
                {
                    SH.CastQ(Target);
                    Utility.DelayAction.Add(250, () => SH.CastW());
                }
                else if (MenuHandler.getMenuBool("QKS") && Target.IsValidTarget(SH.QRange) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.Q].GetDamage(Target) > Target.Health)
                {
                    SH.CastQ(Target);
                }
                else if (MenuHandler.getMenuBool("WKS") && Target.IsValidTarget(SH.WRange) && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.W].GetDamage(Target) > Target.Health)
                {
                    SH.CastW();
                }
            }

            var BonusRange = Orbwalking.GetRealAutoAttackRange(Player) + (Target.BoundingRadius / 2) - 50;

            SH.animCancel(Target);

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
                if (Target.IsValidTarget(SH.QRange) && CH.CanQ)
                {
                    SH.CastQ(Target);
                    return;
                }
                if (!Target.IsValidTarget(BonusRange + 50) && Target.IsValidTarget(SH.QRange) && CH.CanQ && MenuHandler.getMenuBool("UseQ-GC"))
                {
                    SH.CastQ(Target);
                }
            }
        }

        public static comboCheck startJump1 = new comboCheck();
        public static comboCheck startJump2 = new comboCheck();
        public static bool castedFlash = false;
        public static bool castedTia;

        public static void burstCombo()
        {
            SH.Orbwalk(Target);

            if (!Target.IsValidTarget()) return;

            Queuer.doQueue();

            //kyzer 3rd q combo
            if (Target.IsValidTarget(600) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && CH.QCount == 2 && Queuer.Queue.Count == 0)
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
            if (Target.IsValidTarget(600) && SH._spells[SpellSlot.Q].IsReady() && SH._spells[SpellSlot.W].IsReady() && SH._spells[SpellSlot.E].IsReady() && SH._spells[SpellSlot.R].IsReady() && Queuer.Queue.Count == 0)
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

            if (SH._spells[SpellSlot.E].IsReady() && Environment.TickCount - CH.LastQ >= 250 && MenuHandler.getMenuBool("EFlee"))
            {
                SH.CastE(Game.CursorPos);
            }

            if (SH._spells[SpellSlot.Q].IsReady() && Environment.TickCount - CH.LastE >= 250 && MenuHandler.getMenuBool("QFlee"))
            {
                SH.CastQ();
            }
        }
    }
}
