using System;

namespace Spikes.Model
{
	public class Test
	{
		protected Test()
		{
		}

		public Test(string name)
		{
			this.name = name;
		}

#pragma warning disable 649
		private Guid id;
#pragma warning restore 649

		public virtual Guid Id
		{
			get { return id; }
		}

#pragma warning disable 649
		private int version;
#pragma warning restore 649

		public virtual int Version
		{
			get { return version; }
		}

		private string name;

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}