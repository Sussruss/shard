using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a treefellow corpse" )]
	public class Treefellow : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.Dismount;
		}

		[Constructable]
		public Treefellow() : base( AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "Treefellow";
			Body = 301;

			SetStr( 196, 220 );
			SetDex( 31, 55 );
			SetInt( 66, 90 );

			SetHits( 148, 182 );

			SetDamage( 13, 19 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 25 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 30, 35 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 40.1, 55.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 85.0 );

			Fame = 500;
			Karma = 1500;

			VirtualArmor = 24;
			PackItem( new Log( Utility.RandomMinMax( 13, 34 ) ) );
		}

		public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override int GetIdleSound()
		{
			return 443;
		}

		public override int GetDeathSound()
		{
			return 31;
		}

		public override int GetAttackSound()
		{
			return 672;
		}

		public override bool BleedImmune => true;

        public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
            PackGold(20, 100);
		}

		public Treefellow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( BaseSoundID == 442 )
				BaseSoundID = -1;
		}
	}
}