using System;

namespace Server.Engines.Harvest
{
    public class HarvestBank
    {
        private int m_Current;
        private readonly int m_Maximum;
        private DateTime m_NextRespawn;
        private HarvestVein m_Vein, m_DefaultVein;

        readonly HarvestDefinition m_Definition;

        public void ResetRespawnTimer()
        {
            m_NextRespawn = DateTime.UtcNow;
        }

        public HarvestDefinition Definition => m_Definition;

        public int Current
        {
            get
            {
                CheckRespawn();
                return m_Current;
            }
        }

        public HarvestVein Vein
        {
            get
            {
                CheckRespawn();
                return m_Vein;
            }
            set => m_Vein = m_DefaultVein = value;
        }

        public HarvestVein DefaultVein
        {
            get
            {
                CheckRespawn();
                return m_DefaultVein;
            }
        }

        public void CheckRespawn()
        {
            // Malik's random veins fix
            if (m_NextRespawn > DateTime.UtcNow)
                return;
            if (m_Current == 0)
            {
                m_Vein = m_Definition.GetVeinFrom(Utility.RandomDouble());
                m_Current = m_Maximum;
            }

            /*
			if ( m_Definition.RandomizeVeins )
			{
				m_DefaultVein = m_Definition.GetVeinFrom( Utility.RandomDouble() );
			}

			m_Vein = m_DefaultVein;
            */
        }

        public void Consume(HarvestDefinition def, int amount, Mobile from)
        {
            CheckRespawn();

            if (m_Current == m_Maximum)
            {
                double min = def.MinRespawn.TotalMinutes;
                double max = def.MaxRespawn.TotalMinutes;
                double rnd = Utility.RandomDouble();

                m_Current = m_Maximum - amount;

                double minutes = min + (rnd * (max - min));
                if (def.RaceBonus && from.Race == Race.Elf)	//def.RaceBonus = Core.ML
                    minutes *= .75;	//25% off the time.  

                m_NextRespawn = DateTime.UtcNow + TimeSpan.FromMinutes(minutes);
            }
            else
            {
                m_Current -= amount;
            }

            if (m_Current < 0)
                m_Current = 0;
        }

        public HarvestBank(HarvestDefinition def, HarvestVein defaultVein)
        {
            m_Maximum = Utility.RandomMinMax(def.MinTotal, def.MaxTotal);
            m_Current = m_Maximum;
            m_DefaultVein = defaultVein;
            m_Vein = m_DefaultVein;
            m_Definition = def;
            m_Vein = m_Definition.GetVeinFrom(Utility.RandomDouble());
        }
    }
}