using System;
using System.Collections.Generic;

namespace Spikes.Model
{
	public class Node
	{
		protected Node()
		{
		}

		public Node(string name)
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

		private Node parent;

		public virtual Node Parent
		{
			get { return parent; }
			set { parent = value; }
		}

#pragma warning disable FieldCouldBeMadeReadOnly
		private IList<Node> children = new List<Node>();
#pragma warning restore FieldCouldBeMadeReadOnly

		public virtual IList<Node> Children
		{
			get { return children; }
		}

		public virtual void Add(Node node)
		{
			node.Parent = this;
			children.Add(node);
		}

#pragma warning disable 649
		private long idx;
#pragma warning restore 649

		public virtual long Idx
		{
			get { return idx; }
		}

		private string name;

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}