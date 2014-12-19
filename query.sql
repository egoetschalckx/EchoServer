EXECUTE ResetTree

SELECT * FROM Node

DECLARE @newNodeId bigint
EXECUTE InsertRandomNode @newNodeId output