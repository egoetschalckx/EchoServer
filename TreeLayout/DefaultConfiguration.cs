using System;

namespace Tree
{
	public class DefaultConfiguration : Configuration
	{
		public bool ExecuteFirstWalk { get; set; }
		public bool ExecuteShiftsInFirstWalk { get; set; }
		public bool ExecuteLevelSizeCalc { get; set; }
		public bool ExecuteSecondWalk { get; set; }
		public double GapBetweenAdjacentLevels { get; set; }
		public double GapBetweenAdjacentNodes { get; set; }
		public Location RootLocation { get; set; }
		public AlignmentInLevel AlignmentInLevel { get; set; }

		public double GetGapBetweenLevels(int nextLevel)
		{
			// TODO: GetGapBetweenLevels needs to actually work
			return GapBetweenAdjacentLevels;
		}

		public double GetGapBetweenNodes(ITreeNode x, ITreeNode y)
		{
			// TODO: GetGapBetweenNodes needs to actually work
			return GapBetweenAdjacentNodes;
		}

		public DefaultConfiguration()
		{
			ExecuteFirstWalk = true;
			ExecuteShiftsInFirstWalk = true;
			ExecuteLevelSizeCalc = true;
			ExecuteSecondWalk = true;

			GapBetweenAdjacentLevels = 20;
			GapBetweenAdjacentNodes = 5;

			AlignmentInLevel = AlignmentInLevel.TowardsRoot;
			RootLocation = Location.Top;
		}
	}
}
