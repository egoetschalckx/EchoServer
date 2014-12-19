using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
	public class FixedNodeExtentProvider : NodeExtentProvider
	{
		public readonly double Width;
		public readonly double Height;

		public FixedNodeExtentProvider(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public FixedNodeExtentProvider()
			: this(0, 0)
		{
		}

		public double getWidth(ITreeNode treeNode)
		{
			return Width;
		}

		public double getHeight(ITreeNode treeNode)
		{
			return Height;
		}
	}
}
