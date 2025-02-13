namespace Server.Items
{
	public class StandardPlateKabuto : BaseArmor
	{
		public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 2;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 60;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 70;
        public override int OldStrReq => 70;

        public override int ArmorBase => 3;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

        [Constructable]
		public StandardPlateKabuto() : base( 0x2789 )
		{
			Weight = 6.0;
		}

		public StandardPlateKabuto( Serial serial ) : base( serial )
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
		}
	}
}