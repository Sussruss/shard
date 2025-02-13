namespace Server.Items
{
    public class GreaterHealPotion : BaseHealPotion
	{
		public override int MinHeal => 35;

        public override int MaxHeal => 50;
        //public override double Delay{ get{ return 13.0; } }

		[Constructable]
		public GreaterHealPotion() :this(1)
		{
		}

        [Constructable]
        public GreaterHealPotion(int amount) : base(PotionEffect.HealGreater)
        {
            Name = "Greater Heal Potion";
            Amount = amount;
        }

		public GreaterHealPotion( Serial serial ) : base( serial )
		{
		}

        public override double PotionDelay => 13.0;

        /*
        //Maka - unstacking pots on death, stacking on resurrection
        public override void OnAdded(object parent)
        {
            int amount = Amount;
            if (amount != 1 && parent is Corpse)
            {
                Corpse corpse = (Corpse)parent;
                if (corpse.Owner is PlayerMobile)
                {
                    Delete();
                    for (int i = 0; i < amount; i++)
                        corpse.DropItem(new GreaterHealPotion());
                }
                else
                    base.OnAdded(parent);
            }
            else
                base.OnAdded(parent);
        }
        */
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