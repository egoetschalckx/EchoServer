use [EchoServer]
go

--execute [ResetTree] 
--execute [ResetTree] 10
--execute [ResetTree] 2048
execute [ResetTree] 10000

SELECT * FROM [GetDescendants] (0, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC

SELECT * FROM [GetAncestors] (964, -1)
SELECT * FROM [GetDescendants] (198, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC