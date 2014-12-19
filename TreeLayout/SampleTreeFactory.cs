using System;

namespace Tree
{
	public static class SampleTreeFactory
	{
		public static TreeForTreeLayout createSampleTree()
		{
			TextInBox root = new TextInBox(0, "root", 40, 20);
			TextInBox n1 = new TextInBox(1, "n1", 30, 20);
			TextInBox n1_1 = new TextInBox(2, "n1.1", 80, 36);
			TextInBox n1_2 = new TextInBox(3, "n1.2", 40, 20);
			TextInBox n1_3 = new TextInBox(4, "n1.3", 80, 36);
			TextInBox n2 = new TextInBox(5, "n2", 30, 20);
			TextInBox n2_1 = new TextInBox(6, "n2.1", 30, 20);
			TextInBox n2_2 = new TextInBox(7, "n2.2", 30, 20);
			TextInBox n3 = new TextInBox(8, "n3", 30, 20);

			var tree = new DefaultTreeForTreeLayout(root);
			tree.addChild(root, n1);
			tree.addChild(n1, n1_1);
			tree.addChild(n1, n1_2);
			tree.addChild(n1, n1_3);
			tree.addChild(root, n2);
			tree.addChild(n2, n2_1);
			tree.addChild(n2, n2_2);
			tree.addChild(root, n3);

			return tree;
		}

		public static TreeForTreeLayout createSampleTree2()
		{
			TextInBox root = new TextInBox(0, "prog", 40, 20);
			TextInBox n1 = new TextInBox(1, "classDef", 65, 20);
			TextInBox n1_1 = new TextInBox(2, "class", 50, 20);
			TextInBox n1_2 = new TextInBox(3, "T", 20, 20);
			TextInBox n1_3 = new TextInBox(4, "{", 20, 20);
			TextInBox n1_4 = new TextInBox(5, "member", 60, 20);
			TextInBox n1_5 = new TextInBox(6, "member", 60, 20);
			TextInBox n1_5_1 = new TextInBox(7, "<ERROR:int>", 90, 20);
			TextInBox n1_6 = new TextInBox(8, "member", 60, 20);
			TextInBox n1_6_1 = new TextInBox(9, "int", 30, 20);
			TextInBox n1_6_2 = new TextInBox(10, "i", 20, 20);
			TextInBox n1_6_3 = new TextInBox(11, ";", 20, 20);
			TextInBox n1_7 = new TextInBox(12, "}", 20, 20);

			var tree = new DefaultTreeForTreeLayout(root);
			tree.addChild(root, n1);
			tree.addChild(n1, n1_1);
			tree.addChild(n1, n1_2);
			tree.addChild(n1, n1_3);
			tree.addChild(n1, n1_4);
			tree.addChild(n1, n1_5);
			tree.addChild(n1_5, n1_5_1);
			tree.addChild(n1, n1_6);
			tree.addChild(n1_6, n1_6_1);
			tree.addChild(n1_6, n1_6_2);
			tree.addChild(n1_6, n1_6_3);
			tree.addChild(n1, n1_7);
			return tree;
		}

		public static TreeForTreeLayout createSampleTree3()
		{
			TextInBox root = new TextInBox(0, "root", 40, 20);
			TextInBox n1 = new TextInBox(1, "n1", 30, 20);
			TextInBox n1_1 = new TextInBox(2, "n1.1", 40, 20);
			TextInBox n1_2 = new TextInBox(3, "n1.2", 40, 20);
			TextInBox n1_3 = new TextInBox(4, "n1.3", 40, 20);
			TextInBox n2 = new TextInBox(5, "n2", 30, 20);
			TextInBox n2_1 = new TextInBox(6, "n2.1", 40, 20);
			TextInBox n2_2 = new TextInBox(7, "n2.2", 40, 20);
			TextInBox n3 = new TextInBox(8, "n3", 30, 20);

			var tree = new DefaultTreeForTreeLayout(root);
			tree.addChild(root, n1);
			tree.addChild(n1, n1_1);
			tree.addChild(n1, n1_2);
			tree.addChild(n1, n1_3);
			tree.addChild(root, n2);
			tree.addChild(n2, n2_1);
			tree.addChild(root, n3);
			return tree;
		}
	}
}
