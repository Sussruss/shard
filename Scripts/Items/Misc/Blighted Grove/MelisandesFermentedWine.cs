namespace Server.Items
{
	public class MelisandesFermentedWine : GreaterExplosionPotion
	{
		public override int LabelNumber => 1072114; // Melisande's Fermented Wine

		[Constructable]
		public MelisandesFermentedWine()
		{
			Stackable = false;
			ItemID = 0x99B;
			Hue = Utility.RandomList( 0xB, 0xF, 0x48D ); // TODO update
		}

		public MelisandesFermentedWine( Serial serial ) : base( serial )
		{
		}

		public override void Drink( Mobile from )
		{
			if ( MondainsLegacy.CheckML( from ) )
				base.Drink( from );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1074502 ); // It looks explosive.
			list.Add( 1075085 ); // Requirement: Mondain's Legacy
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


