using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EchoServer.Storage
{
	public class NodeStorage
	{
		private string _connectionString { get; set; }
		private readonly int _maxNodeCount = 10000;
		private readonly INodeEventHandler _nodeEventHandler;

		public NodeStorage(string connectionString, INodeEventHandler nodeEventHandler)
		{
			if (String.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentException("connectionString must not be null, empty, or whitepace");
			}

			if (nodeEventHandler == null)
			{
				throw new ArgumentNullException("nodeEventHandler");
			}

			_connectionString = connectionString;
			_nodeEventHandler = nodeEventHandler;
		}

		public void AddOrRemoveRandomNode()
		{
			int nodeCount;

			try
			{
				nodeCount = GetNodeCount();
			}
			catch (Exception)
			{
				throw;
			}

			if (nodeCount > _maxNodeCount)
			{
				// remove a "random" number of nodes
				var random = new Random();
				var numNewNodes = random.Next(_maxNodeCount / 3);

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

		public long? AddRandomNode()
		{
			var newNodeId = default(long?);

			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (var insertCmd = new SqlCommand("InsertRandomNode", conn))
				{
					insertCmd.CommandType = CommandType.StoredProcedure;
					var newNodeIdParam = new SqlParameter("@newNodeId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
					insertCmd.Parameters.Add(newNodeIdParam);
					
					insertCmd.ExecuteNonQuery();
					newNodeId = newNodeIdParam.Value as long?;
				}

				if (newNodeId.HasValue)
				{
					_nodeEventHandler.AddNode(ReadNode(newNodeId.Value));
				}

				return newNodeId;
			}
		}

		public Node ReadNode(long nodeId)
		{
			using (var conn = new SqlConnection(_connectionString))
			using (var readCmd = new SqlCommand("select * from [node] where [node_id] = @nodeId", conn))
			{
				conn.Open();
				readCmd.Parameters.AddWithValue("@nodeId", nodeId);
				using (var reader = readCmd.ExecuteReader())
				{
					var node = default(Node);
					if (reader.Read())
					{
						node = Node.FromReader(reader);
					}

					return node;
				}
			}
		}

		public long? DeleteRandomNode()
		{
			var deletedNodeId = default(long?);

			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				var randomNodeId = GetRandomNodeId();

				// read descendants of this random node

				// now delete 'em all
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
					_nodeEventHandler.DeleteNode(deletedNodeId.Value);
				}

				return deletedNodeId;
			}
		}

		public int GetNodeCount()
		{
			int nodeCount;
			try
			{
				using (var conn = new SqlConnection(_connectionString))
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

		public Tree ReadTree()
		{
			try
			{
				var sql = String.Format("select * from getDescendants(0, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC");
				var tree = new Tree();
				bool firstNode = true;

				using (var conn = new SqlConnection(_connectionString))
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

		public async Task<Tree> ReadTreeAsync()
		{
			try
			{
				var sql = String.Format("select * from getDescendants(0, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC");
				var tree = new Tree();
				bool firstNode = true;

				// read tree from db
				using (var conn = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand(sql, conn))
				{
					var nodes = new List<Node>();
					await conn.OpenAsync();
					var reader = await cmd.ExecuteReaderAsync();
					while (await reader.ReadAsync())
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

					return tree;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public long GetRandomNodeId()
		{
			using (var conn = new SqlConnection(_connectionString))
			using (var readCmd = new SqlCommand("SELECT TOP 1 node_id FROM [Node] WHERE [node_id] <> 0 ORDER BY NEWID()", conn))
			{
				conn.Open();
				using (var reader = readCmd.ExecuteReader())
				{
					if (reader.Read())
					{
						var randomNodeId = reader.GetInt64(reader.GetOrdinal("node_id"));

						return randomNodeId;
					}
					else
					{
						return -1;
					}
				}
			}
		}
	}
}
