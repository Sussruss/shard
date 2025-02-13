using System;
using System.Collections;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public delegate void InstrumentPickedCallback( Mobile from, BaseInstrument instrument );

	public enum InstrumentQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public abstract class BaseInstrument : Item, ICraftable, ISlayer
	{
		private int m_WellSound, m_BadlySound;
		private SlayerName m_Slayer, m_Slayer2;
		private InstrumentQuality m_Quality;
		private Mobile m_Crafter;
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int SuccessSound
		{
			get => m_WellSound;
            set => m_WellSound = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FailureSound
		{
			get => m_BadlySound;
            set => m_BadlySound = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer
		{
			get => m_Slayer;
            set{ m_Slayer = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer2
		{
			get => m_Slayer2;
            set{ m_Slayer2 = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public InstrumentQuality Quality
		{
			get => m_Quality;
            set{ UnscaleUses(); m_Quality = value; InvalidateProperties(); ScaleUses(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get => m_Crafter;
            set{ m_Crafter = value; InvalidateProperties(); }
		}

		public virtual int InitMinUses => 350;
        public virtual int InitMaxUses => 450;

        public virtual TimeSpan ChargeReplenishRate => TimeSpan.FromMinutes( 5.0 );

        [CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get{ CheckReplenishUses(); return m_UsesRemaining; }
			set{ m_UsesRemaining = value; InvalidateProperties(); }
		}

		private DateTime m_LastReplenished;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastReplenished
		{
			get => m_LastReplenished;
            set { m_LastReplenished = value; CheckReplenishUses(); }
		}

		private bool m_ReplenishesCharges;
		[CommandProperty( AccessLevel.GameMaster )]
		public bool ReplenishesCharges
		{
			get => m_ReplenishesCharges;
            set 
			{
				if( value != m_ReplenishesCharges && value )
					m_LastReplenished = DateTime.UtcNow;

				m_ReplenishesCharges = value; 
			}
		}

		public void CheckReplenishUses()
		{
			CheckReplenishUses( true );
		}

		public void CheckReplenishUses( bool invalidate )
		{
			if( !m_ReplenishesCharges || m_UsesRemaining >= InitMaxUses )
				return;

			if( m_LastReplenished + ChargeReplenishRate < DateTime.UtcNow )
			{
				TimeSpan timeDifference = DateTime.UtcNow - m_LastReplenished;

				m_UsesRemaining = Math.Min( m_UsesRemaining + (int)( timeDifference.Ticks / ChargeReplenishRate.Ticks), InitMaxUses );	//How rude of TimeSpan to not allow timespan division.
				m_LastReplenished = DateTime.UtcNow;

				if( invalidate )
					InvalidateProperties();

			}
		}

		public void ScaleUses()
		{
			UsesRemaining = (UsesRemaining * GetUsesScalar()) / 100;
			//InvalidateProperties();
		}

		public void UnscaleUses()
		{
			UsesRemaining = (UsesRemaining * 100) / GetUsesScalar();
		}

		public int GetUsesScalar()
		{
			if ( m_Quality == InstrumentQuality.Exceptional )
				return 200;

			return 100;
		}

		public void ConsumeUse( Mobile from )
		{
			// TODO: Confirm what must happen here?

			if ( UsesRemaining > 1 )
			{
                //Taran - Instruments should never brake
				//--UsesRemaining;
			}
			else
			{
				if ( from != null )
					from.SendLocalizedMessage( 502079 ); // The instrument played its last tune.

				Delete();
			}
		}

		private static readonly Hashtable m_Instruments = new Hashtable();

		public static BaseInstrument GetInstrument( Mobile from )
		{
			BaseInstrument item = m_Instruments[from] as BaseInstrument;

			if ( item == null )
				return null;

			if ( !item.IsChildOf( from.Backpack ) )
			{
				m_Instruments.Remove( from );
				return null;
			}

			return item;
		}

		public static int GetBardRange( Mobile bard, SkillName skill )
		{
			return 8 + (int)(bard.Skills[skill].Value / 15);
		}

		public static void PickInstrument( Mobile from, InstrumentPickedCallback callback )
		{
            bool releaseLock = true;
			BaseInstrument instrument = GetInstrument( from );

			if ( instrument != null )
			{
				if ( callback != null )
					callback( from, instrument );
			}
			else
			{
				from.SendLocalizedMessage( 500617 ); // What instrument shall you play?
				from.BeginTarget( 1, false, TargetFlags.None, OnPickedInstrument, callback );
			}

            if (releaseLock && from is PlayerMobile)
                ((PlayerMobile)from).EndPlayerAction();
		}

		public static void OnPickedInstrument( Mobile from, object targeted, object state )
		{
            bool releaseLock = true;
			BaseInstrument instrument = targeted as BaseInstrument;

			if ( instrument == null )
			{
				from.SendLocalizedMessage( 500619 ); // That is not a musical instrument.
			}
			else
			{
                releaseLock = false;
				SetInstrument( from, instrument );

				InstrumentPickedCallback callback = state as InstrumentPickedCallback;

				if ( callback != null )
					callback( from, instrument );
			}

            if (releaseLock && from is PlayerMobile)
                ((PlayerMobile)from).EndPlayerAction();
		}

		public static bool IsMageryCreature( BaseCreature bc )
		{
			return ( bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0 );
		}

		public static bool IsFireBreathingCreature( BaseCreature bc )
		{
			if ( bc == null )
				return false;

			return bc.HasBreath;
		}

		public static bool IsPoisonImmune( BaseCreature bc )
		{
			return ( bc != null && bc.PoisonImmune != null );
		}

		public static int GetPoisonLevel( BaseCreature bc )
		{
			if ( bc == null )
				return 0;

			Poison p = bc.HitPoison;

			if ( p == null )
				return 0;

			return p.Level + 1;
		}

		public static double GetBaseDifficulty( Mobile targ )
		{
			/* Difficulty TODO: Add another 100 points for each of the following abilities:
				- Radiation or Aura Damage (Heat, Cold etc.)
				- Summoning Undead
			*/

            double val = (targ.HitsMax * 1.6) + targ.StamMax + targ.ManaMax;

			val += targ.SkillsTotal / 10;

			if ( val > 700 )
				val = 700 + (int)((val - 700) * (3.0 / 11));

			BaseCreature bc = targ as BaseCreature;

			if ( IsMageryCreature( bc ) )
				val += 100;

			if ( IsFireBreathingCreature( bc ) )
				val += 100;

			if ( IsPoisonImmune( bc ) )
				val += 100;

			if ( targ is VampireBat || targ is VampireBatFamiliar )
				val += 100;

			val += GetPoisonLevel( bc ) * 20;

			val /= 10;

			if ( bc != null && bc.IsParagon )
				val += 40.0;

			if ( Core.SE && val > 160.0 )
				val = 160.0;

			return val;
		}

		public double GetDifficultyFor( Mobile targ )
		{
			double val = GetBaseDifficulty( targ );

			if ( m_Quality == InstrumentQuality.Exceptional )
				val -= 5.0; // 10%

			if ( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );

				if ( entry != null )
				{
					if ( entry.Slays( targ ) )
						val -= 10.0; // 20%
					else if ( entry.Group.OppositionSuperSlays( targ ) )
						val += 10.0; // -20%
				}
			}

			if ( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );

				if ( entry != null )
				{
					if ( entry.Slays( targ ) )
						val -= 10.0; // 20%
					else if ( entry.Group.OppositionSuperSlays( targ ) )
						val += 10.0; // -20%
				}
			}

			return val;
		}

		public static void SetInstrument( Mobile from, BaseInstrument item )
		{
			m_Instruments[from] = item;
		}

		public BaseInstrument( int itemID, int wellSound, int badlySound ) : base( itemID )
		{
			m_WellSound = wellSound;
			m_BadlySound = badlySound;
			UsesRemaining = Utility.RandomMinMax( InitMinUses, InitMaxUses );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			int oldUses = m_UsesRemaining;
			CheckReplenishUses( false );

			base.GetProperties( list );

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == InstrumentQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if( m_ReplenishesCharges )
				list.Add( 1070928 ); // Replenish Charges

			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_UsesRemaining != oldUses )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( InvalidateProperties ) );
		}

		public override void OnSingleClick( Mobile from )
		{
            LabelTo(from, Sphere.ComputeName(this));
            /*
			ArrayList attrs = new ArrayList();

			if ( DisplayLootType )
			{
				if ( LootType == LootType.Blessed )
					attrs.Add( new EquipInfoAttribute( 1038021 ) ); // blessed
				else if ( LootType == LootType.Cursed )
					attrs.Add( new EquipInfoAttribute( 1049643 ) ); // cursed
			}

			if ( m_Quality == InstrumentQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );

			if( m_ReplenishesCharges )
				attrs.Add( new EquipInfoAttribute( 1070928 ) ); // Replenish Charges

			// TODO: Must this support item identification?
			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					attrs.Add( new EquipInfoAttribute( entry.Title ) );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					attrs.Add( new EquipInfoAttribute( entry.Title ) );
			}

			int number;

			if ( Name == null )
			{
				number = LabelNumber;
			}
			else
			{
				this.LabelTo( from, Name );
				number = 1041000;
			}

			if ( attrs.Count == 0 && Crafter == null && Name != null )
				return;

			EquipmentInfo eqInfo = new EquipmentInfo( number, m_Crafter, false, (EquipInfoAttribute[])attrs.ToArray( typeof( EquipInfoAttribute ) ) );

			from.Send( new DisplayEquipmentInfo( this, eqInfo ) );
            */
		}

		public BaseInstrument( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 3 ); // version

			writer.Write( m_ReplenishesCharges );
			if( m_ReplenishesCharges )
				writer.Write( m_LastReplenished );


			writer.Write( m_Crafter );

			writer.WriteEncodedInt( (int) m_Quality );
			writer.WriteEncodedInt( (int) m_Slayer );
			writer.WriteEncodedInt( (int) m_Slayer2 );

			writer.WriteEncodedInt( UsesRemaining );

			writer.WriteEncodedInt( m_WellSound );
			writer.WriteEncodedInt( m_BadlySound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_ReplenishesCharges = reader.ReadBool();

					if( m_ReplenishesCharges )
						m_LastReplenished = reader.ReadDateTime();

					goto case 2;
				}
				case 2:
				{
					m_Crafter = reader.ReadMobile();

					m_Quality = (InstrumentQuality)reader.ReadEncodedInt();
					m_Slayer = (SlayerName)reader.ReadEncodedInt();
					m_Slayer2 = (SlayerName)reader.ReadEncodedInt();

					UsesRemaining = reader.ReadEncodedInt();

					m_WellSound = reader.ReadEncodedInt();
					m_BadlySound = reader.ReadEncodedInt();
					
					break;
				}
				case 1:
				{
					m_Crafter = reader.ReadMobile();

					m_Quality = (InstrumentQuality)reader.ReadEncodedInt();
					m_Slayer = (SlayerName)reader.ReadEncodedInt();

					UsesRemaining = reader.ReadEncodedInt();

					m_WellSound = reader.ReadEncodedInt();
					m_BadlySound = reader.ReadEncodedInt();

					break;
				}
				case 0:
				{
					m_WellSound = reader.ReadInt();
					m_BadlySound = reader.ReadInt();
					UsesRemaining = Utility.RandomMinMax( InitMinUses, InitMaxUses );

					break;
				}
			}

			CheckReplenishUses();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 1 ) )
			{
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
			}
			else if ( from.BeginAction( typeof( BaseInstrument ) ) )
			{
				SetInstrument( from, this );

				// Delay of 7 second before beign able to play another instrument again
				new InternalTimer( from ).Start();

				if ( CheckMusicianship( from ) )
					PlayInstrumentWell( from );
				else
					PlayInstrumentBadly( from );
			}
			else
			{
				from.SendLocalizedMessage( 500119 ); // You must wait to perform another action
			}
		}

		public static bool CheckMusicianship( Mobile m )
		{
			m.CheckSkill( SkillName.Musicianship, 0.0, 120.0 );

			return ( (m.Skills[SkillName.Musicianship].Value / 100) > Utility.RandomDouble() );
		}

		public void PlayInstrumentWell( Mobile from )
		{
			from.PlaySound( m_WellSound );
		}

		public void PlayInstrumentBadly( Mobile from )
		{
			from.PlaySound( m_BadlySound );
		}

		private class InternalTimer : Timer
		{
			private readonly Mobile m_From;

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 6.0 ) )
			{
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				m_From.EndAction( typeof( BaseInstrument ) );
			}
		}
		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (InstrumentQuality)quality;

			if ( makersMark )
				Crafter = from;

			return quality;
		}

		#endregion
	}
}