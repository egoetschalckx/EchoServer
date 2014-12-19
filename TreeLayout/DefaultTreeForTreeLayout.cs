using System;
using System.Collections.Generic;

namespace Tree
{
	public class DefaultTreeForTreeLayout : AbstractTreeForTreeLayout
	{
		private readonly List<ITreeNode> emptyList = new List<ITreeNode>();

		private Dictionary<ITreeNode, List<ITreeNode>> childrenMap = new Dictionary<ITreeNode, List<ITreeNode>>();
		private Dictionary<ITreeNode, ITreeNode> parents = new Dictionary<ITreeNode, ITreeNode>();
		private List<ITreeNode> NullChildrenMap { get; set; }

		public DefaultTreeForTreeLayout(ITreeNode root) : base(root)
		{
		}

		public override ITreeNode getParent(ITreeNode node)
		{
			return parents[node];
		}

		public override List<ITreeNode> getChildrenList(ITreeNode node)
		{
			var result = default(List<ITreeNode>);
			if (node == null)
			{
				return NullChildrenMap ?? emptyList;
			}

			if (childrenMap.ContainsKey(node))
			{
				result = childrenMap[node];
			}

			return result ?? emptyList;
		}

		public bool hasNode(ITreeNode node)
		{
			return node.Equals(getRoot()) || parents.ContainsKey(node);
		}

		public void addChild(ITreeNode parentNode, ITreeNode node)
		{
			//checkArg(hasNode(parentNode), "parentNode is not in the tree");
			//checkArg(!hasNode(node), "node is already in the tree");
			List<ITreeNode> list = null;

			if (parentNode == null)
			{
				list = new List<ITreeNode>();
				NullChildrenMap = list;
			}
			else if (!childrenMap.ContainsKey(parentNode))
			{
				list = new List<ITreeNode>();
				childrenMap[parentNode] = list;
			}
			else
			{
				list = childrenMap[parentNode];
			}

			list.Add(node);

			if (node != null)
			{
				parents[node] = parentNode;
			}
		}

		public void addChildren(ITreeNode parentNode, List<ITreeNode> nodes)
		{
			foreach (ITreeNode node in nodes)
			{
				addChild(parentNode, node);
			}
		}
	}
}
