using System;
using System.IO;
using System.Xml;
using Server.Logging;

namespace Server.Engines.Reports
{
	public abstract class PersistanceWriter
	{
		public abstract void SetInt32( string key, int value );
		public abstract void SetBoolean( string key, bool value );
		public abstract void SetString( string key, string value );
		public abstract void SetDateTime( string key, DateTime value );

		public abstract void BeginObject( PersistableType typeID );
		public abstract void BeginChildren();
		public abstract void FinishChildren();
		public abstract void FinishObject();

		public abstract void WriteDocument( PersistableObject root );
		public abstract void Close();
	}

	public sealed class XmlPersistanceWriter : PersistanceWriter
	{
		private readonly string m_RealFilePath;
		private readonly string m_TempFilePath;

		private readonly StreamWriter m_Writer;
		private readonly XmlTextWriter m_Xml;
		private readonly string m_Title;

		public XmlPersistanceWriter( string filePath, string title )
		{
			m_RealFilePath = filePath;
			m_TempFilePath = Path.ChangeExtension( filePath, ".tmp" );

			m_Writer = new StreamWriter( m_TempFilePath );
			m_Xml = new XmlTextWriter( m_Writer );

			m_Title = title;
		}

		public override void SetInt32( string key, int value )
		{
			m_Xml.WriteAttributeString( key, XmlConvert.ToString( value ) );
		}

		public override void SetBoolean( string key, bool value )
		{
			m_Xml.WriteAttributeString( key, XmlConvert.ToString( value ) );
		}

		public override void SetString( string key, string value )
		{
			if ( value != null )
				m_Xml.WriteAttributeString( key, value );
		}

		public override void SetDateTime( string key, DateTime value )
		{
			if ( value != DateTime.MinValue )
				m_Xml.WriteAttributeString( key, XmlConvert.ToString( value, XmlDateTimeSerializationMode.Local ) );
		}

		public override void BeginObject( PersistableType typeID )
		{
			m_Xml.WriteStartElement( typeID.Name );
		}

		public override void BeginChildren()
		{
		}

		public override void FinishChildren()
		{
		}

		public override void FinishObject()
		{
			m_Xml.WriteEndElement();
		}

		public override void WriteDocument( PersistableObject root )
		{
			ConsoleLog.Write.Information($"Reports: {m_Title}: Save started" );

			m_Xml.Formatting = Formatting.Indented;
			m_Xml.IndentChar = '\t';
			m_Xml.Indentation = 1;

			m_Xml.WriteStartDocument( true );

			root.Serialize( this );

			ConsoleLog.Write.Information($"Reports: {m_Title}: Save complete" );
		}

		public override void Close()
		{
			m_Xml.Close();
			m_Writer.Close();

			try
			{
				string renamed = null;

				if ( File.Exists( m_RealFilePath ) )
				{
					renamed = Path.ChangeExtension( m_RealFilePath, ".rem" );
					File.Move( m_RealFilePath, renamed );
					File.Move( m_TempFilePath, m_RealFilePath );
					File.Delete( renamed );
				}
				else
				{
					File.Move( m_TempFilePath, m_RealFilePath );
				}
			}
			catch ( Exception ex )
			{
				ConsoleLog.Write.Warning("Close Ex", ex );
			}
		}
	}
}