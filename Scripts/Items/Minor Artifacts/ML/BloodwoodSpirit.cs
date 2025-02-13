namespace Server.Items
{
	public class BloodwoodSpirit : BaseTalisman
	{
		public override int LabelNumber => 1075034; // Bloodwood Spirit
		public override bool ForceShowName => true;

        [Constructable]
		public BloodwoodSpirit() : base( 0x2F5A )
		{
			Hue = 0x27;
			MaxChargeTime = 1200;

			Removal = TalismanRemoval.Damage;
			Blessed = GetRandomBlessed();
			Protection = GetRandomProtection();

			SkillBonuses.SetValues( 0, SkillName.SpiritSpeak, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Necromancy, 5.0 );
		}

		public BloodwoodSpirit( Serial serial ) :  base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
