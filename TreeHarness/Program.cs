using EchoServer.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Tree;

namespace TreeHarness
{
	class Program
	{
		private static string _connStr { get { return ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString; } }

		static void Main(string[] args)
		{
			var nodeStorage = new NodeStorage(_connStr, new TreeHarnessNodeEventHandler());
			var nodeCount = nodeStorage.GetNodeCount();

			//makeShiftedSvg(SampleTreeFactory.createSampleTree3());
			//makeUnshiftedSvg(SampleTreeFactory.createSampleTree3());

			makeShiftedSvg(GetFancyTree());
			makeUnshiftedSvg(GetFancyTree());
		}

		static void makeShiftedSvg(TreeForTreeLayout tree)
		{
			var configuration = new DefaultConfiguration()
			{
				ExecuteShiftsInFirstWalk = true
			};

			//var textInBoxNodeExtentProvider = new TextInBoxNodeExtentProvider();
			//var fixedNodeExtentProvider = new FixedNodeExtentProvider(20, 20);

			var stopwatch = Stopwatch.StartNew();
			var treeLayout = new TreeLayout(tree, configuration);
			stopwatch.Stop();
			var timeTreeCalc = stopwatch.ElapsedMilliseconds;

			var svgTree = new SVGForTextInBoxTree(treeLayout);
			var xmlDoc = svgTree.GetHtml();
			xmlDoc.Save("F:/Projects/EchoServer/EchoServer/tree_shifted.html");

			xmlDoc = svgTree.GetSvg();
			xmlDoc.Save("F:/Projects/EchoServer/EchoServer/tree_shifted.svg");
		}

		static void makeUnshiftedSvg(TreeForTreeLayout tree)
		{
			var configuration = new DefaultConfiguration()
			{
				ExecuteShiftsInFirstWalk = false
			};

			var stopwatch = Stopwatch.StartNew();
			var treeLayout = new TreeLayout(tree, configuration);
			stopwatch.Stop();
			var timeTreeCalc = stopwatch.ElapsedMilliseconds;

			var svgTree = new SVGForTextInBoxTree(treeLayout);
			var xmlDoc = svgTree.GetHtml();
			xmlDoc.Save("F:/Projects/EchoServer/EchoServer/tree_unshifted.html");

			var svgDoc = svgTree.GetSvg();
			xmlDoc.Save("F:/Projects/EchoServer/EchoServer/tree_unshifted.svg");
		}

		static TreeForTreeLayout GetFancyTree()
		{
			var nodeStorage = new NodeStorage(_connStr, new TreeHarnessNodeEventHandler());
			var tree = nodeStorage.ReadTree();

			var boxMap = new Dictionary<long, TextInBox>();

			var root = new TextInBox(0, "0", 20, 20);
			boxMap[0] = root;
			var defaultTreeForTreeLayout = new DefaultTreeForTreeLayout(root);

			foreach (var node in tree.nodes)
			{
				var tib = new TextInBox(node.nodeId, node.nodeId.ToString(), 20, 20);
				boxMap[node.nodeId] = tib;
				defaultTreeForTreeLayout.addChild(boxMap[node.parentNodeId], tib);
			}

			return defaultTreeForTreeLayout;
		}

		internal class TreeHarnessNodeEventHandler : INodeEventHandler
		{
			public void AddNode(Node node)
			{
			}

			public void DeleteNode(long deletedNodeId)
			{
			}
		}
	}
}
