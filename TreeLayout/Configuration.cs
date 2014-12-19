namespace Tree
{
	public enum Location
	{
		Top, Left, Bottom, Right
	}

	public enum AlignmentInLevel
	{
		Center, TowardsRoot, AwayFromRoot
	}

	public interface Configuration
	{
		bool ExecuteFirstWalk { get; set; }
		bool ExecuteShiftsInFirstWalk { get; set; }
		bool ExecuteLevelSizeCalc { get; set; }
		bool ExecuteSecondWalk { get; set; }
		Location RootLocation { get; set; }
		AlignmentInLevel AlignmentInLevel { get; set; }
		double GapBetweenAdjacentLevels { get; set; }
		double GapBetweenAdjacentNodes { get; set; }
		double GetGapBetweenLevels(int nextLevel);
		double GetGapBetweenNodes(ITreeNode x, ITreeNode y);
	}
}
