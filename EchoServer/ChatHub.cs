using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace EchoServer
{
	public class ChatHub : Hub
	{
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
			var tree = ReadTree();
			Clients.Caller.onTreeReceived(tree);
		}

		internal static void SendSystemMessage(string message)
		{
			IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
			context.Clients.All.onMessageReceived("SYSTEM", message);
		}

		internal static void AddOrRemoveRandomNode()
		{
			//return;

			int nodeCount;

			try
			{
				nodeCount = GetNodeCount();
			}
			catch (Exception)
			{
				throw;
			}

			var maxNodeCount = 512;
			if (nodeCount > maxNodeCount)
			{
				// remove a random number of nodes
				var random = new Random();
				var numNewNodes = random.Next(maxNodeCount / 3);

				for (var i = 0; i < numNewNodes; i++)
				{
					try
					{
						DeleteRandomNode();
					}
					catch (Exception)
					{
						throw;
					}
				}
			}
			else
			{
				try
				{
					AddRandomNode();
				}
				catch (Exception)
				{
					throw;
				}
			}
		}

		internal static void AddRandomNode()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString;
			var newNodeId = default(long?);

			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (var insertCmd = new SqlCommand("insertRandomNode", conn))
				{
					insertCmd.CommandType = CommandType.StoredProcedure;
					var newNodeIdParam = new SqlParameter("@newNodeId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
					insertCmd.Parameters.Add(newNodeIdParam);
					insertCmd.ExecuteNonQuery();
					newNodeId = newNodeIdParam.Value as long?;
				}

				if (newNodeId.HasValue)
				{
					var node = ReadNode(newNodeId.Value);
					IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
					context.Clients.All.onNewNode(node);
				}
			}
		}

		internal static Node ReadNode(long nodeId)
		{
			var connectionString = ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString;
			var newNodeId = default(long?);

			using (var conn = new SqlConnection(connectionString))
			using (var readCmd = new SqlCommand("select * from [node] where [node_id] = @nodeId", conn))
			{
				conn.Open();
				readCmd.Parameters.AddWithValue("@nodeId", nodeId);
				using (var reader = readCmd.ExecuteReader())
				{
					reader.Read();
					var node = Node.FromReader(reader);
					return node;
				}
			}
		}

		internal static void DeleteRandomNode()
		{
			var deletedNodeId = default(long?);
			var connectionString = ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString;

			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (var insertCmd = new SqlCommand("deleteRandomNode", conn))
				{
					insertCmd.CommandType = CommandType.StoredProcedure;
					var deletedNodeIdParam = new SqlParameter("@deletedNodeId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
					insertCmd.Parameters.Add(deletedNodeIdParam);
					insertCmd.ExecuteNonQuery();
					deletedNodeId = deletedNodeIdParam.Value as long?;
				}

				if (deletedNodeId.HasValue)
				{
					IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
					context.Clients.All.onDeleteNode(deletedNodeId.Value);
				}
			}
		}

		internal static int GetNodeCount()
		{
			try
			{
				var connectionString = ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString;
				int nodeCount;

				using (var conn = new SqlConnection(connectionString))
				using (var countCmd = new SqlCommand("select count(node_id) as nodeCount from [node]", conn))
				{
					conn.Open();
					using (var reader = countCmd.ExecuteReader())
					{
						reader.Read();
						nodeCount = reader.GetInt32(reader.GetOrdinal("nodeCount"));
						return nodeCount;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected async Task<List<Node>> ReadTreeAsync()
		{
			try
			{
				var sql = String.Format("select * from getDescendants(0, -1)");

				var connectionString = ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString;

				// read tree from db
				using (var conn = new SqlConnection(connectionString))
				using (var cmd = new SqlCommand(sql, conn))
				{
					var nodes = new List<Node>();
					await conn.OpenAsync();
					var reader = await cmd.ExecuteReaderAsync();
					while (await reader.ReadAsync())
					{
						var node = Node.FromReader(reader);
						nodes.Add(node);
					}
					return nodes;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected Tree ReadTree()
		{
			try
			{
				var sql = String.Format("select * from getDescendants(0, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC");
				var connectionString = ConfigurationManager.ConnectionStrings["EchoServer"].ConnectionString;
				var tree = new Tree();
				bool firstNode = true;

				using (var conn = new SqlConnection(connectionString))
				{
					conn.Open();

					using (var cmd = new SqlCommand(sql, conn))
					{
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								var node = Node.FromReader(reader);
								tree.nodes.Add(node);

								if (firstNode)
								{
									tree.maxDepth = node.depth;
									tree.minDepth = node.depth;
									firstNode = false;
								}
								else
								{
									if (node.depth > tree.maxDepth)
									{
										tree.maxDepth = node.depth;
									}

									if (node.depth < tree.minDepth)
									{
										tree.minDepth = node.depth;
									}
								}
							}
						}
					}
				}

				return tree;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
