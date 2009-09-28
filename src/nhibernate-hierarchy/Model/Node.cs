using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spikes.Model
{
	[DebuggerDisplay("Name = {Name}")]
	public class Node
	{
		public static int Count;

		public Node()
		{
		}

		public Node(string name)
		{
			Count++;
			this.name = name;
		}

		private Guid id;

		public virtual Guid Id
		{
			get { return id; }
		}

		private int version;

		public virtual int Version
		{
			get { return version; }
		}

		private Node root;

		public virtual Node Root
		{
			get { return root; }
			set { root = value; }
		}

		private Node parent;

		public virtual Node Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		private IList<Node> children = new List<Node>();

		public virtual IList<Node> Children
		{
			[DebuggerStepThrough]
			get { return children; }
		}

		public virtual void Add(Node node)
		{
			node.Parent = this;
			children.Add(node);
		}

		private long idx;

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