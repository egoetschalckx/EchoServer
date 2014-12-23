CREATE PROCEDURE [dbo].[BrokenTree1]
AS
	EXECUTE [ResetTree]

	DECLARE @newNodeId1 BIGINT
	DECLARE @newNodeId2 BIGINT
	EXECUTE [InsertNode] 0, 'n1', @newNodeId1 OUTPUT
	EXECUTE [InsertNode] 0, 'n2', @newNodeId2 OUTPUT
	--EXECUTE [InsertNode] 0, 'n3'

	EXECUTE [InsertNode] @newNodeId1, 'n1_1'
	EXECUTE [InsertNode] @newNodeId1, 'n1_2'

	EXECUTE [InsertNode] @newNodeId2, 'n2_1'
	--EXECUTE [InsertNode] @newNodeId2, 'n2_2'
RETURN 0
