using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class SandVortexStatue : Item, IRewardItem
	{
        private bool m_IsRewardItem = true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get => m_IsRewardItem;
            set => m_IsRewardItem = value;
        }
		[Constructable]
		public SandVortexStatue() : base( 0x2616 )
		{
			Weight = 1.0;
		}

        public SandVortexStatue(Serial serial) : base(serial) { }

		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( 0 ); }

		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}