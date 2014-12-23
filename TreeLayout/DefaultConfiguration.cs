using System;

namespace Tree
{
	public class DefaultConfiguration : Configuration
	{
		public bool ExecuteShiftsInFirstWalk { get; set; }
		public double GapBetweenAdjacentLevels { get; set; }
		public double GapBetweenAdjacentNodes { get; set; }
		public Location RootLocation { get; set; }
		public AlignmentInLevel AlignmentInLevel { get; set; }

		public double GetGapBetweenLevels(int nextLevel)
		{
			return GapBetweenAdjacentLevels;
		}

		public double GetGapBetweenNodes(ITreeNode x, ITreeNode y)
		{
			return GapBetweenAdjacentNodes;
		}

		public DefaultConfiguration()
		{
			ExecuteShiftsInFirstWalk = true;

			GapBetweenAdjacentLevels = 25;
			GapBetweenAdjacentNodes = 10;

			AlignmentInLevel = AlignmentInLevel.TowardsRoot;
			RootLocation = Location.Top;
		}
	}
}
