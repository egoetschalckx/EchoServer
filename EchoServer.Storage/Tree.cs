﻿using EchoServer.Storage;
using System;
using System.Collections.Generic;

namespace EchoServer
{
	public class Tree
	{
		public List<Node> nodes { get; private set; }
		public int maxDepth { get; set; }
		public int minDepth { get; set; }

		public Tree()
		{
			nodes = new List<Node>();
		}
	}
}