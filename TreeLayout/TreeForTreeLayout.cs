using System.Collections.Generic;

namespace Tree
{
	public interface TreeForTreeLayout
	{
		ITreeNode getRoot();
		bool isLeaf(ITreeNode node);
		bool isChildOfParent(ITreeNode node, ITreeNode parentNode);
		List<ITreeNode> getChildren(ITreeNode parentNode);
		List<ITreeNode> getChildrenReverse(ITreeNode parentNode);
		ITreeNode getFirstChild(ITreeNode parentNode);
		ITreeNode getLastChild(ITreeNode parentNode);
	}
}
