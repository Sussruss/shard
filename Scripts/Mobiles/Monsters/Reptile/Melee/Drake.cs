using Server.Items;
namespace Server.Mobiles
{
	[CorpseName( "a drake corpse" )]
	public class Drake : BaseCreature
	{
		[Constructable]
		public Drake () : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Drake";
			Body = Utility.RandomList( 60, 61 );
			BaseSoundID = 362;

			SetStr( 401, 430 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );

			SetHits( 281, 298 );

			SetDamage( 12, 17 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 80.0 );

			Fame = 5500;

			VirtualArmor = 46;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 84.3;

			PackReg( 3 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
            PackGold(500);
            AddLoot( LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 1);
            if (Utility.RandomDouble() <= 0.05)
                AddItem(new RandomAccWeap(Utility.RandomMinMax(1,2)));
		}

		public override bool ReacquireOnMovement => true;
        public override bool HasBreath => true; // fire breath enabled
		public override int TreasureMapLevel => 2;
        public override int Meat => 10;
        public override int Hides => 15;

        public override HideType HideType
        {
            get
            {
                double roll = Utility.RandomDouble();

                if (roll <= 0.02)
                    return HideType.Barbed;
                if (roll <= 0.1)
                    return HideType.Horned;
                if (roll <= 0.25)
                    return HideType.Spined;

                return HideType.Regular;
            }
        }
		public override int Scales => 2;
        public override ScaleType ScaleType => ( Body == 60 ? ScaleType.Yellow : ScaleType.Red );
        public override FoodType FavoriteFood => FoodType.Meat | FoodType.Fish;

        public Drake( Serial serial ) : base( serial )
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