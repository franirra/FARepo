using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace FuckingAwesomeThresh
{
    class ComboStateHandler
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static void Combo()
        {
            if (CheckHandler.GetQUnit() != null &&
                CheckHandler.GetQUnit().GetBuffData(CheckHandler.ThreshQBuff).EndTime - 100 > Environment.TickCount)
                return;

            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget() || target.HasAntiCc())
                return;

            if (CheckHandler.Spells[SpellSlot.R].IsReady() &&
                target.IsValidTarget(CheckHandler.Spells[SpellSlot.R].Range) && Player.Position.GetEnemiesInRange(CheckHandler.Spells[SpellSlot.R].Range).Count >= 2)
            {
                CheckHandler.Spells[SpellSlot.R].Cast();
            }

            if (CheckHandler.Spells[SpellSlot.E].IsReady() &&
                target.IsValidTarget(CheckHandler.Spells[SpellSlot.E].Range))
            {
                var pos = target.Position.Extend(Player.Position, Player.Distance(target) + 200);
                CheckHandler.Spells[SpellSlot.E].Cast(pos);
            }

            if (CheckHandler.Spells[SpellSlot.Q].IsReady() &&
                target.IsValidTarget(CheckHandler.Spells[SpellSlot.Q].Range))
            {
                CheckHandler.Spells[SpellSlot.Q].Cast(target);
            }

        }
    }
}
