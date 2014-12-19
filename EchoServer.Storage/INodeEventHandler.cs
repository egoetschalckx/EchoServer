using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer.Storage
{
	public interface INodeEventHandler
	{
		void AddNode(Node node);
		void DeleteNode(long nodeId);
	}
}
