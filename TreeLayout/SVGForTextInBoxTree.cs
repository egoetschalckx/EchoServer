using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Tree
{
	public class SVGForTextInBoxTree
	{
		private readonly TreeLayout treeLayout;

		private TreeForTreeLayout getTree()
		{
			return treeLayout.getTree();
		}

		private List<ITreeNode> getChildren(TextInBox parent)
		{
			return getTree().getChildren(parent);
		}

		private Rectangle getBoundsOfNode(TextInBox node)
		{
			return treeLayout.getNodeBounds()[node];
		}

		public SVGForTextInBoxTree(TreeLayout treeLayout)
		{
			this.treeLayout = treeLayout;
		}

		private void generateEdges(XmlDocument doc, XmlElement svgNode, TextInBox parent)
		{
			if (!getTree().isLeaf(parent))
			{
				var b1 = getBoundsOfNode(parent);
				double x1 = b1.getCenterX();
				double y1 = b1.getCenterY();

				foreach (var child in getChildren(parent))
				{
					var childBox = (TextInBox)child;
					var b2 = getBoundsOfNode(childBox);

					var line = new Line(x1, y1, b2.getCenterX(), b2.getCenterY());

					//result.Append(line(x1, y1, b2.getCenterX(), b2.getCenterY(), "stroke:black; stroke-width:2px;"));
					//result.AppendFormat("line({0}, {1}, {2}, {3})\n", x1, y1, b2.getCenterX(), b2.getCenterY());
					//<line x1="0" y1="0" x2="200" y2="200" style="stroke:rgb(255,0,0);stroke-width:2" />

					var lineNode = doc.CreateElement("line");
					lineNode.SetAttribute("x1", x1.ToString());
					lineNode.SetAttribute("y1", y1.ToString());
					lineNode.SetAttribute("x2", b2.getCenterX().ToString());
					lineNode.SetAttribute("y2", b2.getCenterY().ToString());
					//lineNode.SetAttribute("stroke-color", "black");
					//lineNode.SetAttribute("stroke-width", "2");
					lineNode.SetAttribute("style", "stroke: rgb(0,0,0); stroke-width: 1;");
					svgNode.AppendChild(lineNode);

					// recursion
					generateEdges(doc, svgNode, childBox);
				}
			}
		}

		private void generateBox(XmlDocument doc, XmlElement svgNode, TextInBox textInBox)
		{
			// draw the box in the background
			var box = getBoundsOfNode(textInBox);
			var adjustedBox = new Rectangle(box.X + 1, box.Y + 1, box.Width - 2, box.Height - 2);

			//result.Append(rect(box.X + 1, box.Y + 1, box.getWidth() - 2, box.getHeight() - 2, "fill:orange; stroke:rgb(0,0,0);", "rx=\"10\""));
			//result.AppendFormat("rect({0}, {1}, {2}, {3})\n", svgRect.X, svgRect.Y, svgRect.Width, svgRect.Height);

			var boxNode = doc.CreateElement("rect");
			//<rect id="oRect" x="50" y="100" width="100" height="100" fill="red" stroke="black" stroke-width="2" />
			boxNode.SetAttribute("x", adjustedBox.X.ToString());
			boxNode.SetAttribute("y", adjustedBox.Y.ToString());
			boxNode.SetAttribute("width", adjustedBox.Width.ToString());
			boxNode.SetAttribute("height", adjustedBox.Height.ToString());

			boxNode.SetAttribute("fill", "white");
			boxNode.SetAttribute("stroke", "black");
			//boxNode.SetAttribute("stroke-width", "2");
			svgNode.AppendChild(boxNode);

			// draw the text on top of the box (possibly multiple lines)
			var lines = textInBox.text.Split(new char[] { '\n' });
			int fontSize = 12;
			//int x = (int)adjustedBox.X + fontSize / 2 + 2;
			//int y = (int)adjustedBox.Y + fontSize + 1;

			int x = (int)(adjustedBox.X + (adjustedBox.Width / 2));
			int y = (int)(adjustedBox.Y + (adjustedBox.Height / 2) + 5);

			var style = String.Format("font-family: sans-serif; font-size: {0}px; text-anchor=middle;", fontSize);
			for (int i = 0; i < lines.Length; i++)
			{
				//<text x="200" y="80" font-family="Verdana" font-size="25" fill="blue" stroke="yellow" stroke-width="0.5" text-anchor="middle">MSDN Magazine</text>
				var textNode = doc.CreateElement("text");
				textNode.SetAttribute("x", x.ToString());
				textNode.SetAttribute("y", y.ToString());
				//textNode.SetAttribute("style", style);
				textNode.SetAttribute("text-anchor", "middle");
				textNode.InnerText = lines[i];
				svgNode.AppendChild(textNode);

				/*var txtBlipNode = doc.CreateElement("rect");
				txtBlipNode.SetAttribute("x", x.ToString());
				txtBlipNode.SetAttribute("y", y.ToString());
				txtBlipNode.SetAttribute("width", "1");
				txtBlipNode.SetAttribute("height", "1");

				txtBlipNode.SetAttribute("fill", "black");
				txtBlipNode.SetAttribute("stroke", "black");
				svgNode.AppendChild(txtBlipNode);*/

				y += fontSize;
			}
		}

		public XmlDocument GetHtml()
		{
			StringBuilder result = new StringBuilder();

			XmlDocument doc = new XmlDocument();
			var htmlNode = doc.CreateElement("html");
			var bodyNode = doc.CreateElement("body");
			var svgNode = doc.CreateElement("svg");

			doc.AppendChild(htmlNode).AppendChild(bodyNode).AppendChild(svgNode);

			FillSvg(doc, svgNode);

			return doc;
		}

		public XmlDocument GetSvg()
		{
			StringBuilder result = new StringBuilder();

			XmlDocument doc = new XmlDocument();
			var svgNode = doc.CreateElement("svg");

			doc.AppendChild(svgNode);

			FillSvg(doc, svgNode);

			return doc;
		}

		public void FillSvg(XmlDocument doc, XmlElement svgNode)
		{
			StringBuilder result = new StringBuilder();

			generateEdges(doc, svgNode, (TextInBox)getTree().getRoot());
			foreach (var kvp in treeLayout.getNodeBounds())
			{
				generateBox(doc, svgNode, (TextInBox)kvp.Key);
			}

			var size = treeLayout.getBounds().getBounds().getSize();
			var svg = result.ToString();

			svgNode.SetAttribute("width", size.width + "px");
			svgNode.SetAttribute("height", size.height + "px");
		}
	}
}
