﻿namespace Server.Items
{
	public class HalfEmptyJars : Item
	{
		[Constructable]
		public HalfEmptyJars()
			: base(0xe4c)
		{
			Movable = true;
			Stackable = false;
		}

		public HalfEmptyJars(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class Jars2 : Item
	{
		[Constructable]
		public Jars2()
			: base(0xE4d)
		{
			Movable = true;
			Stackable = false;
		}

		public Jars2(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class Jars3 : Item
	{
		[Constructable]
		public Jars3()
			: base(0xE4e)
		{
			Movable = true;
			Stackable = false;
		}

		public Jars3(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class Jars4 : Item
	{
		[Constructable]
		public Jars4()
			: base(0xE4f)
		{
			Movable = true;
			Stackable = false;
		}

		public Jars4(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}