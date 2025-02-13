using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public class Teleporter : Item
	{
        private bool m_Active, m_Creatures, m_CombatCheck;
		private Point3D m_PointDest;
		private Map m_MapDest;
		private bool m_SourceEffect;
		private bool m_DestEffect;
        private bool m_RemoveEventGear = true;
        private bool m_AllowCriminals = true;
		private int m_SoundID;
		private TimeSpan m_Delay;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowCriminals
        {
            get => m_AllowCriminals;
            set { m_AllowCriminals = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveEventGear
        {
            get => m_RemoveEventGear;
            set => m_RemoveEventGear = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool SourceEffect
		{
			get => m_SourceEffect;
            set => m_SourceEffect = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool DestEffect
		{
			get => m_DestEffect;
            set{ m_DestEffect = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SoundID
		{
			get => m_SoundID;
            set{ m_SoundID = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Delay
		{
			get => m_Delay;
            set{ m_Delay = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get => m_Active;
            set { m_Active = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest
		{
			get => m_PointDest;
            set { m_PointDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest
		{
			get => m_MapDest;
            set { m_MapDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Creatures
		{
			get => m_Creatures;
            set { m_Creatures = value; InvalidateProperties(); }
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CombatCheck
        {
            get => m_CombatCheck;
            set { m_CombatCheck = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DeleteOnUse{ get; set; }

		public override int LabelNumber => 1026095; // teleporter

		[Constructable]
		public Teleporter() : this( new Point3D( 0, 0, 0 ), null, false )
		{
		}

		[Constructable]
		public Teleporter( Point3D pointDest, Map mapDest ) : this( pointDest, mapDest, false )
		{
		}

		[Constructable]
		public Teleporter( Point3D pointDest, Map mapDest, bool creatures ) : base( 0x1BC3 )
		{
		    Name = "a teleporter";
			Movable = false;
			Visible = false;

			m_Active = true;
			m_PointDest = pointDest;
			m_MapDest = mapDest;
			m_Creatures = creatures;

            m_CombatCheck = false;

		    DeleteOnUse = false;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Active )
				list.Add( 1060742 ); // active
			else
				list.Add( 1060743 ); // inactive

			if ( m_MapDest != null )
				list.Add( 1060658, "Map\t{0}", m_MapDest );

			if ( m_PointDest != Point3D.Zero )
				list.Add( 1060659, "Coords\t{0}", m_PointDest );

			list.Add( 1060660, "Creatures\t{0}", m_Creatures ? "Yes" : "No" );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( m_Active )
			{
                if (from.AccessLevel >= AccessLevel.Counselor)
                {
                    LabelTo(from, "GM View:");

                    if (m_MapDest != null && m_PointDest != Point3D.Zero)
                        LabelTo(from, "{0} [{1}]", m_PointDest, m_MapDest);
                    else if (m_MapDest != null)
                        LabelTo(from, "[{0}]", m_MapDest);
                    else if (m_PointDest != Point3D.Zero)
                        LabelTo(from, m_PointDest.ToString());
                }
                else if (string.IsNullOrEmpty(Name))
                    LabelTo(from, Name);
                else
                    LabelTo(from, "a teleporter");
			}
			else
			{
				LabelTo( from, "(inactive)" );
			}
		}

		public virtual void StartTeleport( Mobile m )
		{
			if ( m_Delay == TimeSpan.Zero )
				DoTeleport( m );
			else
                Timer.DelayCall<Mobile>(m_Delay, DoTeleport, m);
        }

		public virtual void DoTeleport( Mobile m )
		{
			Map map = m_MapDest;

			if ( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = m_PointDest;

			if ( p == Point3D.Zero )
				p = m.Location;

			BaseCreature.TeleportPets( m, p, map );

			bool sendEffect = ( !m.Hidden || m.AccessLevel == AccessLevel.Player );

			if ( m_SourceEffect && sendEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			m.MoveToWorld( p, map );

			if ( m_DestEffect && sendEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			if ( m_SoundID > 0 && sendEffect )
				Effects.PlaySound( m.Location, m.Map, m_SoundID );

            if (DeleteOnUse)
                Delete();
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m_Active )
			{
				if ( !m_Creatures && !m.Player )
					return true;

                if (m_CombatCheck && SpellHelper.CheckCombat(m))
                {
                    m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                    return true;
                }

                if (!m_AllowCriminals && (m.Criminal || Misc.NotorietyHandlers.IsGuardCandidate(m)))
                {
                    m.SendAsciiMessage("Criminals or murderers can't use this teleporter!");
                    return true;
                }

                if (m_RemoveEventGear && m.IsInEvent)
                {
                    m.IsInEvent = false;
                    m.SendAsciiMessage("You auto supply has been removed.");

                    SupplySystem.RemoveEventGear(m);
                }

			    StartTeleport( m );
				return false;
			}

			return true;
		}

		public Teleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 5 ); // version

            writer.Write(DeleteOnUse);

            writer.Write(m_CombatCheck);

		    writer.Write(m_RemoveEventGear);
		    writer.Write(m_AllowCriminals);

			writer.Write( m_SourceEffect );
			writer.Write( m_DestEffect );
			writer.Write( m_Delay );
			writer.WriteEncodedInt( m_SoundID );

			writer.Write( m_Creatures );

			writer.Write( m_Active );
			writer.Write( m_PointDest );
			writer.Write( m_MapDest );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 5:
			        {
			            DeleteOnUse = reader.ReadBool();
			            goto case 4;
			        }
                case 4:
                    {
                        m_CombatCheck = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
			        {
			            m_RemoveEventGear = reader.ReadBool();
			            m_AllowCriminals = reader.ReadBool();
			            goto case 2;
			        }
				case 2:
				{
					m_SourceEffect = reader.ReadBool();
					m_DestEffect = reader.ReadBool();
					m_Delay = reader.ReadTimeSpan();
					m_SoundID = reader.ReadEncodedInt();

					goto case 1;
				}
				case 1:
				{
					m_Creatures = reader.ReadBool();

					goto case 0;
				}
				case 0:
				{
					m_Active = reader.ReadBool();
					m_PointDest = reader.ReadPoint3D();
					m_MapDest = reader.ReadMap();

					break;
				}
			}
		}
	}

	public class SkillTeleporter : Teleporter
	{
		private SkillName m_Skill;
		private double m_Required;
		private string m_MessageString;
		private int m_MessageNumber;

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get => m_Skill;
            set{ m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double Required
		{
			get => m_Required;
            set{ m_Required = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string MessageString
		{
			get => m_MessageString;
            set{ m_MessageString = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MessageNumber
		{
			get => m_MessageNumber;
            set{ m_MessageNumber = value; InvalidateProperties(); }
		}

		private void EndMessageLock( object state )
		{
			((Mobile)state).EndAction( this );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( Active )
			{
				if ( !Creatures && !m.Player )
					return true;

				Skill sk = m.Skills[m_Skill];

				if ( sk == null || sk.Base < m_Required )
				{
					if ( m.BeginAction( this ) )
					{
						if ( m_MessageString != null )
							m.Send( new UnicodeMessage( Serial, ItemID, MessageType.Regular, 0x3B2, 3, "ENU", null, m_MessageString ) );
						else if ( m_MessageNumber != 0 )
							m.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, m_MessageNumber, null, "" ) );

						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
					}

					return false;
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int skillIndex = (int)m_Skill;
			string skillName;

			if ( skillIndex >= 0 && skillIndex < SkillInfo.Table.Length )
				skillName = SkillInfo.Table[skillIndex].Name;
			else
				skillName = "(Invalid)";

			list.Add( 1060661, "{0}\t{1:F1}", skillName, m_Required );

			if ( m_MessageString != null )
				list.Add( 1060662, "Message\t{0}", m_MessageString );
			else if ( m_MessageNumber != 0 )
				list.Add( 1060662, "Message\t#{0}", m_MessageNumber );
		}

		[Constructable]
		public SkillTeleporter()
		{
		}

		public SkillTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version

			writer.Write( (int) m_Skill );
			writer.Write( m_Required );
			writer.Write( m_MessageString );
			writer.Write( m_MessageNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Skill = (SkillName)reader.ReadInt();
					m_Required = reader.ReadDouble();
					m_MessageString = reader.ReadString();
					m_MessageNumber = reader.ReadInt();

					break;
				}
			}
		}
	}

	public class KeywordTeleporter : Teleporter
	{
		private string m_Substring;
		private int m_Keyword;
		private int m_Range;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Substring
		{
			get => m_Substring;
            set{ m_Substring = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Keyword
		{
			get => m_Keyword;
            set{ m_Keyword = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get => m_Range;
            set{ m_Range = value; InvalidateProperties(); }
		}

		public override bool HandlesOnSpeech => true;

        public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && Active )
			{
				Mobile m = e.Mobile;

				if ( !Creatures && !m.Player )
					return;

				if ( !m.InRange( GetWorldLocation(), m_Range ) )
					return;

				bool isMatch = false;

				if ( m_Keyword >= 0 && e.HasKeyword( m_Keyword ) )
					isMatch = true;
				else if ( m_Substring != null && e.Speech.ToLower().IndexOf( m_Substring.ToLower() ) >= 0 )
					isMatch = true;

				if ( !isMatch )
					return;

				e.Handled = true;
				StartTeleport( m );
			}
		}

        public override void DoTeleport(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), m_Range) || m.Map != Map)
                return;

            base.DoTeleport(m);
        }

		public override bool OnMoveOver( Mobile m )
		{
			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060661, "Range\t{0}", m_Range );

			if ( m_Keyword >= 0 )
				list.Add( 1060662, "Keyword\t{0}", m_Keyword );

			if ( m_Substring != null )
				list.Add( 1060663, "Substring\t{0}", m_Substring );
		}

		[Constructable]
		public KeywordTeleporter()
		{
			m_Keyword = -1;
			m_Substring = null;
		}

		public KeywordTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version

			writer.Write( m_Substring );
			writer.Write( m_Keyword );
			writer.Write( m_Range );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Substring = reader.ReadString();
					m_Keyword = reader.ReadInt();
					m_Range = reader.ReadInt();

					break;
				}
			}
		}
	}

    public class WaitTeleporter : KeywordTeleporter
    {
        private static Dictionary<Mobile, TeleportingInfo> m_Table;

        public static void Initialize()
        {
            m_Table = new Dictionary<Mobile, TeleportingInfo>();

            EventSink.Logout += new LogoutEventHandler(EventSink_Logout);
        }

        public static void EventSink_Logout(LogoutEventArgs e)
        {
            Mobile from = e.Mobile;
            TeleportingInfo info;

            if (from == null || !m_Table.TryGetValue(from, out info))
                return;

            info.Timer.Stop();
            m_Table.Remove(from);
        }

        private int m_StartNumber;
        private string m_StartMessage;
        private int m_ProgressNumber;
        private string m_ProgressMessage;
        private bool m_ShowTimeRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int StartNumber
        {
            get => m_StartNumber;
            set => m_StartNumber = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string StartMessage
        {
            get => m_StartMessage;
            set => m_StartMessage = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ProgressNumber
        {
            get => m_ProgressNumber;
            set => m_ProgressNumber = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ProgressMessage
        {
            get => m_ProgressMessage;
            set => m_ProgressMessage = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowTimeRemaining
        {
            get => m_ShowTimeRemaining;
            set => m_ShowTimeRemaining = value;
        }

        [Constructable]
        public WaitTeleporter()
        {
        }

        public static string FormatTime(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
            {
                int h = (int)ts.TotalHours;
                return String.Format("{0} hour{1}", h, (h == 1) ? "" : "s");
            }
            else if (ts.TotalMinutes >= 1)
            {
                int m = (int)ts.TotalMinutes;
                return String.Format("{0} minute{1}", m, (m == 1) ? "" : "s");
            }

            int s = Math.Max((int)ts.TotalSeconds, 0);
            return String.Format("{0} second{1}", s, (s == 1) ? "" : "s");
        }

        private void EndLock(Mobile m)
        {
            m.EndAction(this);
        }

        public override void StartTeleport(Mobile m)
        {
            TeleportingInfo info;

            if (m_Table.TryGetValue(m, out info))
            {
                if (info.Teleporter == this)
                {
                    if (m.BeginAction(this))
                    {
                        if (m_ProgressMessage != null)
                            m.SendMessage(m_ProgressMessage);
                        else if (m_ProgressNumber != 0)
                            m.SendLocalizedMessage(m_ProgressNumber);

                        if (m_ShowTimeRemaining)
                            m.SendMessage("Time remaining: {0}", FormatTime(m_Table[m].Timer.Next - DateTime.UtcNow));

                        Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(5), EndLock, m);
                    }

                    return;
                }
                else
                {
                    info.Timer.Stop();
                }
            }

            if (m_StartMessage != null)
                m.SendMessage(m_StartMessage);
            else if (m_StartNumber != 0)
                m.SendLocalizedMessage(m_StartNumber);

            if (Delay == TimeSpan.Zero)
                DoTeleport(m);
            else
                m_Table[m] = new TeleportingInfo(this, Timer.DelayCall<Mobile>(Delay, DoTeleport, m));
        }

        public override void DoTeleport(Mobile m)
        {
            m_Table.Remove(m);

            base.DoTeleport(m);
        }

        public WaitTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_StartNumber);
            writer.Write(m_StartMessage);
            writer.Write(m_ProgressNumber);
            writer.Write(m_ProgressMessage);
            writer.Write(m_ShowTimeRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_StartNumber = reader.ReadInt();
            m_StartMessage = reader.ReadString();
            m_ProgressNumber = reader.ReadInt();
            m_ProgressMessage = reader.ReadString();
            m_ShowTimeRemaining = reader.ReadBool();
        }

        private class TeleportingInfo
        {
            private WaitTeleporter m_Teleporter;
            private Timer m_Timer;

            public WaitTeleporter Teleporter => m_Teleporter;
            public Timer Timer => m_Timer;

            public TeleportingInfo(WaitTeleporter tele, Timer t)
            {
                m_Teleporter = tele;
                m_Timer = t;
            }
        }
    }
}