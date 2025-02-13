using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a glowing orc corpse" )]
	public class OrcishMage : BaseCreature
	{
		public override InhumanSpeech SpeechType => InhumanSpeech.Orc;

        [Constructable]
		public OrcishMage () : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Orcish Mage";
			Body = 140;
			BaseSoundID = 0x45A;

			SetStr( 116, 150 );
			SetDex( 91, 115 );
			SetInt( 161, 180 );

			SetHits( 70, 90 );

			SetDamage( 4, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 60.1, 69.5 );
			SetSkill( SkillName.Magery, 60.1, 69.5 );
			SetSkill( SkillName.MagicResist, 60.1, 75.0 );
			SetSkill( SkillName.Tactics, 50.1, 65.0 );
			SetSkill( SkillName.Wrestling, 40.1, 50.0 );

			Fame = 3000;

			VirtualArmor = 30;


			PackReg( 6 );

			if ( 0.05 > Utility.RandomDouble() )
				PackItem( new OrcishKinMask() );

            if (Utility.RandomDouble() <= 0.7)
            {
                Spellbook book = new Spellbook();
                book.Content = ulong.MaxValue;
                book.LootType = LootType.Regular;
                AddItem(book);
            }

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls );
		}

		public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 1;
        public override int Meat => 1;

        public override OppositionGroup OppositionGroup => OppositionGroup.SavagesAndOrcs;

        public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
				return false;

			return base.IsEnemy( m );
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is OrcishKinMask )
			{
				AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x307 );
			}
		}

		public OrcishMage( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
