using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			if ( Core.AOS )
			{
				Register( new PoisonImpl( "Lesser",		0,  4, 16,  7.5, 3.0, 2.25, 10, 4 ) );
				Register( new PoisonImpl( "Regular",	1,  8, 18, 10.0, 3.0, 3.25, 10, 3 ) );
				Register( new PoisonImpl( "Greater",	2, 12, 20, 15.0, 3.0, 4.25, 10, 2 ) );
				Register( new PoisonImpl( "Deadly",		3, 16, 30, 30.0, 3.0, 5.25, 15, 2 ) );
				Register( new PoisonImpl( "Lethal",		4, 20, 50, 35.0, 3.0, 5.25, 20, 2 ) );
			}
			else
			{
				Register( new PoisonImpl( "Lesser",		0, 4, 14,  2.500, 8.5, 4.0, 5, 4 ) );
				Register( new PoisonImpl( "Regular",	1, 5, 15,  3.125, 9.0, 4.5, 6, 3 ) );
				Register( new PoisonImpl( "Greater",	2, 6, 17,  9.250, 9.5, 4.5, 7, 2 ) );
				Register( new PoisonImpl( "Deadly",		3, 7, 19, 12.500, 10.0, 4.5, 8, 2 ) );
				Register( new PoisonImpl( "Lethal",		4, 9, 26, 25.000, 8.0, 5.0, 8, 2 ) );
			}
		}

		public static Poison IncreaseLevel( Poison oldPoison )
		{
			Poison newPoison = ( oldPoison == null ? null : GetPoison( oldPoison.Level + 1 ) );

			return ( newPoison == null ? oldPoison : newPoison );
		}

		// Info
		private readonly string m_Name;
		private readonly int m_Level;

		// Damage
		private readonly int m_Minimum;
	    private readonly int m_Maximum;
	    private readonly double m_Scalar;

		// Timers
		private readonly TimeSpan m_Delay;
		private readonly TimeSpan m_Interval;
		private readonly int m_Count;
	    private readonly int m_MessageInterval;

	    public PoisonImpl( string name, int level, int min, int max, double percent, double delay, double interval, int count, int messageInterval )
		{
            m_Name = name;
            m_Level = level;
            m_Minimum = min;
            m_Maximum = max;
            m_Scalar = percent * 0.01;
            m_Delay = TimeSpan.FromSeconds(delay);
	        m_Interval = TimeSpan.FromSeconds(interval);
             //m_Delay = TimeSpan.FromSeconds(120);
            //m_Interval = TimeSpan.FromSeconds(9);
            m_Count = count;
            m_MessageInterval = messageInterval;
            AddDelay = new TimeSpan(0);
        }

		public override string Name => m_Name;
        public override int Level => m_Level;

        public class PoisonTimer : Timer
		{
			private readonly PoisonImpl m_Poison;
			private readonly Mobile m_Mobile;
			private Mobile m_From;
			private int m_LastDamage;
			private int m_Index;

			public Mobile From{ get => m_From;
                set => m_From = value;
            }

			public PoisonTimer( Mobile m, PoisonImpl p ) : base( p.m_Delay + p.AddDelay, p.m_Interval )
			{
				m_From = m;
				m_Mobile = m;
				m_Poison = p;
                m_From.SendAsciiMessage("You have been poisoned!");
			}

			protected override void OnTick()
			{
                if ((Core.AOS && m_Poison.Level < 4 && TransformationSpellHelper.UnderTransformation(m_Mobile, typeof(VampiricEmbraceSpell))) ||
                    (m_Poison.Level < 3 && OrangePetals.UnderEffect(m_Mobile)) ||
					AnimalForm.UnderTransformation( m_Mobile, typeof( Unicorn ) ) )
				{
					if ( m_Mobile.CurePoison( m_Mobile ) )
					{
						m_Mobile.SendAsciiMessage("You feel yourself resisting the effects of the poison" );

						m_Mobile.NonlocalOverheadMessage( MessageType.Emote, 0x22, true,
							String.Format( "* {0} seems resistant to the poison *", m_Mobile.Name ) );

						Stop();
						return;
					}
				}



				if ( m_Index++ == m_Poison.m_Count )
				{
					m_Mobile.SendLocalizedMessage( 502136 ); // The poison seems to have worn off.
					m_Mobile.Poison = null;

					Stop();
					return;
				}

				int damage;

				if ( !Core.AOS && m_LastDamage != 0 && Utility.RandomBool() )
				{
					damage = m_LastDamage;
				}
				else
				{
					damage = 1 + (int)(m_Mobile.Hits * m_Poison.m_Scalar);

					if ( damage < m_Poison.m_Minimum )
						damage = m_Poison.m_Minimum;
					else if ( damage > m_Poison.m_Maximum )
						damage = m_Poison.m_Maximum;

					m_LastDamage = damage;
				}

				if ( m_From != null )
					m_From.DoHarmful( m_Mobile, true );

				IHonorTarget honorTarget = m_Mobile as IHonorTarget;
				if ( honorTarget != null && honorTarget.ReceivedHonorContext != null )
					honorTarget.ReceivedHonorContext.OnTargetPoisoned();

				AOS.Damage( m_Mobile, m_From, damage, 0, 0, 0, 100, 0 );

                //if (0.60 <= Utility.RandomDouble()) // OSI: randomly revealed between first and third damage tick, guessing 60% chance
                //    m_Mobile.RevealingAction();

				if ( (m_Index % m_Poison.m_MessageInterval) == 0 )
					m_Mobile.OnPoisoned( m_From, m_Poison, m_Poison );
			}
		}

		public override Timer ConstructTimer( Mobile m )
		{
			return new PoisonTimer( m, this );
		}
	}
}