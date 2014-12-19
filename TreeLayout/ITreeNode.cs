using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
	public interface ITreeNode
	{
		long Id { get; }
		double Width { get; }
		double Height { get; }
	}
}
