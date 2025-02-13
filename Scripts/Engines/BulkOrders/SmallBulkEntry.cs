using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.BulkOrders
{
	public class SmallBulkEntry
	{
		private readonly Type m_Type;
		private readonly int m_Number;
		private readonly int m_Graphic;

		public Type Type => m_Type;
        public int Number => m_Number;
        public int Graphic => m_Graphic;

        public SmallBulkEntry( Type type, int number, int graphic )
		{
			m_Type = type;
			m_Number = number;
			m_Graphic = graphic;
		}

		public static SmallBulkEntry[] BlacksmithWeapons => GetEntries( "Blacksmith", "weapons" );

        public static SmallBulkEntry[] BlacksmithArmor => GetEntries( "Blacksmith", "armor" );

        public static SmallBulkEntry[] TailorCloth => GetEntries( "Tailoring", "cloth" );

        public static SmallBulkEntry[] TailorLeather => GetEntries( "Tailoring", "leather" );

        private static Hashtable m_Cache;

		public static SmallBulkEntry[] GetEntries( string type, string name )
		{
			if ( m_Cache == null )
				m_Cache = new Hashtable();

			Hashtable table = (Hashtable)m_Cache[type];

			if ( table == null )
				m_Cache[type] = table = new Hashtable();

			SmallBulkEntry[] entries = (SmallBulkEntry[])table[name];

			if ( entries == null )
				table[name] = entries = LoadEntries( type, name );

			return entries;
		}

		public static SmallBulkEntry[] LoadEntries( string type, string name )
		{
			return LoadEntries( String.Format( "Data/Bulk Orders/{0}/{1}.cfg", type, name ) );
		}

		public static SmallBulkEntry[] LoadEntries( string path )
		{
			path = Path.Combine( Core.BaseDirectory, path );

			List<SmallBulkEntry> list = new List<SmallBulkEntry>();

			if ( File.Exists( path ) )
			{
				using ( StreamReader ip = new StreamReader( path ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						if ( line.Length == 0 || line.StartsWith( "#" ) )
							continue;

						try
						{
							string[] split = line.Split( '\t' );

							if ( split.Length >= 2 )
							{
								Type type = ScriptCompiler.FindTypeByName( split[0] );
								int graphic = Utility.ToInt32( split[split.Length - 1] );

								if ( type != null && graphic > 0 )
                                    list.Add(new SmallBulkEntry(type, graphic < 0x4000 ? 1020000 + graphic : 1078872 + graphic, graphic));
                            }
						}
						catch
						{
						}
					}
				}
			}

			return list.ToArray();
		}
	}
}