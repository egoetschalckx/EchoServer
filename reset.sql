use [EchoServer]
go

--EXECUTE [BrokenTree1]
--execute [ResetTree] 5
--execute [ResetTree] 8
--execute [ResetTree] 10
--execute [ResetTree] 12
--execute [ResetTree] 16
--execute [ResetTree] 32
--execute [ResetTree] 64
--execute [ResetTree] 128
--execute [ResetTree] 256
--execute [ResetTree] 512
--execute [ResetTree] 1024
execute [ResetTree] 2048
--execute [ResetTree] 10000

SELECT * FROM [GetDescendants] (0, -1) order by [depth] ASC, [parent_node_id] ASC, (cast([nv] as float) / [dv]) ASC
