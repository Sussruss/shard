using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server.Logging;

namespace Server.Accounting
{
    public class Accounts
    {
        private static Dictionary<string, IAccount> m_Accounts = new Dictionary<string, IAccount>();


        public static void Configure()
        {
            EventSink.WorldLoad += Load;
            EventSink.WorldSave += Save;
        }

        public static int Count => m_Accounts.Count;

        public static ICollection<IAccount> GetAccounts()
        {
#if !MONO
            return m_Accounts.Values;
#else
			return new List<IAccount>( m_Accounts.Values );
#endif
        }

        public static IAccount GetAccount(string username)
        {
            IAccount a;

            m_Accounts.TryGetValue(username, out a);

            return a;
        }










































        public static void Add(IAccount a)
        {
            m_Accounts[a.Username] = a;
        }

        public static void Remove(string username)
        {
            m_Accounts.Remove(username);
        }

        public static void Load()
        {
            m_Accounts = new Dictionary<string, IAccount>(32, StringComparer.OrdinalIgnoreCase);

            string filePath = Path.Combine("Saves/Accounts", "accounts.xml");

            if (!File.Exists(filePath))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root = doc["accounts"];

            foreach (XmlElement account in root.GetElementsByTagName("account"))
            {
                try
                {
                    Account acct = new Account(account);
                }
                catch
                {
					ConsoleLog.Write.Warning("Account instance load failed");
                }
            }
        }

        public static void Save(WorldSaveEventArgs e)
        {
            if (!Directory.Exists("Saves/Accounts"))
                Directory.CreateDirectory("Saves/Accounts");

            string filePath = Path.Combine("Saves/Accounts", "accounts.xml");

            using (StreamWriter op = new StreamWriter(filePath))
            {
                XmlTextWriter xml = new XmlTextWriter(op);

                xml.Formatting = Formatting.Indented;
                xml.IndentChar = '\t';
                xml.Indentation = 1;

                xml.WriteStartDocument(true);

                xml.WriteStartElement("accounts");

                xml.WriteAttributeString("count", m_Accounts.Count.ToString());

                foreach (Account a in GetAccounts())
                    a.Save(xml);

                xml.WriteEndElement();

                xml.Close();
            }
        }
    }
}