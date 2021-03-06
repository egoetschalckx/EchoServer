﻿namespace Tree
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
		bool ExecuteShiftsInFirstWalk { get; set; }
		Location RootLocation { get; set; }
		AlignmentInLevel AlignmentInLevel { get; set; }
		double GapBetweenAdjacentLevels { get; set; }
		double GapBetweenAdjacentNodes { get; set; }
		double GetGapBetweenLevels(int nextLevel);
		double GetGapBetweenNodes(ITreeNode x, ITreeNode y);
	}
}
