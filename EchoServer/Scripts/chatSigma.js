var chatSigma = new function () {
	//this._maxDepth = null;
	//this._minDepth = null;
	//this._depthDelta = null;
	//this._depthScale = null;
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
			label: '' + node.nodeId,
			x: x,
			y: node.depth,
			size: 1,
			color: this.getRandomColor(),
			parentId: 'n' + node.parentNodeId,
			//nodeIndex: this._sigma.graph.nodes().length,
			//parentIndex: 
		});

		if (node.parentNodeId >= 0) {
			this._sigma.graph.addEdge({
				id: 'e' + node.nodeId,
				source: 'n' + node.parentNodeId,
				target: 'n' + node.nodeId,
				size: 0,
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
		if (tree === null || tree.nodes === null) {
			return;
		}

		this._sigma.graph.clear();

		var start = new Date().getTime();

		this._sigma.graph.addNode({
			id: 'n0',
			label: 'GOT ROOT?',
			x: 0,
			y: 0,
			size: 8,
			color: this.getRandomColor(),
			parentId: -1
		});

		$(tree.nodes).each(function () {
			chatSigma.addNode(this, false);
		});

		this._sigma.startBuchheim('n0');

		if (redraw === true) {
			this.refreshGraph();
		}

		var end = new Date().getTime();
		var time = end - start;
		alert('Execution time: ' + time);
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
	chatSigma._sigma.addRenderer({
		container: document.getElementById('sigmaWebGl_div'),
		type: 'webgl',
		camera: 'cam1',
		settings: {
			//defaultLabelColor: '#000000',
			drawLabels: 'false'
		}
	});
});
