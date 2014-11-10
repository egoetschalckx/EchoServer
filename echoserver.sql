use [EchoServer]
set nocount on

if object_id('node', 'U') is not null
	drop table node

create table [node] (
	node_id bigint identity(0, 1),
	name nvarchar(64) not null,
	nv bigint not null,
	dv bigint not null,
	snv bigint not null,
	sdv bigint not null,
	depth int not null,
	parent_node_id bigint not null
)
go

-- the first node is 2/1, we will reserve it
insert into [Node] values ('[reserved]', 2, 1, 3, 1, 0, -1)
go

if object_id('insertNode') is null
    exec ('create procedure insertNode as select 1')
go
alter procedure insertNode(
	@parentNodeId bigint,
	@name nvarchar(64),
	@newNodeId bigint = null output
)
as
begin
	set nocount on
	-- we'll sanitize parentNodeId NULL to be same as 0, which is the root/reserved node
	if (@parentNodeId = null)
		set @parentNodeId = 0

	declare @nvp bigint = -1
	declare @dvp bigint
	declare @snvp bigint
	declare @sdvp bigint
	declare @c bigint

	-- Hazel (2008)
	-- Adding nv to snv gives the numerator of the first child
	-- Adding dv to sdv gives the denominator of the first child
	-- given the nv, dv, snv and sdv of a parent node p, we can determine the nv and dv of its cth child as follows:
	-- nvc = nvp + c × snvp (2.1)
	-- dvc = dvp + c × sdvp (2.2)
	-- Since the next sibling of the cth child of node p, is the (c + 1)th child of node p, it follows that
	-- snvc = nvp + (c + 1) × snvp (2.3)
	-- sdvc = dvp + (c + 1) × sdvp (2.4)

	-- read the parent's fractions
	-- todo: what does this do if parent no exist
	select
		@nvp = n.[nv],
		@dvp = n.[dv],
		@snvp = n.[snv],
		@sdvp = n.[sdv]
	from [node] as n
	where n.[node_id] = @parentNodeId

	--print '@nvp = ' + cast(@nvp as nvarchar)
	--print '@dvp = ' + cast(@dvp as nvarchar)
	--print '@snvp = ' + cast(@snvp as nvarchar)
	--print '@sdvp = ' + cast(@sdvp as nvarchar)

	declare @pf float = @nvp / @dvp
	declare @psf float = @snvp / @sdvp

	--print 'nvp / dvp = ' + cast(@pf as nvarchar)
	--print 'snvp / sdvp = ' + cast(@psf as nvarchar)

	-- find the next child (c) to insert under parent
	select @c = COUNT(n.node_id) + 1
	from [node] as n
	where 
		(cast(n.[nv] as float) / n.[dv]) > (cast(@nvp as float) / @dvp) 
		and 
		(cast(n.[nv] as float) / n.[dv]) < (cast(@snvp as float) / @sdvp) 

	--print '@c = ' + cast(@c as nvarchar)

	insert into [Node] 
	select 
		@name,
		p.[nv] + @c * p.[snv], -- nvc
		p.[dv] + @c * p.[sdv], -- dvc
		p.[nv] + (@c + 1) * p.[snv], -- svnc
		p.[dv] + (@c + 1) * p.[sdv], -- sdv
		p.[depth] + 1,
		p.[node_id]
	from [node] as p
	where p.[node_id] = @parentNodeId

	set @newNodeId = scope_identity()
end
go

if object_id('insertRandomNode') is null
    exec ('create procedure insertRandomNode as select 1')
go
alter procedure insertRandomNode(
	@newNodeId bigint output
) 
as
begin
	set nocount on
	declare @name nvarchar(64) = 'eric'
	declare @internalNewNodeId bigint
	declare @parentNodeId bigint

	select @name = cast(crypt_gen_random(8) as nvarchar(64))

	select top 1 @parentNodeId = node_id from [node] order by newid()

	execute insertNode @parentNodeId, @name, @internalNewNodeId output

	set @newNodeId = @internalNewNodeId
end
go

if object_id('deleteNode') is null
    exec ('create procedure deleteNode as select 1')
go
alter procedure deleteNode(
	@nodeId bigint
) 
as
begin
	set nocount on

	delete 
	from [node]
	where [node_id] in (select node_id from getDescendants(@nodeId, -1)) OR [node_id] = @nodeId
end
go

if object_id('deleteRandomNode') is null
    exec ('create procedure deleteRandomNode as select 1')
go
alter procedure deleteRandomNode(
	@deletedNodeId bigint output
) 
as
begin
	set nocount on
	declare @randomNodeId bigint
	declare @internalDeletedNodeId bigint

	select top 1 @randomNodeId = node_id from [node] where [node_id] <> 0 order by newid()

	execute deleteNode @randomNodeId

	set @deletedNodeId = @randomNodeId
end
go

--alter function getDescendants(
alter function getDescendants(
	@parentNodeId bigint,
	@numGenerations int
) returns @childtable table (
	node_id bigint not null,
	name nvarchar(64) not null,
	nv bigint not null, 
	dv bigint not null,
	snv bigint not null,
	sdv bigint not null,
	depth int not null,
	parent_node_id bigint not null)
as
begin
	if @parentNodeId = null
		set @parentNodeId = 0

	declare @nvp bigint
	declare @dvp bigint
	declare @snvp bigint
	declare @sdvp bigint
	declare @parentDepth int

	-- read the parent's fractions
	-- todo: what does this do if parent no exist
	select
		@nvp = n.[nv],
		@dvp = n.[dv],
		@snvp = n.[snv],
		@sdvp = n.[sdv],
		@parentDepth = n.[depth]
	from [node] as n
	where n.[node_id] = @parentNodeId

	insert into @childtable
	select n.node_id, n.name, n.nv, n.dv, n.snv, n.sdv, n.depth, n.parent_node_id
	from [node] as n
	where
		(cast(n.[nv] as float) / n.[dv]) > (cast(@nvp as float) / @dvp) 
		and 
		(cast(n.[nv] as float) / n.[dv]) < (cast(@snvp as float) / @sdvp) 
		and (@numGenerations = -1 or n.[depth]  <= @parentDepth + @numGenerations)
	--order by n.[depth] ASC, n.[parent_node_id] ASC--, (cast(n.[nv] as float) / n.[dv]) ASC

	return
end
go

--create function getAncestors(
alter function getAncestors(
	@nodeId bigint,
	@numGenerations int
) returns @ancestortable table (
	node_id bigint not null,
	name nvarchar(64) not null,
	nv bigint not null, 
	dv bigint not null,
	snv bigint not null,
	sdv bigint not null,
	depth int not null,
	parent_node_id bigint not null)
as
begin
	declare @ancnv bigint = 0
	declare @ancdv bigint = 1
	declare @ancsnv bigint = 1
	declare @ancsdv bigint = 0
	declare @div bigint
	declare @mod bigint
	declare @numerator bigint
	declare @denominator bigint
	declare @depth int

	select
		@numerator = n.[nv],
		@denominator = n.[dv],
		@depth = n.[depth]
	from [node] as n
	where n.[node_id] = @nodeId
	
	-- todo: loop appears to include node itself
	while @numerator > 0 and @denominator > 0
	begin
		set @div = @numerator / @denominator
		set @mod = @numerator % @denominator
		set @ancnv = @ancnv + @div * @ancsnv
		set @ancdv = @ancdv + @div * @ancsdv
		set @ancsnv = @ancnv + @ancsnv
		set @ancsdv = @ancdv + @ancsdv
		
		insert into @ancestortable
		select node_id, name, nv, dv, snv, sdv, depth, parent_node_id
		from node
		where 
			nv = @ancnv 
			and dv = @ancdv
			and node_id <> @nodeId
			and (@numGenerations = -1 or depth >= @depth - @numGenerations)
		
		set @numerator = @mod
		
		if @numerator <> 0
		begin
			set @denominator = @denominator % @mod
			if @denominator = 0
			begin
				set @denominator = 1
			end
		end
	end
	return
end
go

declare @twoFourId bigint
execute insertNode 0, '[2.1] 5 2 8 3'
execute insertNode 0, '[2.2] 8 3 11 4'
execute insertNode 0, '[2.3] 11 4 14 5'
execute insertNode 0, '[2.4] 14 5 17 6', @twoFourId output
execute insertNode @twoFourId, '[2.4.1] 31 11 48 17'
execute insertNode @twoFourId, '[2.4.2] 48 17 65 23'
execute insertNode @twoFourId, '[2.4.3] 65 23 82 29'

declare @i int = 7
declare @parentNodeId bigint
declare @newNodeId bigint
while (@i < 20)
begin
	execute insertRandomNode  @newNodeId output
	set @i = @i + 1
end

--select * from node

--execute deleteNode @twoFourId
select * from getDescendants(0, -1) as n order by n.[depth] ASC, n.[parent_node_id] ASC, (cast(n.[nv] as float) / n.[dv]) ASC
--select * from getDescendants(0, 2) order by depth asc
--select * from getDescendants(0, 1) order by depth asc
--select * from getDescendants(0, 0) order by depth asc
--select * from getDescendants(@twoFourId, 1) order by depth asc

print '2.4 id = ' + cast(@twoFourId as nvarchar) + ' newNodeId = ' + cast(@newNodeId as nvarchar)
--select * from getAncestors(@twoFourId, -1) order by depth asc
--select * from getAncestors(@twoFourId, 0) order by depth asc
--select * from getAncestors(@twoFourId, 1) order by depth asc
--select * from getAncestors(@twoFourId, 2) order by depth asc

--select * from getAncestors(@newNodeId, 0) order by depth asc
--select * from getAncestors(@newNodeId, 1) order by depth asc
--select * from getAncestors(@newNodeId, 2) order by depth asc
--select * from getAncestors(@newNodeId, 3) order by depth asc
--select * from getAncestors(@newNodeId, 5) order by depth asc
--select * from getAncestors(@newNodeId, -1) order by depth asc