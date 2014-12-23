	use [EchoServer]
go

	EXECUTE [ResetTree]

	DECLARE @newNodeId1 BIGINT
	DECLARE @newNodeId17 BIGINT
	DECLARE @newNodeId4 BIGINT
	DECLARE @newNodeId6 BIGINT
	DECLARE @newNodeId61 BIGINT

	EXECUTE [InsertNode] 0, 'n1', @newNodeId1 OUTPUT
	EXECUTE [InsertNode] 0, 'n2'
	EXECUTE [InsertNode] 0, 'n3'
	EXECUTE [InsertNode] 0, 'n4', @newNodeId4 OUTPUT
	EXECUTE [InsertNode] 0, 'n5'
	EXECUTE [InsertNode] 0, 'n6', @newNodeId6 OUTPUT

	EXECUTE [InsertNode] @newNodeId1, 'n1_1'
	EXECUTE [InsertNode] @newNodeId1, 'n1_2'
	EXECUTE [InsertNode] @newNodeId1, 'n1_3'
	EXECUTE [InsertNode] @newNodeId1, 'n1_4'
	EXECUTE [InsertNode] @newNodeId1, 'n1_5'
	EXECUTE [InsertNode] @newNodeId1, 'n1_6'
	EXECUTE [InsertNode] @newNodeId1, 'n1_7', @newNodeId17 OUTPUT

	EXECUTE [InsertNode] @newNodeId17, 'n2_1'

	execute [InsertNode] @newNodeId4, 'n4_1'

	execute [InsertNode] @newNodeId6, 'n6_1', @newNodeId61 OUTPUT

	EXECUTE [InsertNode] @newNodeId61, 'n6_1_1'
	EXECUTE [InsertNode] @newNodeId61, 'n6_1_2'
	EXECUTE [InsertNode] @newNodeId61, 'n6_1_3'
	EXECUTE [InsertNode] @newNodeId61, 'n6_1_4'
	EXECUTE [InsertNode] @newNodeId61, 'n6_1_5'
	EXECUTE [InsertNode] @newNodeId61, 'n6_1_6'
	EXECUTE [InsertNode] @newNodeId61, 'n6_1_7'

	SELECT * FROM [GetDescendants] (0, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC