var chatSigma = new function () {
	this._maxDepth = null;
	this._minDepth = null;
	this._depthDelta = null;
	this._depthScale = null;
	this._sigma = new sigma({});

	this.addNode = function (node, redraw) {
		var randNumMin = 0;
		var randNumMax = 1;

		var leftOrRight = (Math.floor(Math.random() * (randNumMax - randNumMin + 1)) + randNumMin)

		var lowestX = 0;
		var highestX = 0
		var nodes = this._sigma.graph.nodes();

		var x = 0;
		if (nodes.length > 0) {
			$(nodes).each(function () {
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

			if (leftOrRight === 0) {
				x = lowestX - 1;
			} else {
				x = highestX + 1;
			}
		}

		this._sigma.graph.addNode({
			id: 'n' + node.nodeId,
			label: 'n' + node.nodeId,
			x: x,
			y: node.depth,
			size: 0.1,
			color: this.getRandomColor()
		});

		if (node.parentNodeId > 0) {
			this._sigma.graph.addEdge({
				id: 'e' + node.nodeId,
				source: 'n' + node.nodeId,
				target: 'n' + node.parentNodeId,
				size: 0.1,
				color: '#000000'
			});
		}

		if (redraw === true) {
			this.refreshGraph();
		}
	};

	this.deleteNode = function (nodeId, redraw) {
		if (nodeId.indexOf('n') === 0) {
			nodeId = nodeId.substring(1);
		}

		$(this._sigma.graph.edges()).each(function () {
			if (this.target === 'n' + nodeId) {
				deleteNode(this.source);

				_sigma.graph.dropNode(this.source);
				//_sigma.graph.dropEdge(this.id);
			}
		});

		_sigma.graph.dropNode('n' + nodeId);
		//_sigma.graph.dropEdge('e' + nodeId);

		if (redraw === true) {
			this.refreshGraph();
		}
	};

	this.refreshTree = function (tree, redraw) {
		if (tree === null || tree.nodes === null) { return; }

		this._sigma.graph.clear();

		//var i = 0;
		_maxDepth = tree.maxDepth;
		_minDepth = tree.minDepth;
		_depthDelta = _maxDepth - _minDepth;
		_depthScale = 1 / _depthDelta;

		var depthCount = 0;
		var prevDepth = -1;
		$(tree.nodes).each(function () {
			/*if (prevDepth != this.depth) {
				depthCount = 1;
				prevDepth = this.depth;
			} else {
				depthCount++;
			}*/

			chatSigma.addNode(this, false);
		});

		if (redraw === true) {
			this.refreshGraph();
		}
	};

	this.getRandomColor = function () {
		var letters = '0123456789ABCDEF'.split('');
		var color = '#';
		for (var i = 0; i < 6; i++) {
			color += letters[Math.floor(Math.random() * 16)];
		}
		return color;
	}

	this.refreshGraph = function () {
		// resets the sizes based on num children
		/*var initialSize = 0.1;
		var nodes = this._sigma.graph.nodes();
		for (var i = 0; i < nodes.length; i++) {
			var degree = this._sigma.graph.degree(nodes[i].id);
			var degreeSqrt = Math.sqrt(degree);
			nodes[i].size = initialSize * degreeSqrt;
		}*/

		this._sigma.refresh();
		//_sigma.startForceAtlas2();
		//_sigma.startForceAtlas2({ worker: true, barnesHutOptimize: false });
	}

	// set the render
	sigma.renderers.def = sigma.renderers.canvas;
}

$(function () {

	// create the renderer
	chatSigma._sigma.addRenderer({
		container: document.getElementById('sigmaWebGl_div'),
		type: 'webgl',
		camera: 'cam1',
		settings: {
			defaultLabelColor: '#000000',
			defaultEdgeType: 'curve',
			drawLabels: 'true'
		}
	});

	// Initialize the dragNodes plugin:
	//var dragListener = sigma.plugins.dragNodes(_sigma, _sigma.renderers[0]);

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

	/*function addNode(node, x) {
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
		//_sigma.startForceAtlas2({ worker: true, barnesHutOptimize: false });
	}*/
});
