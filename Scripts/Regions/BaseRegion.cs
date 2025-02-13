using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Regions
{
	public enum SpawnZLevel
	{
		Lowest,
		Highest,
		Random
	}

	public class BaseRegion : Region
	{
		public static void Configure()
		{
			DefaultRegionType = typeof( BaseRegion );
		}

		private string m_RuneName;
		private bool m_NoLogoutDelay;

		private SpawnEntry[] m_Spawns;
		private SpawnZLevel m_SpawnZLevel;
		private bool m_ExcludeFromParentSpawns;
        private static readonly Hashtable m_NextDisplayTime = new Hashtable();

		public string RuneName{ get => m_RuneName;
            set => m_RuneName = value;
        }

		public bool NoLogoutDelay{ get => m_NoLogoutDelay;
            set => m_NoLogoutDelay = value;
        }

		public SpawnEntry[] Spawns
		{
			get => m_Spawns;
            set
			{
				if ( m_Spawns != null )
				{
					for ( int i = 0; i < m_Spawns.Length; i++ )
						m_Spawns[i].Delete();
				}

				m_Spawns = value;
			}
		}

		public SpawnZLevel SpawnZLevel{ get => m_SpawnZLevel;
            set => m_SpawnZLevel = value;
        }

		public bool ExcludeFromParentSpawns{ get => m_ExcludeFromParentSpawns;
            set => m_ExcludeFromParentSpawns = value;
        }

		public override void OnUnregister()
		{
			base.OnUnregister();

			Spawns = null;
		}

		public static string GetRuneNameFor( Region region )
		{
			while ( region != null )
			{
				BaseRegion br = region as BaseRegion;

				if ( br != null && br.m_RuneName != null )
					return br.m_RuneName;

                if (br == null)
                    return "an unknown location";

                if (region.Parent != null)
                    return region.Parent.ToString();
                else
                    return region.ToString();
			}

			return null;
		}

		public override TimeSpan GetLogoutDelay( Mobile m )
		{
			if ( m_NoLogoutDelay )
			{
				if ( m.Aggressors.Count == 0 && m.Aggressed.Count == 0 && !NotorietyHandlers.IsGuardCandidate(m) )
					return TimeSpan.Zero;
			}

			return base.GetLogoutDelay( m );
		}

		public static bool CanSpawn( Region region, params Type[] types )
		{
			while ( region != null )
			{
				if ( !region.AllowSpawn() )
					return false;

				BaseRegion br = region as BaseRegion;

				if ( br != null )
				{
					if ( br.Spawns != null )
					{
						for ( int i = 0; i < br.Spawns.Length; i++ )
						{
							SpawnEntry entry = br.Spawns[i];

							if ( entry.Definition.CanSpawn( types ) )
								return true;
						}
					}

					if ( br.ExcludeFromParentSpawns )
						return false;
				}

				region = region.Parent;
			}

			return false;
		}

		public override bool AcceptsSpawnsFrom( Region region )
		{
			if ( region == this || !m_ExcludeFromParentSpawns )
				return base.AcceptsSpawnsFrom( region );

			return false;
		}

		private Rectangle3D[] m_Rectangles;
		private int[] m_RectangleWeights;
		private int m_TotalWeight;

		private static readonly List<Rectangle3D> m_RectBuffer1 = new List<Rectangle3D>();
		private static readonly List<Rectangle3D> m_RectBuffer2 = new List<Rectangle3D>();

		private void InitRectangles()
		{
			if ( m_Rectangles != null )
				return;

			// Test if area rectangles are overlapping, and in that case break them into smaller non overlapping rectangles
			for ( int i = 0; i < Area.Length; i++ )
			{
				m_RectBuffer2.Add( Area[i] );

				for ( int j = 0; j < m_RectBuffer1.Count && m_RectBuffer2.Count > 0; j++ )
				{
					Rectangle3D comp = m_RectBuffer1[j];

					for ( int k = m_RectBuffer2.Count - 1; k >= 0; k-- )
					{
						Rectangle3D rect = m_RectBuffer2[k];

						int l1 = rect.Start.X, r1 = rect.End.X, t1 = rect.Start.Y, b1 = rect.End.Y;
						int l2 = comp.Start.X, r2 = comp.End.X, t2 = comp.Start.Y, b2 = comp.End.Y;

						if ( l1 < r2 && r1 > l2 && t1 < b2 && b1 > t2 )
						{
							m_RectBuffer2.RemoveAt( k );

							int sz = rect.Start.Z;
							int ez = rect.End.X;

							if ( l1 < l2 )
							{
								m_RectBuffer2.Add( new Rectangle3D( new Point3D( l1, t1, sz ), new Point3D( l2, b1, ez ) ) );
							}

							if ( r1 > r2 )
							{
								m_RectBuffer2.Add( new Rectangle3D( new Point3D( r2, t1, sz ), new Point3D( r1, b1, ez ) ) );
							}

							if ( t1 < t2 )
							{
								m_RectBuffer2.Add( new Rectangle3D( new Point3D( Math.Max( l1, l2 ), t1, sz ), new Point3D( Math.Min( r1, r2 ), t2, ez ) ) );
							}

							if ( b1 > b2 )
							{
								m_RectBuffer2.Add( new Rectangle3D( new Point3D( Math.Max( l1, l2 ), b2, sz ), new Point3D( Math.Min( r1, r2 ), b1, ez ) ) );
							}
						}
					}
				}

				m_RectBuffer1.AddRange( m_RectBuffer2 );
				m_RectBuffer2.Clear();
			}

			m_Rectangles = m_RectBuffer1.ToArray();
			m_RectBuffer1.Clear();

			m_RectangleWeights = new int[m_Rectangles.Length];
			for ( int i = 0; i < m_Rectangles.Length; i++ )
			{
				Rectangle3D rect = m_Rectangles[i];
				int weight = rect.Width * rect.Height;

				m_RectangleWeights[i] = weight;
				m_TotalWeight += weight;
			}
		}

		private static readonly List<Int32> m_SpawnBuffer1 = new List<Int32>();
		private static readonly List<Item>  m_SpawnBuffer2 = new List<Item>();

		public Point3D RandomSpawnLocation( int spawnHeight, bool land, bool water, Point3D home, int range )
		{
			Map map = Map;

			if ( map == Map.Internal )
				return Point3D.Zero;

			InitRectangles();

			if ( m_TotalWeight <= 0 )
				return Point3D.Zero;

			for ( int i = 0; i < 10; i++ ) // Try 10 times
			{
				int x, y, minZ, maxZ;

				if ( home == Point3D.Zero )
				{
					int rand = Utility.Random( m_TotalWeight );

					x = int.MinValue; y = int.MinValue;
					minZ = int.MaxValue; maxZ = int.MinValue;
					for ( int j = 0; j < m_RectangleWeights.Length; j++ )
					{
						int curWeight = m_RectangleWeights[j];

						if ( rand < curWeight )
						{
							Rectangle3D rect = m_Rectangles[j];

							x = rect.Start.X + rand % rect.Width;
							y = rect.Start.Y + rand / rect.Width;

							minZ = rect.Start.Z;
							maxZ = rect.End.Z;

							break;
						}

						rand -= curWeight;
					}
				}
				else
				{
					x = Utility.RandomMinMax( home.X - range, home.X + range );
					y = Utility.RandomMinMax( home.Y - range, home.Y + range );

					minZ = int.MaxValue; maxZ = int.MinValue;
					for ( int j = 0; j < Area.Length; j++ )
					{
						Rectangle3D rect = Area[j];

						if ( x >= rect.Start.X && x < rect.End.X && y >= rect.Start.Y && y < rect.End.Y )
						{
							minZ = rect.Start.Z;
							maxZ = rect.End.Z;
							break;
						}
					}

					if ( minZ == int.MaxValue )
						continue;
				}

				if ( x < 0 || y < 0 || x >= map.Width || y >= map.Height )
					continue;

                LandTile lt = map.Tiles.GetLandTile(x, y);

				int ltLowZ = 0, ltAvgZ = 0, ltTopZ = 0;
				map.GetAverageZ( x, y, ref ltLowZ, ref ltAvgZ, ref ltTopZ );

                TileFlag ltFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;
                bool ltImpassable = ((ltFlags & TileFlag.Impassable) != 0);

				if ( !lt.Ignored && ltAvgZ >= minZ && ltAvgZ < maxZ )
					if ( (ltFlags & TileFlag.Wet) != 0 ) {
						if ( water )
							m_SpawnBuffer1.Add( ltAvgZ );
					}
					else if ( land && !ltImpassable )
						m_SpawnBuffer1.Add( ltAvgZ );

                StaticTile[] staticTiles = map.Tiles.GetStaticTiles(x, y, true);

				for ( int j = 0; j < staticTiles.Length; j++ )
				{
                    StaticTile tile = staticTiles[j];
                    ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];
                    int tileZ = tile.Z + id.CalcHeight;

					if ( tileZ >= minZ && tileZ < maxZ )
						if ( (id.Flags & TileFlag.Wet) != 0 ) {
							if ( water )
								m_SpawnBuffer1.Add( tileZ );
						}
						else if ( land && id.Surface && !id.Impassable )
							m_SpawnBuffer1.Add( tileZ );
				}


				Sector sector = map.GetSector( x, y );

				for ( int j = 0; j < sector.Items.Count; j++ )
				{
					Item item = sector.Items[j];

                    if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y))
                    {
						m_SpawnBuffer2.Add( item );

						if ( !item.Movable )
						{
							ItemData id = item.ItemData;
							int itemZ = item.Z + id.CalcHeight;

							if ( itemZ >= minZ && itemZ < maxZ )
								if ( (id.Flags & TileFlag.Wet) != 0 ) {
									if ( water )
										m_SpawnBuffer1.Add( itemZ );
								}
								else if ( land && id.Surface && !id.Impassable )
									m_SpawnBuffer1.Add( itemZ );
						}
					}
				}


				if ( m_SpawnBuffer1.Count == 0 )
				{
					m_SpawnBuffer1.Clear();
					m_SpawnBuffer2.Clear();
					continue;
				}

				int z;
				switch ( m_SpawnZLevel )
				{
					case SpawnZLevel.Lowest:
					{
						z = int.MaxValue;

						for ( int j = 0; j < m_SpawnBuffer1.Count; j++ )
						{
							int l = m_SpawnBuffer1[j];

							if ( l < z )
								z = l;
						}

						break;
					}
					case SpawnZLevel.Highest:
					{
						z = int.MinValue;

						for ( int j = 0; j < m_SpawnBuffer1.Count; j++ )
						{
							int l = m_SpawnBuffer1[j];

							if ( l > z )
								z = l;
						}

						break;
					}
					default: // SpawnZLevel.Random
					{
						int index = Utility.Random( m_SpawnBuffer1.Count );
						z = m_SpawnBuffer1[index];

						break;
					}
				}

				m_SpawnBuffer1.Clear();


				if ( !Find( new Point3D( x, y, z ), map ).AcceptsSpawnsFrom( this ) )
				{
					m_SpawnBuffer2.Clear();
					continue;
				}

				int top = z + spawnHeight;

				bool ok = true;
				for ( int j = 0; j < m_SpawnBuffer2.Count; j++ )
				{
					Item item = m_SpawnBuffer2[j];
					ItemData id = item.ItemData;

					if ( ( id.Surface || id.Impassable ) && item.Z + id.CalcHeight > z && item.Z < top )
					{
						ok = false;
						break;
					}
				}

				m_SpawnBuffer2.Clear();

				if ( !ok )
					continue;

				if ( ltImpassable && ltAvgZ > z && ltLowZ < top )
					continue;

				for ( int j = 0; j < staticTiles.Length; j++ )
				{
                    StaticTile tile = staticTiles[j];
                    ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

					if ( ( id.Surface || id.Impassable ) && tile.Z + id.CalcHeight > z && tile.Z < top )
					{
						ok = false;
						break;
					}
				}

				if ( !ok )
					continue;

				for ( int j = 0; j < sector.Mobiles.Count; j++ )
				{
					Mobile m = sector.Mobiles[j];

					if ( m.X == x && m.Y == y && ( m.AccessLevel == AccessLevel.Player || !m.Hidden ) )
						if ( m.Z + 16 > z && m.Z < top )
						{
							ok = false;
							break;
						}
				}

				if ( ok )
					return new Point3D( x, y, z );
			}

			return Point3D.Zero;
		}

		public override string ToString()
		{
			if ( Name != null )
				return Name;
			else if ( RuneName != null )
				return RuneName;
			else
				return GetType().Name;
		}

        public override void OnEnter(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            #region Entering region broadcast system
            /*
            if (this is TownRegion || this is CustomRegion)
            {
                PlayerMobile from = (PlayerMobile) m;

                if (from.AccessLevel > AccessLevel.Player) //Don't broadcast staff's location
                    return;

                if (!from.ShowArriveMsg) //Show arrive message is disabled
                    return;

                if (this is CustomRegion)
                {
                    CustomRegion cR = (CustomRegion) this;
                    if (!cR.Controller.BroadcastArriveMsg || string.IsNullOrEmpty(cR.Name))
                        return;
                }

                DateTime dt = DateTime.MinValue;

                if (m_NextDisplayTime[from] != null)
                {
                    dt = (DateTime)m_NextDisplayTime[from];
                }

                if (DateTime.UtcNow > dt)
                {
                    from.SendAsciiMessage("You have arrived in " + Name);

                    List<Mobile> mobs = new List<Mobile>();

                    foreach ( NetState state in NetState.Instances )
                    {
                        Mobile mob = state.Mobile;
                        mobs.Add(mob);
                    }

                    for (int i = 0; i < mobs.Count; ++i)
                    {
                        if (!(mobs[i] is PlayerMobile))
                            continue;

                        PlayerMobile pm = (PlayerMobile) mobs[i];

                        if (pm.ShowArriveMsg && pm != from)
                        {
                            pm.SendAsciiMessage(33, string.Format("{0} has arrived in {1}", m.Name, Name));
                            m_NextDisplayTime[from] = DateTime.UtcNow + TimeSpan.FromSeconds(15);
                        }
                    }
                }
             
            }
            */
            #endregion
        }

		public BaseRegion( string name, Map map, int priority, params Rectangle2D[] area ) : base( name, map, priority, area )
		{
		}

		public BaseRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
		}

		public BaseRegion( string name, Map map, Region parent, params Rectangle2D[] area ) : base( name, map, parent, area )
		{
		}

		public BaseRegion( string name, Map map, Region parent, params Rectangle3D[] area ) : base( name, map, parent, area )
		{
		}

		public BaseRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
			ReadString( xml["rune"], "name", ref m_RuneName, false );

			bool logoutDelayActive = true;
			ReadBoolean( xml["logoutDelay"], "active", ref logoutDelayActive, false );
			m_NoLogoutDelay = !logoutDelayActive;


			XmlElement spawning = xml["spawning"];
			if ( spawning != null )
			{
				ReadBoolean( spawning, "excludeFromParent", ref m_ExcludeFromParentSpawns, false );

                SpawnZLevel zLevel = SpawnZLevel.Lowest;
                ReadEnum(spawning, "zLevel", ref zLevel, false);
                m_SpawnZLevel = zLevel;


				List<SpawnEntry> list = new List<SpawnEntry>();

				foreach ( XmlNode node in spawning.ChildNodes )
				{
					XmlElement el = node as XmlElement;

					if ( el != null )
					{
						SpawnDefinition def = SpawnDefinition.GetSpawnDefinition( el );
						if ( def == null )
							continue;

						int id = 0;
						if ( !ReadInt32( el, "id", ref id, true ) )
							continue;

						int amount = 0;
						if ( !ReadInt32( el, "amount", ref amount, true ) )
							continue;

						TimeSpan minSpawnTime = SpawnEntry.DefaultMinSpawnTime;
						ReadTimeSpan( el, "minSpawnTime", ref minSpawnTime, false );

						TimeSpan maxSpawnTime = SpawnEntry.DefaultMaxSpawnTime;
						ReadTimeSpan( el, "maxSpawnTime", ref maxSpawnTime, false );

						Point3D home = Point3D.Zero;
						int range = 0;

						XmlElement homeEl = el["home"];
						if ( ReadPoint3D( homeEl, map, ref home, false ) )
							ReadInt32( homeEl, "range", ref range, false );

                        Direction dir = SpawnEntry.InvalidDirection;
                        ReadEnum(el["direction"], "value", ref dir, false);

						SpawnEntry entry = new SpawnEntry( id, this, home, range, dir, def, amount, minSpawnTime, maxSpawnTime );
						list.Add( entry );
					}
				}

				if ( list.Count > 0 )
				{
					m_Spawns = list.ToArray();
				}
			}
		}
	}
}