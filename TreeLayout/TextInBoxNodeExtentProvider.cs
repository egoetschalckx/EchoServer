using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
	public class TextInBoxNodeExtentProvider : NodeExtentProvider
	{
		public double getWidth(ITreeNode treeNode)
		{
			return treeNode.Width;
		}

		public double getHeight(ITreeNode treeNode)
		{
			return treeNode.Height;
		}
	}
}
