using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class TitanStatue : Item, IRewardItem
    {
        private bool m_IsRewardItem = true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get => m_IsRewardItem;
            set => m_IsRewardItem = value;
        }
		[Constructable]
		public TitanStatue() : base( 0x25CD )
		{
			Weight = 1.0;
		}

        public TitanStatue(Serial serial) : base(serial) { }

		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( 0 ); }

		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}