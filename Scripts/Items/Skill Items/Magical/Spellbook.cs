using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Engines.Craft;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public enum SpellbookType
    {
        Invalid = -1,
        Regular,
        Necromancer,
        Paladin,
        Ninja,
        Samurai,
        Arcanist
    }

    public enum BookQuality
    {
        Regular,
        Exceptional,
    }

    public class Spellbook : Item, ICraftable, ISlayer
    {
        private string m_EngravedText;
        private BookQuality m_Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get => m_EngravedText;
            set { m_EngravedText = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BookQuality Quality
        {
            get => m_Quality;
            set { m_Quality = value; InvalidateProperties(); }
        }

        public static void Initialize()
        {
            EventSink.OpenSpellbookRequest += EventSink_OpenSpellbookRequest;
            EventSink.CastSpellRequest += EventSink_CastSpellRequest;

            CommandSystem.Register("AllSpells", AccessLevel.GameMaster, AllSpells_OnCommand);
        }

        [Usage("AllSpells")]
        [Description("Completely fills a targeted spellbook with scrolls.")]
        private static void AllSpells_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, AllSpells_OnTarget);
            e.Mobile.SendMessage("Target the spellbook to fill.");
        }

        private static void AllSpells_OnTarget(Mobile from, object obj)
        {
            if (obj is Spellbook)
            {
                Spellbook book = (Spellbook)obj;

                if (book.BookCount == 64)
                    book.Content = ulong.MaxValue;
                else
                    book.Content = (1ul << book.BookCount) - 1;

                from.SendMessage("The spellbook has been filled.");

                CommandLogging.WriteLine(from, "{0} {1} filling spellbook {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(book));
            }
            else
            {
                from.BeginTarget(-1, false, TargetFlags.None, AllSpells_OnTarget);
                from.SendMessage("That is not a spellbook. Try again.");
            }
        }

        private static void EventSink_OpenSpellbookRequest(OpenSpellbookRequestEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!Multis.DesignContext.Check(from))
                return; // They are customizing

            SpellbookType type;

            switch (e.Type)
            {
                default:
                case 1: type = SpellbookType.Regular; break;
                case 2: type = SpellbookType.Necromancer; break;
                case 3: type = SpellbookType.Paladin; break;
                case 4: type = SpellbookType.Ninja; break;
                case 5: type = SpellbookType.Samurai; break;
                case 6: type = SpellbookType.Arcanist; break;
            }

            Spellbook book = Spellbook.Find(from, -1, type);

            if (book != null)
                book.DisplayTo(from);
        }

        private static void EventSink_CastSpellRequest(CastSpellRequestEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!Multis.DesignContext.Check(from))
                return; // They are customizing

            Spellbook book = e.Spellbook as Spellbook;
            int spellID = e.SpellID;

            if (book == null || !book.HasSpell(spellID))
                book = Find(from, spellID);

            if (book != null && book.HasSpell(spellID))
            {
                SpecialMove move = SpellRegistry.GetSpecialMove(spellID);

                if (move != null)
                {
                    SpecialMove.SetCurrentMove(from, move);
                }
                else
                {
                    Spell spell = SpellRegistry.NewSpell(spellID, from, null);

                    if (spell != null)
                        spell.Cast();
                    else
                        from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.
                }
            }
            else
            {
                from.SendLocalizedMessage(500015); // You do not have that spell!
            }
        }

        private static Dictionary<Mobile, List<Spellbook>> m_Table = new Dictionary<Mobile, List<Spellbook>>();

        public static SpellbookType GetTypeForSpell(int spellID)
        {
            if (spellID >= 0 && spellID < 64)
                return SpellbookType.Regular;
            else if (spellID >= 100 && spellID < 117)
                return SpellbookType.Necromancer;
            else if (spellID >= 200 && spellID < 210)
                return SpellbookType.Paladin;
            else if (spellID >= 400 && spellID < 406)
                return SpellbookType.Samurai;
            else if (spellID >= 500 && spellID < 508)
                return SpellbookType.Ninja;
            else if (spellID >= 600 && spellID < 617)
                return SpellbookType.Arcanist;

            return SpellbookType.Invalid;
        }

        public static Spellbook FindRegular(Mobile from)
        {
            return Find(from, -1, SpellbookType.Regular);
        }

        public static Spellbook FindNecromancer(Mobile from)
        {
            return Find(from, -1, SpellbookType.Necromancer);
        }

        public static Spellbook FindPaladin(Mobile from)
        {
            return Find(from, -1, SpellbookType.Paladin);
        }

        public static Spellbook FindSamurai(Mobile from)
        {
            return Find(from, -1, SpellbookType.Samurai);
        }

        public static Spellbook FindNinja(Mobile from)
        {
            return Find(from, -1, SpellbookType.Ninja);
        }

        public static Spellbook FindArcanist(Mobile from)
        {
            return Find(from, -1, SpellbookType.Arcanist);
        }

        public static Spellbook Find(Mobile from, int spellID)
        {
            return Find(from, spellID, GetTypeForSpell(spellID));
        }

        public static Spellbook Find(Mobile from, int spellID, SpellbookType type)
        {
            if (from == null)
                return null;

            if (from.Deleted)
            {
                m_Table.Remove(from);
                return null;
            }

            List<Spellbook> list = null;

            m_Table.TryGetValue(from, out list);

            bool searchAgain = false;

            if (list == null)
                m_Table[from] = list = FindAllSpellbooks(from);
            else
                searchAgain = true;

            Spellbook book = FindSpellbookInList(list, from, spellID, type);

            if (book == null && searchAgain)
            {
                m_Table[from] = list = FindAllSpellbooks(from);

                book = FindSpellbookInList(list, from, spellID, type);
            }

            return book;
        }

        public static Spellbook FindSpellbookInList(List<Spellbook> list, Mobile from, int spellID, SpellbookType type)
        {
            Container pack = from.Backpack;

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (i >= list.Count)
                    continue;

                Spellbook book = list[i];
                if (!book.Deleted && (book.Parent == from || (pack != null && book.Parent == pack)) && ValidateSpellbook(book, spellID, type))
                    return book;

                list.RemoveAt(i);
            }

            return null;
        }

        public static List<Spellbook> FindAllSpellbooks(Mobile from)
        {
            List<Spellbook> list = new List<Spellbook>();

            Item item = from.FindItemOnLayer(Layer.OneHanded);

            if (item is Spellbook)
                list.Add((Spellbook)item);

            Container pack = from.Backpack;

            if (pack == null)
                return list;

            for (int i = 0; i < pack.Items.Count; ++i)
            {
                item = pack.Items[i];

                if (item is Spellbook)
                    list.Add((Spellbook)item);
            }

            return list;
        }

        public static Spellbook FindEquippedSpellbook(Mobile from)
        {
            return (from.FindItemOnLayer(Layer.OneHanded) as Spellbook);
        }

        public static bool ValidateSpellbook(Spellbook book, int spellID, SpellbookType type)
        {
            return (book.SpellbookType == type && (spellID == -1 || book.HasSpell(spellID)));
        }

        public override bool DisplayWeight => false;

        private AosAttributes m_AosAttributes;
        private AosSkillBonuses m_AosSkillBonuses;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get => m_AosAttributes;
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses
        {
            get => m_AosSkillBonuses;
            set { }
        }

        public virtual SpellbookType SpellbookType => SpellbookType.Regular;
        public virtual int BookOffset => 0;
        public virtual int BookCount => 64;

        private ulong m_Content;
        private int m_Count;

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            if (!Ethics.Ethic.CheckTrade(from, to, newOwner, this))
                return false;

            return base.AllowSecureTrade(from, to, newOwner, accepted);
        }

        public override bool CanEquip(Mobile from)
        {
            if (!Ethics.Ethic.CheckEquip(from, this))
            {
                return false;
            }
            else if (!from.CanBeginAction(typeof(BaseWeapon)))
            {
                return false;
            }

            return base.CanEquip(from);
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is SpellScroll && dropped.Amount == 1)
            {
                SpellScroll scroll = (SpellScroll)dropped;

                SpellbookType type = GetTypeForSpell(scroll.SpellID);

                if (type != SpellbookType)
                {
                    return false;
                }
                else if (HasSpell(scroll.SpellID))
                {
                    from.SendLocalizedMessage(500179); // That spell is already present in that spellbook.
                    return false;
                }
                else
                {
                    int val = scroll.SpellID - BookOffset;

                    if (val >= 0 && val < BookCount)
                    {
                        m_Content |= (ulong)1 << val;
                        ++m_Count;

                        InvalidateProperties();

                        scroll.Delete();

                        from.Send(new PlaySound(0x249, GetWorldLocation()));
                        return true;
                    }

                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ulong Content
        {
            get => m_Content;
            set
            {
                if (m_Content != value)
                {
                    m_Content = value;

                    m_Count = 0;

                    while (value > 0)
                    {
                        m_Count += (int)(value & 0x1);
                        value >>= 1;
                    }

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpellCount => m_Count;

        [Constructable]
        public Spellbook()
            : this((ulong)0)
        {
        }

        [Constructable]
        public Spellbook(ulong content)
            : this(content, 0xEFA)
        {
        }

        public Spellbook(ulong content, int itemID)
            : base(itemID)
        {
            m_AosAttributes = new AosAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);

            Weight = 3.0;
            Layer = Layer.OneHanded;
            LootType = LootType.Blessed;

            Content = content;
        }

        public override void OnAdded(object parent)
        {
            if (Core.AOS && parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.AddTo(from);

                int strBonus = m_AosAttributes.BonusStr;
                int dexBonus = m_AosAttributes.BonusDex;
                int intBonus = m_AosAttributes.BonusInt;

                if (strBonus != 0 || dexBonus != 0 || intBonus != 0)
                {
                    string modName = Serial.ToString();

                    if (strBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                    if (dexBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                    if (intBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                }

                from.CheckStatTimers();
            }
        }

        public override void OnRemoved(object parent)
        {
            if (Core.AOS && parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.Remove();

                string modName = Serial.ToString();

                from.RemoveStatMod(modName + "Str");
                from.RemoveStatMod(modName + "Dex");
                from.RemoveStatMod(modName + "Int");

                from.CheckStatTimers();
            }
        }

        public bool HasSpell(int spellID)
        {
            spellID -= BookOffset;

            return (spellID >= 0 && spellID < BookCount && (m_Content & ((ulong)1 << spellID)) != 0);
        }

        public Spellbook(Serial serial)
            : base(serial)
        {
        }

        public void DisplayTo(Mobile to)
        {
            // The client must know about the spellbook or it will crash!

            NetState ns = to.NetState;

            if (ns == null)
                return;

            if (Parent == null)
            {
                to.Send(WorldPacket);
            }
            else if (Parent is Item)
            {
                // What will happen if the client doesn't know about our parent?
                if (ns.ContainerGridLines)
                    to.Send(new ContainerContentUpdate6017(this));
                else
                    to.Send(new ContainerContentUpdate(this));
            }
            else if (Parent is Mobile)
            {
                // What will happen if the client doesn't know about our parent?
                to.Send(new EquipUpdate(this));
            }

            if (ns.HighSeas)
                to.Send(new DisplaySpellbookHS(this));
            else
                to.Send(new DisplaySpellbook(this));

            if (Core.AOS)
            {
                if ( ns.NewSpellbook )
                {
                    to.Send(new NewSpellbookContent(this, ItemID, BookOffset + 1, m_Content));
                }
                else
                {
                    to.Send(new SpellbookContent(m_Count, BookOffset + 1, m_Content, this));
                }
            }
            else
            {
				if ( ns.ContainerGridLines )
                {
                    to.Send(new SpellbookContent6017(m_Count, BookOffset + 1, m_Content, this));
                }
                else
                {
                    to.Send(new SpellbookContent(m_Count, BookOffset + 1, m_Content, this));
                }
            }
        }

        private Mobile m_Crafter;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get => m_Crafter;
            set { m_Crafter = value; InvalidateProperties(); }
        }

        public override bool DisplayLootType => Core.AOS;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Quality == BookQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (m_EngravedText != null)
                list.Add(1072305, m_EngravedText); // Engraved: ~1_INSCRIPTION~

            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.Name); // crafted by ~1_NAME~

            m_AosSkillBonuses.GetProperties(list);

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                    list.Add(entry.Title);
            }

            int prop;

            if ((prop = m_AosAttributes.WeaponDamage) != 0)
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = m_AosAttributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = m_AosAttributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

            if ((prop = m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = m_AosAttributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            list.Add(1042886, m_Count.ToString()); // ~1_NUMBERS_OF_SPELLS~ Spells
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Crafter != null)
                LabelTo(from, 1050043, m_Crafter.Name); // crafted by ~1_NAME~

            LabelTo(from, string.Format("{0} spell{1}", m_Count, m_Count == 1 ? "" : "s"));
            //LabelTo(from, 1042886, m_Count.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            Container pack = from.Backpack;

            if (Parent == from || (pack != null && Parent == pack))
                DisplayTo(from);
            else
                from.SendLocalizedMessage(500207); // The spellbook must be in your backpack (and not in a container within) to open.
        }


        private SlayerName m_Slayer;
        private SlayerName m_Slayer2;
        //Currently though there are no dual slayer spellbooks, OSI has a habit of putting dual slayer stuff in later

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
        {
            get => m_Slayer;
            set { m_Slayer = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get => m_Slayer2;
            set { m_Slayer2 = value; InvalidateProperties(); }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)5); // version

            writer.Write((byte)m_Quality);

            writer.Write((string)m_EngravedText); writer.Write(m_Crafter);

            writer.Write((int)m_Slayer);
            writer.Write((int)m_Slayer2);

            m_AosAttributes.Serialize(writer);
            m_AosSkillBonuses.Serialize(writer);

            writer.Write(m_Content);
            writer.Write(m_Count);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                    {
                        m_Quality = (BookQuality)reader.ReadByte();

                        goto case 4;
                    }
                case 4:
                    {
                        m_EngravedText = reader.ReadString();

                        goto case 3;
                    }
                case 3:
                    {
                        m_Crafter = reader.ReadMobile();
                        goto case 2;
                    }
                case 2:
                    {
                        m_Slayer = (SlayerName)reader.ReadInt();
                        m_Slayer2 = (SlayerName)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_AosAttributes = new AosAttributes(this, reader);
                        m_AosSkillBonuses = new AosSkillBonuses(this, reader);

                        goto case 0;
                    }
                case 0:
                    {
                        m_Content = reader.ReadULong();
                        m_Count = reader.ReadInt();

                        break;
                    }
            }

            if (m_AosAttributes == null)
                m_AosAttributes = new AosAttributes(this);

            if (m_AosSkillBonuses == null)
                m_AosSkillBonuses = new AosSkillBonuses(this);

            if (Core.AOS && Parent is Mobile)
                m_AosSkillBonuses.AddTo((Mobile)Parent);

            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if (Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = (Mobile)Parent;

                string modName = Serial.ToString();

                if (strBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                if (dexBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                if (intBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
            }

            if (Parent is Mobile)
                ((Mobile)Parent).CheckStatTimers();
        }

        private static readonly int[] m_LegendPropertyCounts = new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	// 0 properties : 21/52 : 40%
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,					// 1 property   : 15/52 : 29%
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,									// 2 properties : 10/52 : 19%
				3, 3, 3, 3, 3, 3												// 3 properties :  6/52 : 12%

			};

        private static readonly int[] m_ElderPropertyCounts = new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,// 0 properties : 15/34 : 44%
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 				// 1 property   : 10/34 : 29%
				2, 2, 2, 2, 2, 2,							// 2 properties :  6/34 : 18%
				3, 3, 3										// 3 properties :  3/34 :  9%
			};

        private static readonly int[] m_GrandPropertyCounts = new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	// 0 properties : 10/20 : 50%
				1, 1, 1, 1, 1, 1,				// 1 property   :  6/20 : 30%
				2, 2, 2,						// 2 properties :  3/20 : 15%
				3								// 3 properties :  1/20 :  5%
			};

        private static readonly int[] m_MasterPropertyCounts = new int[]
			{
				0, 0, 0, 0, 0, 0,				// 0 properties : 6/10 : 60%
				1, 1, 1,						// 1 property   : 3/10 : 30%
				2								// 2 properties : 1/10 : 10%
			};

        private static readonly int[] m_AdeptPropertyCounts = new int[]
			{
				0, 0, 0,						// 0 properties : 3/4 : 75%
				1								// 1 property   : 1/4 : 25%
			};

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            int magery = from.Skills.Magery.BaseFixedPoint;

            if (magery >= 800)
            {
                int[] propertyCounts;
                int minIntensity;
                int maxIntensity;

                if (magery >= 1000)
                {
                    if (magery >= 1200)
                        propertyCounts = m_LegendPropertyCounts;
                    else if (magery >= 1100)
                        propertyCounts = m_ElderPropertyCounts;
                    else
                        propertyCounts = m_GrandPropertyCounts;

                    minIntensity = 55;
                    maxIntensity = 75;
                }
                else if (magery >= 900)
                {
                    propertyCounts = m_MasterPropertyCounts;
                    minIntensity = 25;
                    maxIntensity = 45;
                }
                else
                {
                    propertyCounts = m_AdeptPropertyCounts;
                    minIntensity = 0;
                    maxIntensity = 15;
                }

                int propertyCount = propertyCounts[Utility.Random(propertyCounts.Length)];

                BaseRunicTool.ApplyAttributesTo(this, true, 0, propertyCount, minIntensity, maxIntensity);
            }

            if (makersMark)
                Crafter = from;

            m_Quality = (BookQuality)(quality - 1);

            return quality;
        }
    }
}