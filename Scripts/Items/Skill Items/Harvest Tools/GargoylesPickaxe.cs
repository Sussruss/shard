using Server.Engines.Harvest;

namespace Server.Items
{
	public class GargoylesPickaxe : BaseAxe
	{
		public override int LabelNumber => 1041281; // a gargoyle's pickaxe
		public override HarvestSystem HarvestSystem => Mining.System;

        ////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq => 50;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 35;

        public override int OldStrengthReq => 25;
        public override int OldMinDamage => 1;
        public override int OldMaxDamage => 15;
        public override int OldSpeed => 35;
        public override float MlSpeed => 3.00f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

        [Constructable]
        public GargoylesPickaxe() : this(Utility.RandomMinMax(101, 125))
        {
		}

		[Constructable]
        public GargoylesPickaxe(int uses) : base(0xE85 + Utility.Random(2))
        {
			Weight = 11.0;
			UsesRemaining = uses;
			ShowUsesRemaining = true;
		}

		public GargoylesPickaxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (Hue == 0x973)
                Hue = 0x0;
		}
	}
}