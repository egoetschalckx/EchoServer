$(function () {
	var
		maxDepth,
		minDepth,
		depthDelta,
		depthScale;

	// set the render
	sigma.renderers.def = sigma.renderers.canvas;

	var _sigma = new sigma({
		
	});

	_sigma.addRenderer({
		container: document.getElementById('graphWebGlContainer'),
		type: 'webgl',
		camera: 'cam1',
		settings: {
			defaultLabelColor: '#fff',
			defaultEdgeType: 'curve',
			drawLabels: 'false'
		}
	});

	// Initialize the dragNodes plugin:
	//var dragListener = sigma.plugins.dragNodes(_sigma, _sigma.renderers[0]);

	// Declare a proxy to reference the hub.
	var chat = $.connection.chatHub;

	// Create a function that the hub can call to broadcast messages.
	chat.client.onMessageReceived = function (name, message) {
		// Html encode display name and message.
		var encodedName = $('<div />').text(name).html();
		var encodedMsg = $('<div />').text(message).html();

		// Add the message to the page.
		$('#discussion').append('<li><strong>' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
	};

	chat.client.onNewNode = function (node) {
		$('#discussion').append('<li><strong>New Node</strong>:&nbsp;&nbsp;' + node.name + '</li>');

		var randNumMin = 0;
		var randNumMax = 1;

		var leftOrRight = (Math.floor(Math.random() * (randNumMax - randNumMin + 1)) + randNumMin)

		var lowestX = 0;
		var highestX = 0;
		$(_sigma.graph.nodes()).each(function () {
			if (this.y != node.depth) {
				return;
			}

			if (this.x > highestX) {
				highestX = this.x;
			}

			if (this.x < lowestX) {
				lowestX = this.x;
			}
		})

		var x = 0;
		if (leftOrRight === 0) {
			x = lowestX - 1;
		} else {
			x = highestX + 1;
		}

		addNode(node, x);

		refreshGraph();
	};

	chat.client.onDeleteNode = function (nodeId) {
		$('#discussion').append('<li><strong>Delete Node</strong>:&nbsp;&nbsp;' + nodeId + '</li>');

		deleteNode('' + nodeId);

		refreshGraph();
	};

	chat.client.onTreeReceived = function (tree) {
		if (tree === null || tree.nodes === null) {
			return;
		}

		_sigma.graph.clear();

		var i = 0;
		_maxDepth = tree.maxDepth;
		_minDepth = tree.minDepth;
		_depthDelta = maxDepth - minDepth;
		_depthScale = 1 / depthDelta;

		var depthCount = 0;
		var prevDepth = -1;
		$(tree.nodes).each(function () {
			if (prevDepth != this.depth) {
				depthCount = 1;
				prevDepth = this.depth;
			} else {
				depthCount++;
			}

			addNode(this, depthCount);

			i++;

			if (i % 10 === 0) {
				console.log('treeNode ' + i);
			}
		});

		refreshGraph();
	};

	// Get the user name and store it to prepend to messages.
	//$('#displayname').val(prompt('Enter your name:', ''));
	$('#displayname').val('EricJS');

	// Set initial focus to message input box.
	$('#message').focus();

	// Start the connection.
	$.connection.hub.start({
			//transport: ['webSockets']
		}).done(function () {
		$('#sendmessage').click(function () {
			// Call the Send method on the hub.
			chat.server.send($('#displayname').val(), $('#message').val());

			// Clear text box and reset focus for next comment.
			$('#message').val('').focus();
		});

		chat.server.getTree();
	});

	/*dragListener.bind('startdrag', function (event) {
		console.log(event);
	});
	dragListener.bind('drag', function (event) {
		console.log(event);
	});
	dragListener.bind('drop', function (event) {
		console.log(event);
	});
	dragListener.bind('dragend', function (event) {
		console.log(event);
	});*/

	function addNode(node, x) {
		_sigma.graph.addNode({
			id: 'n' + node.nodeId,
			label: 'n' + node.nodeId,
			x: x,
			y: node.depth,
			size: 0.1,
			color: getRandomColor()
		});

		if (node.parentNodeId > 0) {
			_sigma.graph.addEdge({
				id: 'e' + node.nodeId,
				source: 'n' + node.nodeId,
				target: 'n' + node.parentNodeId,
				size: 0.1,
				color: '#000000'
			});
		}
	}

	function deleteNode(nodeId) {
		if (nodeId.indexOf('n') === 0) {
			nodeId = nodeId.substring(1);
		}

		$(_sigma.graph.edges()).each(function () {
			if (this.target === 'n' + nodeId) {
				deleteNode(this.source);

				_sigma.graph.dropNode(this.source);
				//_sigma.graph.dropEdge(this.id);
			}
		});

		_sigma.graph.dropNode('n' + nodeId);
		//_sigma.graph.dropEdge('e' + nodeId);
	}

	function getRandomColor() {
		var letters = '0123456789ABCDEF'.split('');
		var color = '#';
		for (var i = 0; i < 6; i++) {
			color += letters[Math.floor(Math.random() * 16)];
		}
		return color;
	}

	function refreshGraph() {
		var initialSize = 0.1;
		var nodes = _sigma.graph.nodes();
		for (var i = 0; i < nodes.length; i++) {
			var degree = _sigma.graph.degree(nodes[i].id);
			var degreeSqrt = Math.sqrt(degree);
			nodes[i].size = initialSize * degreeSqrt;
		}
		_sigma.refresh();
		//_sigma.startForceAtlas2();
		_sigma.startForceAtlas2({ worker: true, barnesHutOptimize: false });
	}
});
