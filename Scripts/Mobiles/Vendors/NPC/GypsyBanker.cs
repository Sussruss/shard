using Server.Items;

namespace Server.Mobiles
{
	public class GypsyBanker : Banker
	{
		public override bool IsActiveVendor => false;
        public override NpcGuild NpcGuild => NpcGuild.None;
        public override bool ClickTitle => false;

        [Constructable]
		public GypsyBanker()
		{
			Title = "the gypsy banker";
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new JesterHat( RandomBrightHue() ) ); break;
				case 1: AddItem( new Bandana( RandomBrightHue() ) ); break;
				case 2: AddItem( new SkullCap( RandomBrightHue() ) ); break;
			}

			Item item = FindItemOnLayer( Layer.Pants );

			if ( item != null )
				item.Hue = RandomBrightHue();

			item = FindItemOnLayer( Layer.Shoes );

			if ( item != null )
				item.Hue = RandomBrightHue();

			item = FindItemOnLayer( Layer.OuterLegs );

			if ( item != null )
				item.Hue = RandomBrightHue();

			item = FindItemOnLayer( Layer.InnerLegs );

			if ( item != null )
				item.Hue = RandomBrightHue();

			item = FindItemOnLayer( Layer.OuterTorso );

			if ( item != null )
				item.Hue = RandomBrightHue();

			item = FindItemOnLayer( Layer.InnerTorso );

			if ( item != null )
				item.Hue = RandomBrightHue();

			item = FindItemOnLayer( Layer.Shirt );

			if ( item != null )
				item.Hue = RandomBrightHue();
		}

		public GypsyBanker( Serial serial ) : base( serial )
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
		}
	}
}