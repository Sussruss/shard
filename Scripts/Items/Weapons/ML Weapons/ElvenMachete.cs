namespace Server.Items
{
	[FlipableAttribute( 0x2D35, 0x2D29 )]
	public class ElvenMachete : BaseSword
	{
		public override WeaponAbility PrimaryAbility => WeaponAbility.DefenseMastery;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Bladeweave;

        public override int AosStrengthReq => 20;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 41;
        public override float MlSpeed => 2.75f;

        public override int OldStrengthReq => 20;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 15;
        public override int OldSpeed => 41;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x239;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;

        [Constructable]
		public ElvenMachete() : base( 0x2D35 )
		{
			Weight = 6.0;
		}

		public ElvenMachete( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}