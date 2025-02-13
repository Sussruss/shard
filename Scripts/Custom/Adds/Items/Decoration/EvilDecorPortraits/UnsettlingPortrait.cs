//Ber 2006

using System;
using Server.Network;

namespace Server.Items
{
    public class UnsettlingPortrait : AddonComponent
	{
		//private InternalTimer m_Timer;
		public static TimeSpan AnimDelay = TimeSpan.FromSeconds( 1.0 ); //the delay between animation is 1 seconds
		public DateTime m_NextAnim;

		[Constructable]
		public UnsettlingPortrait() : base( 10853 )
		{
			Name = "Unsettling Portrait";
			Movable = true;
		}

		public UnsettlingPortrait( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( m.InRange( this, 3 ) ) 
			{
				switch ( ItemID ) 
				{ 
					//do swap or animation here 
					case 10853: //1
						ItemID=10854;
						m.PrivateOverheadMessage( MessageType.Regular, 1153, false, "What was that?", m.NetState ); 
						break;
					case 10854: //2
						ItemID=10853; 
						break;
					default: break; 
				}
			}
			else
			{
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that
			}
		}

		public override bool HandlesOnMovement => true;

        public override void OnMovement( Mobile m, Point3D oldLocation ) 
		{ 
			if ( DateTime.UtcNow >= m_NextAnim && m.InRange( this, 4 ) ) // check if it's time to animate & mobile in range & in los.
			{
				m_NextAnim = DateTime.UtcNow + AnimDelay; // set next animation time

				switch ( ItemID ) 
				{ 
					//do swap or animation here 
					case 10853: //1
						ItemID=10854;
						new InternalTimer( this, m ).Start();  
						break;
					case 10854: //2
						ItemID=10853; 
						break;
					default: break; 
				}
			}
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 );
		}
        
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}

		public class InternalTimer : Timer
		{
			private int m_Count = 3;
			private readonly UnsettlingPortrait m_UnsettlingPortrait;
			private readonly Mobile m_From;
	
			public InternalTimer( UnsettlingPortrait unsettlingportrait, Mobile from ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_UnsettlingPortrait = unsettlingportrait;
				m_From = from;
			}
// added
			protected override void OnTick() 
			{
				m_Count--;

				if ( m_Count == ( 2 ) )
				{
					m_UnsettlingPortrait.ItemID=10853;
				}
				if ( m_Count == ( 1 ) )
				{
					m_UnsettlingPortrait.ItemID=10854; 
				}
				if ( m_Count == 0 )
				{
					Stop();
				}
				if ( m_From.NetState == null )
				{
					Stop();
				}
			}
//end add		
			
		}
	}
			
	public class UnsettlingPortraitAddon : BaseAddon
	{
	    public override BaseAddonDeed Deed => new UnsettlingPortraitDeed();

        [Constructable]
		public UnsettlingPortraitAddon()
		{
		  Name = "Unsettling Portrait South";
			Weight = 2.0;
			
			AddComponent( new UnsettlingPortrait(), 0, 0, 0 );
		}

		public UnsettlingPortraitAddon( Serial serial ) : base( serial )
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

	public class UnsettlingPortraitDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new UnsettlingPortraitAddon();

        [Constructable]
		public UnsettlingPortraitDeed()
		{
		    Name = "Unsettling Portrait South Deed";
		}

		public UnsettlingPortraitDeed( Serial serial ) : base( serial )
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