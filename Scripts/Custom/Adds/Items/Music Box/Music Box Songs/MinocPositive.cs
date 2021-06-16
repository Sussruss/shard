namespace Server.Items.MusicBox
{
    public class MinocPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public MinocPositiveSong()
            : base(1075150)
        {
            Song = MusicName.Minoc;
            //Name = "Minoc (Positive)";
        }

        public MinocPositiveSong(Serial s)
            : base(s)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}


