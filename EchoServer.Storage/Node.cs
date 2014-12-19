using System;
using System.Data.SqlClient;

namespace EchoServer.Storage
{
	public class Node
	{
		public long nodeId { get; set; }
		public string name { get; set; }
		public long parentNodeId { get; set; }
		public long nv { get; set; }
		public long dv { get; set; }
		public long snv { get; set; }
		public long sdv { get; set; }
		public int depth { get; set; }

		public static Node FromReader(SqlDataReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException();
			}

			var node = new Node()
			{
				nodeId = reader.GetInt64(reader.GetOrdinal("node_id")),
				parentNodeId = reader.GetInt64(reader.GetOrdinal("parent_node_id")),
				name = reader.GetString(reader.GetOrdinal("name")),
				nv = reader.GetInt64(reader.GetOrdinal("nv")),
				dv = reader.GetInt64(reader.GetOrdinal("dv")),
				snv = reader.GetInt64(reader.GetOrdinal("snv")),
				sdv = reader.GetInt64(reader.GetOrdinal("sdv")),
				depth = reader.GetInt32(reader.GetOrdinal("depth")),
			};

			return node;
		}
	}
}