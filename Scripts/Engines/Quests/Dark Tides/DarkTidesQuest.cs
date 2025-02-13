using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
	public class DarkTidesQuest : QuestSystem
	{
		private static readonly Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( AcceptConversation ),
				typeof( AnimateMaabusCorpseObjective ),
				typeof( BankerConversation ),
				typeof( CashBankCheckObjective ),
				typeof( FetchAbraxusScrollObjective ),
				typeof( FindBankObjective ),
				typeof( FindCallingScrollObjective ),
				typeof( FindCityOfLightObjective ),
				typeof( FindCrystalCaveObjective ),
				typeof( FindMaabusCorpseObjective ),
				typeof( FindMaabusTombObjective ),
				typeof( FindMardothAboutKronusObjective ),
				typeof( FindMardothAboutVaultObjective ),
				typeof( FindMardothEndObjective ),
				typeof( FindVaultOfSecretsObjective ),
				typeof( FindWellOfTearsObjective ),
				typeof( HorusConversation ),
				typeof( LostCallingScrollConversation ),
				typeof( MaabasConversation ),
				typeof( MardothEndConversation ),
				typeof( MardothKronusConversation ),
				typeof( MardothVaultConversation ),
				typeof( RadarConversation ),
				typeof( ReadAbraxusScrollConversation ),
				typeof( ReadAbraxusScrollObjective ),
				typeof( ReanimateMaabusConversation ),
				typeof( RetrieveAbraxusScrollObjective ),
				typeof( ReturnToCrystalCaveObjective ),
				typeof( SecondHorusConversation ),
				typeof( SpeakCavePasswordObjective ),
				typeof( UseCallingScrollObjective ),
				typeof( VaultOfSecretsConversation ),
				typeof( FindHorusAboutRewardObjective ),
				typeof( HealConversation ),
				typeof( HorusRewardConversation )
			};

		public override Type[] TypeReferenceTable => m_TypeReferenceTable;

        public override object Name =>
            // Dark Tides
            1060095;

        public override object OfferMessage =>
            /* <I>An old man who looks to be 200 years old from the looks
				 * of his translucently pale and heavily wrinkled skin, turns
				 * to you and gives you a half-cocked grin that makes you
				 * feel somewhat uneasy.<BR><BR>
				 * 
				 * After a short pause, he begins to speak to you...</I><BR><BR>
				 * 
				 * Hmm. What's this?  Another budding Necromancer to join the
				 * ranks of Evil?  Here... let me take a look at you...  Ah
				 * yes...  Very Good! I sense the forces of evil are strong
				 * within you, child � but you need training so that you can
				 * learn to focus your skills against those aligned against
				 * our cause.  You are destined to become a legendary
				 * Necromancer - with the proper training, that only I can
				 * give you.<BR><BR>
				 * 
				 * <I>Mardoth pauses just long enough to give you a wide,
				 * skin-crawling grin.</I><BR><BR>
				 * 
				 * Let me introduce myself. I am Mardoth, the guildmaster of
				 * the Necromantic Brotherhood.  I have taken it upon myself
				 * to train anyone willing to learn the dark arts of Necromancy.
				 * The path of destruction, decay and obliteration is not an
				 * easy one.  Only the most evil and the most dedicated can
				 * hope to master the sinister art of death.<BR><BR>
				 * 
				 * I can lend you training and help supply you with equipment �
				 * in exchange for a few services rendered by you, of course.
				 * Nothing major, just a little death and destruction here and
				 * there - the tasks should be easy as a tasty meat pie for one
				 * as treacherous and evil as yourself.<BR><BR>
				 * 
				 * What do you say?  Do we have a deal?
				 */
            1060094;

        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;

        public override int Picture => 0x15B5;

        public DarkTidesQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
		public DarkTidesQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();

			AddConversation( new AcceptConversation() );
		}

		public override bool IgnoreYoungProtection( Mobile from )
		{
			if ( from is SummonedPaladin )
				return true;

			return base.IgnoreYoungProtection( from );
		}

		public static bool HasLostCallingScroll( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is DarkTidesQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( FindMardothAboutKronusObjective ) ) || qs.IsObjectiveInProgress( typeof( FindWellOfTearsObjective ) ) || qs.IsObjectiveInProgress( typeof( UseCallingScrollObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( KronusScroll ) ) == null );
				}
			}

			return false;
		}
	}
}