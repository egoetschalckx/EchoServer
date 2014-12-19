using System;

namespace Tree
{
	public class Rectangle
	{
		public double Width { get; set; }
		public double Height { get; set; }
		public double X { get; set; }
		public double Y { get; set; }

		public Rectangle() { }

		public Rectangle(double x, double y, double width, double height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public double getCenterX()
		{
			return X + Width / 2.0;
		}

		public double getCenterY()
		{
			return Y + Height / 2.0;
		}

		public Rectangle getBounds()
		{
			if (Width < 0 || Height < 0)
			{
				return new Rectangle();
			}

			double x1 = Math.Floor(X);
			double y1 = Math.Floor(Y);
			double x2 = Math.Ceiling(X + Width);
			double y2 = Math.Ceiling(Y + Height);
			return new Rectangle(x1, y1, x2 - x1, y2 - y1);
		}

		public Dimension getSize()
		{
			return new Dimension(Width, Height);
		}
	}
}
