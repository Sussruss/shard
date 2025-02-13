using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBChainmailArmor: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(ChainCoif), 17, Utility.RandomMinMax(15, 25), 0x13BB, 0));
                Add(new GenericBuyInfo(typeof(ChainChest), 143, Utility.RandomMinMax(15, 25), 0x13BF, 0));
                Add(new GenericBuyInfo(typeof(ChainLegs), 149, Utility.RandomMinMax(15, 25), 0x13BE, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( ChainCoif ), 6 );
				Add( typeof( ChainChest ), 12 );
				Add( typeof( ChainLegs ), 13 );
			}
		}
	}
}
