using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Tree
{
	public class TreeLayout
	{
		private readonly TreeForTreeLayout tree;

		private double boundsLeft = double.MaxValue;
		private double boundsRight = double.MinValue;
		private double boundsTop = double.MaxValue;
		private double boundsBottom = double.MinValue;

		private readonly Dictionary<ITreeNode, double> mod = new Dictionary<ITreeNode, double>();
		private readonly Dictionary<ITreeNode, ITreeNode> thread = new Dictionary<ITreeNode, ITreeNode>();
		private readonly Dictionary<ITreeNode, double> prelim = new Dictionary<ITreeNode, double>();
		private readonly Dictionary<ITreeNode, double> change = new Dictionary<ITreeNode, double>();
		private readonly Dictionary<ITreeNode, double> shift = new Dictionary<ITreeNode, double>();
		private readonly Dictionary<ITreeNode, ITreeNode> ancestors = new Dictionary<ITreeNode, ITreeNode>();
		private readonly Dictionary<ITreeNode, int> number = new Dictionary<ITreeNode, int>();
		private readonly Dictionary<ITreeNode, IPoint> positions = new Dictionary<ITreeNode, IPoint>();

		public readonly List<long> FirstWalkVisitedNodes = new List<long>();
		public readonly List<long> SecondWalkVisitedNodes = new List<long>();

		private ITreeNode NullAncestor { get; set; }
		private double NullMod { get; set; }
		private ITreeNode NullThread { get; set; }

		public TreeForTreeLayout getTree()
		{
			return tree;
		}

		private double getWidthOrHeightOfNode(ITreeNode treeNode, bool returnWidth)
		{
			return returnWidth ? treeNode.Width : treeNode.Height;
		}

		private double getNodeThickness(ITreeNode treeNode)
		{
			return getWidthOrHeightOfNode(treeNode, !isLevelChangeInYAxis());
		}

		private double getNodeSize(ITreeNode treeNode)
		{
			return getWidthOrHeightOfNode(treeNode, isLevelChangeInYAxis());
		}

		private readonly Configuration configuration;

		public Configuration getConfiguration()
		{
			return configuration;
		}

		private bool isLevelChangeInYAxis()
		{
			Location rootLocation = configuration.RootLocation;
			return rootLocation == Location.Top || rootLocation == Location.Bottom;
		}

		private int getLevelChangeSign()
		{
			Location rootLocation = configuration.RootLocation;
			return rootLocation == Location.Bottom || rootLocation == Location.Right ? -1 : 1;
		}

		private void updateBounds(ITreeNode node, double centerX, double centerY)
		{
			double left = centerX - node.Width / 2;
			double right = centerX + node.Width / 2;
			double top = centerY - node.Height / 2;
			double bottom = centerY + node.Height / 2;

			if (boundsLeft > left)
			{
				boundsLeft = left;
			}

			if (boundsRight < right)
			{
				boundsRight = right;
			}

			if (boundsTop > top)
			{
				boundsTop = top;
			}

			if (boundsBottom < bottom)
			{
				boundsBottom = bottom;
			}
		}

		public Rectangle getBounds()
		{
			return new Rectangle(0, 0, boundsRight - boundsLeft, boundsBottom - boundsTop);
		}

		private readonly List<double> sizeOfLevel = new List<double>();

		private void calcSizeOfLevels(ITreeNode node, int level)
		{
			double oldSize;
			if (sizeOfLevel.Count <= level)
			{
				// new level, init size to 0
				sizeOfLevel.Add(0);
				oldSize = 0;
			}
			else
			{
				oldSize = sizeOfLevel[level];
			}

			double size = getNodeThickness(node);

			if (oldSize < size)
			{
				sizeOfLevel[level] = size;
			}

			if (!tree.isLeaf(node))
			{
				foreach (var child in tree.getChildren(node))
				{
					calcSizeOfLevels(child, level + 1);
				}
			}
		}

		public int getLevelCount()
		{
			return sizeOfLevel.Count;
		}

		public double getSizeOfLevel(int level)
		{
			//checkArg(level >= 0, "level must be >= 0");
			//checkArg(level < getLevelCount(), "level must be < levelCount");

			return sizeOfLevel[level];
		}

		private class NormalizedPosition : IPoint
		{
			private readonly TreeLayout treeLayout;
			private double x_relativeToRoot;
			private double y_relativeToRoot;

			public NormalizedPosition(TreeLayout treeLayout, double x_relativeToRoot, double y_relativeToRoot)
			{
				this.treeLayout = treeLayout;
				this.x_relativeToRoot = x_relativeToRoot;
				this.y_relativeToRoot = y_relativeToRoot;
			}

			public double X
			{
				get { return x_relativeToRoot - treeLayout.boundsLeft; }
			}

			public double Y
			{
				get { return y_relativeToRoot - treeLayout.boundsTop; }
			}
		}

		private double getMod(ITreeNode node)
		{
			if (node == null)
			{
				return NullMod;
			}

			double nodeMod;
			if (!mod.TryGetValue(node, out nodeMod))
			{
				return 0;
			}

			return nodeMod;
		}

		private void setMod(ITreeNode node, double d)
		{
			if (node == null)
			{
				NullMod = d;
				return;
			}

			mod[node] = d;
		}

		private ITreeNode getThread(ITreeNode node)
		{
			if (node == null)
			{
				return NullThread;
			}

			var nodeThread = default(ITreeNode);
			if (!thread.TryGetValue(node, out nodeThread))
			{
				return default(ITreeNode);
			}

			return nodeThread;
		}

		private void setThread(ITreeNode node, ITreeNode thread)
		{
			if (node == null)
			{
				NullThread = thread;
				return;
			}

			if (thread == null)
			{
				throw new ArgumentNullException("thread");
			}

			this.thread[node] = thread;
		}

		private ITreeNode getAncestor(ITreeNode node)
		{
			if (node == null)
			{
				return NullAncestor != null ? NullAncestor : node;
			}

			var nodeAncestor = default(ITreeNode);
			if (!ancestors.TryGetValue(node, out nodeAncestor))
			{
				return node;
			}

			return nodeAncestor;
		}

		private void setAncestor(ITreeNode node, ITreeNode ancestor)
		{
			if (node == null)
			{
				NullAncestor = ancestor;
				return;
			}

			if (ancestor == null)
			{
				throw new ArgumentNullException("ancestor");
			}

			ancestors[node] = ancestor;
		}

		private double getPrelim(ITreeNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			double nodePrelim;
			if (!prelim.TryGetValue(node, out nodePrelim))
			{
				return 0;
			}

			return nodePrelim;
		}

		private void setPrelim(ITreeNode node, double d)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			prelim[node] = d;
		}

		private double getChange(ITreeNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			double nodeChange;
			if (!change.TryGetValue(node, out nodeChange))
			{
				return 0;
			}

			return nodeChange;
		}

		private void setChange(ITreeNode node, double d)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			change[node] = d;
		}

		private double getShift(ITreeNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			double nodeShift;
			if (!change.TryGetValue(node, out nodeShift))
			{
				return 0;
			}

			return nodeShift;
		}

		private void setShift(ITreeNode node, double d)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			shift[node] = d;
		}

		private double getDistance(ITreeNode v, ITreeNode w)
		{
			double gapBetweenXandY = configuration.GetGapBetweenNodes(v, w);
			double sizeOfNodes = getNodeSize(v) + getNodeSize(w);
			double distance = sizeOfNodes / 2 + gapBetweenXandY;
			return distance;
		}

		private ITreeNode nextLeft(ITreeNode v)
		{
			return tree.isLeaf(v) ? getThread(v) : tree.getFirstChild(v);
		}

		private ITreeNode nextRight(ITreeNode v)
		{
			return tree.isLeaf(v) ? getThread(v) : tree.getLastChild(v);
		}

		private int getNumber(ITreeNode node, ITreeNode parentNode)
		{
			int n;
			if (!number.ContainsKey(node))
			{
				int i = 1;
				foreach (var child in tree.getChildren(parentNode))
				{
					number[child] = i++;
				}
			}

			n = number[node];

			return n;
		}

		private ITreeNode getAncestorOrDefault(ITreeNode vIMinus, ITreeNode v, ITreeNode parentOfV, ITreeNode defaultAncestor)
		{
			ITreeNode ancestor = getAncestor(vIMinus);

			return tree.isChildOfParent(ancestor, parentOfV) ? ancestor : defaultAncestor;
		}

		private void moveSubtree(ITreeNode wMinus, ITreeNode wPlus, ITreeNode parent, double shift)
		{
			int subtrees = getNumber(wPlus, parent) - getNumber(wMinus, parent);
			setChange(wPlus, getChange(wPlus) - shift / subtrees);
			setShift(wPlus, getShift(wPlus) + shift);
			setChange(wMinus, getChange(wMinus) + shift / subtrees);
			setPrelim(wPlus, getPrelim(wPlus) + shift);
			setMod(wPlus, getMod(wPlus) + shift);
		}

		private ITreeNode apportion(ITreeNode v, ITreeNode defaultAncestor, ITreeNode leftSibling, ITreeNode parentOfV)
		{
			ITreeNode w = leftSibling;
			if (w == null)
			{
				// v has no left sibling
				return defaultAncestor;
			}

			// The following variables "v..." are used to traverse the contours to the subtrees
			// "Minus" refers to the left subtree
			// "Plus" to the right subtree
			// "I" refers to the inside contour
			// "O" to the outside contour
			var vOPlus = v;
			var vIPlus = v;
			var vIMinus = w; // leftSibling

			// get leftmost sibling of vIPlus
			// i.e. get the leftmost sibling of v
			// i.e. the leftmost child of the parent of v (which is passed in)
			var vOMinus = tree.getFirstChild(parentOfV);

			double sIPlus = getMod(vIPlus);
			double sOPlus = getMod(vOPlus);
			double sIMinus = getMod(vIMinus);
			double sOMinus = getMod(vOMinus);

			var nextRightVIMinus = nextRight(vIMinus);
			var nextLeftVIPlus = nextLeft(vIPlus);

			while (nextRightVIMinus != null && nextLeftVIPlus != null)
			{
				vIMinus = nextRightVIMinus;
				vIPlus = nextLeftVIPlus;

				var prevVOMinus = vOMinus;
				vOMinus = nextLeft(vOMinus);
				if (vOMinus == null)
				{
					int temp = 42;
				}

				var prevVOPlus = vOPlus;
				vOPlus = nextRight(vOPlus);

				if (vOPlus == null)
				{
					int temp = 42;
				}

				setAncestor(vOPlus, v);

				double shift = (getPrelim(vIMinus) + sIMinus) - (getPrelim(vIPlus) + sIPlus) + getDistance(vIMinus, vIPlus);

				if (shift > 0)
				{
					moveSubtree(getAncestorOrDefault(vIMinus, v, parentOfV, defaultAncestor), v, parentOfV, shift);

					sIPlus = sIPlus + shift;
					sOPlus = sOPlus + shift;
				}

				sIMinus = sIMinus + getMod(vIMinus);
				sIPlus = sIPlus + getMod(vIPlus);
				sOMinus = sOMinus + getMod(vOMinus);
				sOPlus = sOPlus + getMod(vOPlus);

				nextRightVIMinus = nextRight(vIMinus);
				nextLeftVIPlus = nextLeft(vIPlus);
			}

			if (nextRightVIMinus != null && nextRight(vOPlus) == null)
			{
				setThread(vOPlus, nextRightVIMinus);
				setMod(vOPlus, getMod(vOPlus) + sIMinus - sOPlus);
			}

			if (nextLeftVIPlus != null && nextLeft(vOMinus) == null)
			{
				setThread(vOMinus, nextLeftVIPlus);
				setMod(vOMinus, getMod(vOMinus) + sIPlus - sOMinus);
				defaultAncestor = v;
			}

			return defaultAncestor;
		}

		// shifts children around
		private void executeShifts(ITreeNode v)
		{
			double shift = 0;
			double change = 0;

			// hrm...
			// rightmost child moves 0
			var children = tree.getChildrenReverse(v);
			foreach (var child in children)
			{
				change = change + getChange(child);

				var newPrelim = getPrelim(child) + shift;
				var newMod = getMod(child) + shift;

				setPrelim(child, newPrelim);
				setMod(child, newMod);

				shift = shift + getShift(child) + change;
			}
		}

		private void firstWalk(ITreeNode v, ITreeNode leftSibling, bool doShifts)
		{
			FirstWalkVisitedNodes.Add(v.Id);
			//Debug.WriteLine("firstWalk " + v.ToString());

			if (tree.isLeaf(v))
			{
				// leaf = end of branch

				if (leftSibling != null)
				{
					// no left sibling = rightmost sibling
					// move v the requisite distance to the right
					setPrelim(v, getPrelim(leftSibling) + getDistance(v, leftSibling));
				}
			}
			else
			{
				// not a leaf - v has children

				// the starting default is v's first child
				var defaultAncestor = tree.getFirstChild(v);

				var previousChild = default(ITreeNode);

				foreach (var child in tree.getChildren(v))
				{
					// recurse through my children
					firstWalk(child, previousChild, doShifts);

					// pick new default
					defaultAncestor = apportion(child, defaultAncestor, previousChild, v);

					previousChild = child;
				}

				if (doShifts)
				{
					executeShifts(v);
				}

				var firstChild = tree.getFirstChild(v);
				var lastChild = tree.getLastChild(v);

				var midpoint = (getPrelim(firstChild) + getPrelim(lastChild)) / 2.0;

				if (leftSibling != null)
				{
					// v has a left sibling
					setPrelim(v, getPrelim(leftSibling) + getDistance(v, leftSibling));

					setMod(v, getPrelim(v) - midpoint);
				}
				else
				{
					// v has no left sibling
					// v is halfway between its first and last child
					setPrelim(v, midpoint);
				}
			}
		}

		// construct the position from the prelim and the level information
		private void secondWalk(ITreeNode v, double m, int level, double levelStart)
		{
			SecondWalkVisitedNodes.Add(v.Id);
			//Debug.WriteLine("secondWalk " + v.ToString());

			// The rootLocation affects the way how x and y are changed and in what direction
			double levelChangeSign = getLevelChangeSign();
			bool levelChangeOnYAxis = isLevelChangeInYAxis();

			double levelSize = getSizeOfLevel(level);

			double x = getPrelim(v) + m;

			double y;
			switch (configuration.AlignmentInLevel)
			{
				case AlignmentInLevel.Center:
					y = levelStart + levelChangeSign * (levelSize / 2);
					break;
				case AlignmentInLevel.TowardsRoot:
					y = levelStart + levelChangeSign * (getNodeThickness(v) / 2);
					break;
				default: // away from root
					y = levelStart + levelSize - levelChangeSign * (getNodeThickness(v) / 2);
					break;
			}

			if (!levelChangeOnYAxis)
			{
				double t = x;
				x = y;
				y = t;
			}

			// set final position
			positions[v] = new NormalizedPosition(this, x, y);

			// grow tree bounds, if needed
			updateBounds(v, x, y);

			if (!tree.isLeaf(v))
			{
				double nextLevelStart =
					levelStart
					+ (levelSize + configuration.GetGapBetweenLevels(level + 1))
					* levelChangeSign;

				foreach (var child in tree.getChildren(v))
				{
					secondWalk(child, m + getMod(v), level + 1, nextLevelStart);
				}
			}
		}

		private Dictionary<ITreeNode, Rectangle> nodeBounds;

		public Dictionary<ITreeNode, Rectangle> getNodeBounds()
		{
			if (nodeBounds == null)
			{
				nodeBounds = new Dictionary<ITreeNode, Rectangle>();

				foreach (var kvp in positions)
				{
					var node = kvp.Key;
					var pos = kvp.Value;
					double x = pos.X - node.Width / 2;
					double y = pos.Y - node.Height / 2;

					nodeBounds[node] = new Rectangle(x, y, node.Width, node.Height);
				}
			}

			return nodeBounds;
		}

		public TreeLayout(
			TreeForTreeLayout tree,
			Configuration configuration)
		{
			this.tree = tree;
			this.configuration = configuration;

			ITreeNode r = tree.getRoot();

			firstWalk(r, default(ITreeNode), configuration.ExecuteShiftsInFirstWalk);

			calcSizeOfLevels(r, 0);

			secondWalk(r, -getPrelim(r), 0, 0);
		}
	}
}
