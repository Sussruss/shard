namespace Server.Mobiles
{
	[CorpseName( "an ice fiend corpse" )]
	public class IceFiend : BaseCreature
	{
		[Constructable]
		public IceFiend () : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Ice Fiend";
			Body = 43;
			BaseSoundID = 357;

			SetStr( 376, 405 );
			SetDex( 176, 195 );
			SetInt( 201, 225 );

			SetHits( 226, 243 );

			SetDamage( 8, 19 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 75.1, 85.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			Fame = 18000;
			Karma = -3000;

			VirtualArmor = 60;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
            PackGold(400, 550);
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int TreasureMapLevel => 4;
        public override int Meat => 1;

        public IceFiend( Serial serial ) : base( serial )
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