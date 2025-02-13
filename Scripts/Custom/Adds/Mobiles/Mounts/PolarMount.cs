//Ridable/Tamable Polar Bear 
//re-wrote by EvilFreak
// Version 2.0

namespace Server.Mobiles
{
	[CorpseName( "a polar bear corpse" )]
	public class PolarBearMount : BaseMount
	{
		[Constructable]
		public PolarBearMount() : this( "Polar Bear" )
		{
			Hue = 1153;
		}

		[Constructable]
		public PolarBearMount( string name ) : base( name, 0xD5 , 16069 , AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xA3;

	//stats are high for my server adjust at will

			SetStr( 210, 300 );
			SetDex( 75, 120 );
			SetInt( 20, 40 );

			SetHits( 100, 270 );
			SetMana( 0 );

			SetDamage( 2, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 19.2, 29.0 );
			SetSkill( SkillName.Wrestling, 19.2, 29.0 );

			Fame = 300;
			Karma = 0;

	//might wanna adjust these for your server

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 100.0;
		}

	//this polar does not eat fish you can adjust this as well

		public override int Meat => 1;
        public override int Hides => 12;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;

        public PolarBearMount( Serial serial ) : base( serial )
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