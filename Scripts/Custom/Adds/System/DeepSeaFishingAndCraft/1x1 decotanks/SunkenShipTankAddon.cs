namespace Server.Items
{
	public class SunkenShipTankAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new SunkenShipTankAddonDeed();

        [ Constructable ]
		public SunkenShipTankAddon()
		{
			AddonComponent ac = null;

//Tank
			//Black on bottom of tank
			ac = new AddonComponent( 5990 );
			ac.Hue = 1;
			ac.Name = "fishtank base";
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 5992 );
			ac.Hue = 1;
			ac.Name = "fishtank base";
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 1;
			ac.Name = "fishtank base";
			AddComponent( ac, 0, 0, 1 );
			ac = new AddonComponent( 5992 );
			ac.Hue = 1;
			ac.Name = "fishtank base";
			AddComponent( ac, 0, 0, 1 );

			//Shades of blue
			ac = new AddonComponent( 5990 );
			ac.Hue = 92;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 2 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 92;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 2 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 92;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 3 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 92;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 3 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 93;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 4 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 93;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 4 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 93;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 5 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 93;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 5 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 94;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 6 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 94;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 6 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 94;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 7 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 94;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 7 );
			ac = new AddonComponent( 5990 );
			ac.Hue = 95;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 8 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 95;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 8 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 95;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 9 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 95;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 9 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 96;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 10 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 96;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 10 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 96;
			ac.Name = "water";
			AddComponent( ac, 0, 0, 11 );

			//Black on top of tank
			ac = new AddonComponent( 4846 );
			ac.Hue = 1;
			ac.Name = "fishtank lid";
			AddComponent( ac, 0, 0, 13 );
			ac = new AddonComponent( 4846 );
			ac.Hue = 1;
			ac.Name = "fishtank lid";
			AddComponent( ac, 0, 0, 14 );


			//Sand
			ac = new AddonComponent( 4846 );
			ac.Hue = 348;
			ac.Name = "sand";
			AddComponent( ac, 1, 1, 12 );

			//Plant and fish
			ac = new AddonComponent( 5363 );
			ac.Name = "H.M.S Destiny";
			ac.Hue = 1162;
			AddComponent( ac, 1, 1, 12 );

		}

		public SunkenShipTankAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SunkenShipTankAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new SunkenShipTankAddon();

        [Constructable]
		public SunkenShipTankAddonDeed()
		{
			Name = "a Sunken Ship Tank";
		}

		public SunkenShipTankAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}