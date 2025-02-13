namespace Server.Mobiles
{
	[CorpseName( "a mountain goat corpse" )]
	public class MountainGoat : BaseCreature
	{
		[Constructable]
		public MountainGoat() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			//Name = "Mountain Goat";
            switch (Utility.Random(3))
            {
                case 0: Name = "Mountain goat"; break;
                case 1: Name = "Cashmire goat"; break;
                case 2: Name = "Dwarf goat"; break;
            }
			Body = 88;
			BaseSoundID = 0x99;

			SetStr( 22, 64 );
			SetDex( 56, 75 );
			SetInt( 16, 30 );

			SetHits( 20, 33 );
			SetMana( 0 );

			SetDamage( 3, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 20 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.MagicResist, 25.1, 30.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.Wrestling, 29.3, 44.0 );

			Fame = 300;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -0.9;
		}

		public override int Meat => 2;
        public override int Hides => 3;
        public override FoodType FavoriteFood => FoodType.GrainsAndHay | FoodType.FruitsAndVegies;

        public MountainGoat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}