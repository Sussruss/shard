namespace Server.Items
{
	public class ArtsGlasses : ElvenGlasses
	{
		public override int LabelNumber => 1073363; //Reading Glasses of the Arts

		public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 10;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
		public ArtsGlasses()
		{
			Attributes.BonusStr = 5;
			Attributes.BonusInt = 5;
			Attributes.BonusHits = 15;
		}
		public ArtsGlasses( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
