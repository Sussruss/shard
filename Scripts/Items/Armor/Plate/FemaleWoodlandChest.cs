namespace Server.Items
{
	//Is this a filler-type item? the clilocs don't match up and at a glacnce I can't find direct reference of it
	[Flipable( 0x2B6D, 0x3164 )]
	public class FemaleElvenPlateChest : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;

        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 95;
        public override int OldStrReq => 95;

        public override bool AllowMaleWearer => true;

        public override int ArmorBase => 30;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

        [Constructable]
		public FemaleElvenPlateChest() : base( 0x2B6D )
		{
			Weight = 8.0;
		}

		public FemaleElvenPlateChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}