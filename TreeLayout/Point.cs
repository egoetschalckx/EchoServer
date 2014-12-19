using System;

namespace Tree
{
	public interface IPoint
	{
		double X { get; }
		double Y { get; }
	}

	public class Point : IPoint
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Point() { }

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
	}
}
