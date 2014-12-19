using System;
using System.Collections.Generic;
using System.Linq;

namespace Tree
{
	public abstract class AbstractTreeForTreeLayout : TreeForTreeLayout
	{
		abstract public ITreeNode getParent(ITreeNode node);
		abstract public List<ITreeNode> getChildrenList(ITreeNode node);

		private readonly ITreeNode root;

		public AbstractTreeForTreeLayout(ITreeNode root)
		{
			this.root = root;
		}

		public ITreeNode getRoot()
		{
			return root;
		}

		public bool isLeaf(ITreeNode node)
		{
			return getChildrenList(node).Count <= 0;
		}

		public bool isChildOfParent(ITreeNode node, ITreeNode parentNode)
		{
			return getParent(node).Equals(parentNode);
		}

		public List<ITreeNode> getChildren(ITreeNode node)
		{
			return getChildrenList(node);
		}

		public List<ITreeNode> getChildrenReverse(ITreeNode node)
		{
			var children = getChildrenList(node);
			children.Reverse();
			return children;
		}

		public ITreeNode getFirstChild(ITreeNode parentNode)
		{
			return getChildrenList(parentNode).First();
		}

		public ITreeNode getLastChild(ITreeNode parentNode)
		{
			return getChildrenList(parentNode).Last();
		}
	}
}
