using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class Butcher : BaseVendor 
	{ 
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
		public Butcher() : base( "the butcher" ) 
		{ 
			SetSkill( SkillName.Anatomy, 45.0, 68.0 );
		} 

		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBButcher() ); 
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new HalfApron() );
			AddItem( new Cleaver() );
		}

		public Butcher( Serial serial ) : base( serial ) 
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