using EchoServer.Storage;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Tree;

namespace EchoServer
{
	public class ChatHub : Hub
	{
		public static int MaxNodeCount = 512;
		private static string _connStr { get { return ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString; } }

		public void Hello()
		{
			Clients.All.hello();
		}

		public void Send(string name, string message)
		{
			if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(message))
			{
				return;
			}

			// Call the broadcastMessage method to update clients.
			Clients.All.onMessageReceived(name, message);
		}

		public void GetTree()
		{
			//var nodes = ReadTreeAsync().Result;
			var nodeStorage = new NodeStorage(_connStr, new ChatHubNodeEventHandler());
			var tree = nodeStorage.ReadTree();
			Clients.Caller.onTreeReceived(tree);
		}

		public void SetMaxNodeCount(string name, int maxNodeCount)
		{
			if (String.IsNullOrWhiteSpace(name) || maxNodeCount < 1)
			{
				return;
			}

			MaxNodeCount = maxNodeCount;
			SendSystemMessage("MaxNodeCount set to " + MaxNodeCount + " by " + name);
		}

		public void FancyTree()
		{
			var nodeStorage = new NodeStorage(_connStr, new ChatHubNodeEventHandler());
			var tree = nodeStorage.ReadTree();

			var boxMap = new Dictionary<long, TextInBox>();

			var root = new TextInBox(0, "0", 40, 20);
			boxMap[0] = root;
			var defaultTreeForTreeLayout = new DefaultTreeForTreeLayout(root);

			foreach (var node in tree.nodes)
			{
				var tib = new TextInBox(node.nodeId, node.nodeId.ToString(), 40, 20);
				boxMap[node.nodeId] = tib;
				defaultTreeForTreeLayout.addChild(boxMap[node.parentNodeId], tib);
			}

			// setup the tree layout configuration
			var configuration = new DefaultConfiguration();

			// create the layout
			var treeLayout = new TreeLayout(defaultTreeForTreeLayout, configuration);

			var svgTree = new SVGForTextInBoxTree(treeLayout);
			var xmlDoc = svgTree.GetSvg();
			xmlDoc.Save("F:/Projects/EchoServer/EchoServer/tree.html");

			// generate the edges and boxes (with text)
			/*var nodeBounds = treeLayout.getNodeBounds();
			foreach (var kvp in nodeBounds)
			{
				var nodeRect = nodeBounds[kvp.Key];
				nodeRect.getCenterX();
			}*/
		}

		internal static void SendSystemMessage(string message)
		{
			IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
			context.Clients.All.onMessageReceived("SYSTEM", message);
		}

		internal static void AddOrRemoveRandomNode()
		{
			var nodeStorage = new NodeStorage(_connStr, new ChatHubNodeEventHandler());
			nodeStorage.AddOrRemoveRandomNode();
		}

		private class ChatHubNodeEventHandler : INodeEventHandler
		{
			public void AddNode(Node node)
			{
				if (node == null)
				{
					return;
				}

				IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
				context.Clients.All.onNewNode(node);
			}

			public void DeleteNode(long deletedNodeId)
			{
				IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
				context.Clients.All.onDeleteNode(deletedNodeId);
			}
		}
	}
}
