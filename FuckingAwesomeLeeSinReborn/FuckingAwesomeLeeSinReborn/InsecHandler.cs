using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace FuckingAwesomeLeeSinReborn
{
    class InsecHandler
    {
        private static Obj_AI_Base _selectedUnit;
        private static Obj_AI_Base _selectedEnemy;
        public static bool flashR;
        public static Vector3 flashPos;
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static void OnClick(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
                return;
            var unit2 = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(a => (a.IsValid<Obj_AI_Hero>()) && a.IsEnemy && a.Distance(Game.CursorPos) < a.BoundingRadius + 80 && a.IsValidTarget());
            if (unit2 != null)
            {
                _selectedEnemy = unit2;
                return;
            }
            var unit = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(a => (a.IsValid<Obj_AI_Hero>() || a.IsValid<Obj_AI_Minion>() || a.IsValid<Obj_AI_Turret>()) && a.IsAlly && a.Distance(Game.CursorPos) < a.BoundingRadius + 80 && a.IsValid && !a.IsDead && !a.Name.ToLower().Contains("ward") && !a.IsMe);
            _selectedUnit = unit;
            if (_selectedUnit == null)
                _selectedEnemy = null;
        }

        public static void Draw()
        {
            if (_selectedUnit != null)
            {
                Render.Circle.DrawCircle(_selectedUnit.Position, _selectedUnit.BoundingRadius + 50, Color.White);
            }
            if (_selectedEnemy.IsValidTarget() && _selectedEnemy.IsVisible && !_selectedEnemy.IsDead)
            {
                Render.Circle.DrawCircle(_selectedEnemy.Position, _selectedEnemy.BoundingRadius + 50, Color.SteelBlue);
                Drawing.DrawText(Drawing.WorldToScreen(_selectedEnemy.Position).X - 40, Drawing.WorldToScreen(_selectedEnemy.Position).Y + 10, Color.White, "Insec Target");
                if (InsecPos().IsValid())
                {
                    Render.Circle.DrawCircle(InsecPos(), 110, Color.SteelBlue);
                }
            }
        }

        private static Vector3 InsecPos()
        {
            if (_selectedUnit != null && _selectedEnemy.IsValidTarget() && Program.Config.Item("clickInsec").GetValue<bool>())
            {
                return _selectedUnit.Position.Extend(_selectedEnemy.Position, _selectedUnit.Distance(_selectedEnemy) + 250);
            }
            if (_selectedUnit == null && _selectedEnemy.IsValidTarget() && Program.Config.Item("mouseInsec").GetValue<bool>())
            {
                return Game.CursorPos.Extend(_selectedEnemy.Position, Game.CursorPos.Distance(_selectedEnemy.Position) + 250);
            }
            return new Vector3();
        }

        public static void DoInsec()
        {
            if (_selectedEnemy == null && Program.Config.Item("insecOrbwalk").GetValue<bool>())
            {
                Orbwalking.Orbwalk(null, Game.CursorPos);
                return;
            }
            if (_selectedEnemy != null)
            {
                Orbwalking.Orbwalk(
                    Orbwalking.InAutoAttackRange(_selectedEnemy) ? _selectedEnemy : null,
                    _selectedUnit == null ? _selectedEnemy.Position : Game.CursorPos);
            }
            if (!InsecPos().IsValid() || !_selectedEnemy.IsValidTarget() || !CheckHandler.spells[SpellSlot.R].IsReady())
                    return;
                if (Player.Distance(InsecPos()) < 120)
                {
                    CheckHandler.spells[SpellSlot.R].CastOnUnit(_selectedEnemy);
                    return;
                }
                if (Player.Distance(InsecPos()) < 600)
                {
                    if (CheckHandler.WState && CheckHandler.spells[SpellSlot.W].IsReady())
                    {
                        WardjumpHandler.Jump(InsecPos(), false, false, true);
                        return;
                    }
                    if (!Program.Config.Item("flashInsec").GetValue<bool>() || CheckHandler.WState && CheckHandler.spells[SpellSlot.W].IsReady() && Items.GetWardSlot() != null) return;
                    if (_selectedEnemy.Distance(Player) < CheckHandler.spells[SpellSlot.R].Range)
                    {
                        CheckHandler.spells[SpellSlot.R].CastOnUnit(_selectedEnemy);
                        flashPos = InsecPos();
                        flashR = true;
                    }
                    else
                    {
                        if (InsecPos().Distance(Player.Position) < 400)
                        {
                            Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerflash"), InsecPos());
                        }
                    }
                }
                if (Player.Distance(_selectedEnemy) < CheckHandler.spells[SpellSlot.Q].Range && CheckHandler.QState && CheckHandler.spells[SpellSlot.Q].IsReady())
                {
                    CheckHandler.spells[SpellSlot.Q].Cast(_selectedEnemy);
                }
                if (!CheckHandler.QState && _selectedEnemy.HasQBuff() || (Program.Config.Item("q2InsecRange").GetValue<bool>() && CheckHandler.BuffedEnemy.IsValidTarget() && CheckHandler.BuffedEnemy.Distance(InsecPos()) < 500))
                {
                    CheckHandler.spells[SpellSlot.Q].Cast();
                }
            
            if (Program.Config.Item("q1InsecRange").GetValue<bool>() || !CheckHandler.QState || !CheckHandler.spells[SpellSlot.Q].IsReady() || !InsecPos().IsValid())
            {
                return;
            }
            foreach (
                var unit in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            a =>
                                a.IsEnemy && (a.IsValid<Obj_AI_Hero>() || a.IsValid<Obj_AI_Minion>()) &&
                                a.Distance(InsecPos()) < 400))
            {
                if (!unit.IsValidTarget())
                    return;
                CheckHandler.spells[SpellSlot.Q].Cast(unit);
            }
        }
    }
}
