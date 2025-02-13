using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class SBFurtrader : SBInfo 
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo(typeof(Hides), 8, Utility.RandomMinMax(35, 45), 0x1079, 0)); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Hides ), 2 ); 
                Add(typeof(Leather), 2);
			} 
		} 
	} 
} 
