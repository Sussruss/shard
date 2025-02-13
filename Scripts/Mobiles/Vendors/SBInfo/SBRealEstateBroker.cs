using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBRealEstateBroker : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(BlankScroll), 5, Utility.RandomMinMax(15, 25), 0x0E34, 0));
                Add(new GenericBuyInfo(typeof(ScribesPen), 8, Utility.RandomMinMax(15, 25), 0xFBF, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( ScribesPen ), 4 );
				Add( typeof( BlankScroll ), 2 );
			}
		}
	}
}