using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
	public class Dimension
	{
		public double width { get; set; }
		public double height { get; set; }

		public Dimension(double width, double height)
		{
			this.width = width;
			this.height = height;
		}
	}
}
