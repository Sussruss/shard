namespace Server.Items
{
	public class MagicArrowScroll : SpellScroll
	{
        public override int ManaCost => 4; //Loki edit

		[Constructable]
		public MagicArrowScroll() : this( 1 )
		{
		}

		[Constructable]
		public MagicArrowScroll( int amount ) : base( 4, 0x1F32, amount )
		{
		}
		
		public MagicArrowScroll( Serial serial ) : base( serial )
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