namespace Server.Items
{
	[Flipable( 0x13cb, 0x13d2 )]
	public class DragonScalemailLegs : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 60;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 20;
        public override int OldStrReq => 10;

        public override int ArmorBase => 46;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public DragonScalemailLegs() : base( 0x13CB )
		{
			Weight = 4.0;
            Name = "Dragon Scalemail Leggings";
            Hue = 2534;
            BaseArmorRating = 48;
		}

		public DragonScalemailLegs( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (BaseArmorRating == 46)
                BaseArmorRating = 48;
		}
	}
}