using System;
using Server.Mobiles;

namespace Server.Spells.Spellweaving
{
	public class SummonFeySpell : ArcaneSummon<ArcaneFey>
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Summon Fey", "Alalithra",
				-1
			);

		public override TimeSpan CastDelayBase => TimeSpan.FromSeconds( 1.5 );

        public override double RequiredSkill => 38.0;
        public override int RequiredMana => 10;

        public SummonFeySpell( Mobile caster, Item scroll )
			: base( caster, scroll, m_Info )
		{
		}

		public override int Sound => 0x217;
    }
}