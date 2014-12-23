;(function(undefined) {
	'use strict';

	if (typeof sigma === 'undefined')
		throw 'sigma is not declared';

	/**
	 * SigmaJS Buchheim Tree Layout Algorithm
	 *
	 * Author: Eric Goetschalckx
	 * Version: 0.1
	 */
	var _root = this;

	function Node(nodeId, parentId, width, height) {
		this.id = nodeId;
		this.children = [];
		this.width = width;
		this.height = height;
		this.parentId = parentId;
		this.mod = 0;
		this.threadId = null;
		this.ancestorId = nodeId;
		this.preliminaryX = 0;
		this.change = 0;
		this.shift = 0;
		this.number = 0;
		this.posX = 0;
		this.posY = 0;
		this.rectX = 0;
		this.rectY = 0;
		this.doCountChildren = true;

		Node.prototype.addChild = function (childNodeId) {
			this.children.push(childNodeId);
		}

		Node.prototype.getFirstChildId = function () {
			return this.children[0];
		}

		Node.prototype.getLastChildId = function () {
			return this.children[this.children.length - 1];
		}

		Node.prototype.isLeaf = function () {
			return this.children.length <= 0;
		}

		Node.prototype.getChildren = function () {
			return this.children;
		}

		Node.prototype.getChildrenReverse = function () {
			var newChidlren = this.children.slice(0);
			return newChidlren.reverse();
		}

		Node.prototype.getWidthOrHeight = function(returnWidth) {
			return returnWidth ? this.width : this.height;
		}

		Node.prototype.getNextLeftId = function () {
			return this.isLeaf() ? this.threadId : this.getFirstChildId();
		}

		Node.prototype.getNextRightId = function () {
			return this.isLeaf() ? this.threadId : this.getLastChildId();
		}

		Node.prototype.isChildOfParent = function (otherParentId) {
			return this.parentId === otherParentId;
		}
	};

	function TreeLayout(sigInst, options) {
		options = options || {};

		this.sigInst = sigInst;
		this.graph = this.sigInst.graph;
		this.rootLocation = "top";
		this.alignmentInLevel = "awayFromRoot";
		this.boundsLeft = Number.MAX_VALUE;
		this.boundsRight = Number.MIN_VALUE;
		this.boundsTop = Number.MAX_VALUE;
		this.boundsBottom = Number.MIN_VALUE;
		this.sizeOfLevel = [];

		// todo: use sigmajs node instead, decorator pattern
		this.nodes = {};

		TreeLayout.prototype.getNodeThickness = function (nodeId) {
			var node = this.nodes[nodeId];
			return node.getWidthOrHeight(!this.isLevelChangeInYAxis());
		}

		TreeLayout.prototype.getNodeSize = function (nodeId) {
			var node = this.nodes[nodeId];
			if (node === undefined) {
				return 0;
			} else {
				return node.getWidthOrHeight(this.isLevelChangeInYAxis());
			}
		}

		TreeLayout.prototype.isLevelChangeInYAxis = function() {
			return this.rootLocation === "top" || this.rootLocation === "bottom";
		}

		TreeLayout.prototype.getLevelChangeSign = function() {
			return (this.rootLocation == "bottom" || this.rootLocation == "right") ? -1 : 1;
		}

		TreeLayout.prototype.updateBounds = function (nodeId, centerX, centerY) {
			var node = this.nodes[nodeId];

			var left = centerX - node.width / 2;
			var right = centerX + node.width / 2;
			var top = centerY - node.height / 2;
			var bottom = centerY + node.height / 2;

			if (this.boundsLeft > left) {
				this.boundsLeft = left;
			}

			if (this.boundsRight < right) {
				this.boundsRight = right;
			}

			if (this.boundsTop > top) {
				this.boundsTop = top;
			}

			if (this.boundsBottom < bottom) {
				this.boundsBottom = bottom;
			}
		}

		TreeLayout.prototype.calcSizeOfLevels = function (nodeId, level) {

			var node = this.nodes[nodeId];

			var oldSize;
			if (this.sizeOfLevel.length <= level) {
				// new level, init size to 0
				this.sizeOfLevel.push(0);
				oldSize = 0;
			} else {
				oldSize = this.sizeOfLevel[level];
			}

			var size = this.getNodeThickness(nodeId);

			if (oldSize < size) {
				this.sizeOfLevel[level] = size;
			}

			if (!node.isLeaf()) {
				var children = node.getChildren();
				for (var i = 0; i < children.length; i++) {
					var childId = children[i];
					this.calcSizeOfLevels(childId, level + 1);
				}
			}
		}

		TreeLayout.prototype.getDistance = function (node1Id, node2Id) {
			//GapBetweenAdjacentLevels = 25;
			//GapBetweenAdjacentNodes = 10;
			/*var gapBetweenXandY = configuration.GetGapBetweenNodes(v, w);
			var sizeOfNodes = getNodeSize(v) + getNodeSize(w);
			var distance = sizeOfNodes / 2 + gapBetweenXandY;
			return distance;*/

			var gapBetweenXandY = 1;
			var sizeOfNodes = this.getNodeSize(node1Id) + this.getNodeSize(node2Id);
			var distance = sizeOfNodes / 2 + gapBetweenXandY;
			return distance;
		}

		TreeLayout.prototype.getNumber = function (nodeId, parentNodeId) {

			var node = this.nodes[nodeId];
			var parentNode = this.nodes[parentNodeId];

			var n;

			// only reset if needed
			// we set all siblings number here
			if (parentNode.doCountChildren) {
				// set the child number, eg: first child, second child, etc...
				var children = parentNode.getChildren();
				var childNum = 1;
				for (var i = 0; i < children.length; i++) {
					var childId = children[i];
					var child = this.nodes[childId];
					child.number = childNum++;
				}

				parentNode.doCountChildren = false;
			}

			// we set this above (hopefully?)
			return node.number;
		}

		TreeLayout.prototype.getAncestorOrDefault = function(vIMinus, node, parentNodeId, defaultAncestorId) {
			var ancestorId = vIMinus.ancestorId;

			if (ancestorId === null) {
				return defaultAncestorId;
			}

			var ancestor = this.nodes[ancestorId];

			return ancestor.isChildOfParent(parentNodeId) ? ancestor : defaultAncestorId;
		}

		TreeLayout.prototype.moveSubtree = function (wMinus, wPlus, parent, shift) {

			var subtrees = this.getNumber(wPlus.id, parent.id) - this.getNumber(wMinus.id, parent.id);

			wPlus.change = wPlus.change - shift / subtrees;

			wPlus.shift = wPlus.shift + shift;

			wMinus.change = wMinus.change + shift / subtrees;

			wPlus.preliminaryX = wPlus.preliminaryX + shift;

			wPlus.mod = wPlus.mod + shift;
		}

		TreeLayout.prototype.apportion = function(nodeId, defaultAncestorId, leftSiblingId, parentNodeId) {
			if (leftSiblingId === null)
			{
				// v has no left sibling
				return defaultAncestorId;
			}

			var node = this.nodes[nodeId];
			var leftSibling = this.nodes[leftSiblingId];
			var parentNode = this.nodes[parentNodeId];

			// The following variables "v..." are used to traverse the contours of the subtrees
			// "Minus" refers to the left subtree
			// "Plus" to the right subtree
			// "I" refers to the inside contour
			// "O" to the outside contour
			var vOPlus = node;
			var vIPlus = node;
			var vIMinus = leftSibling; // leftSibling

			// get leftmost sibling of vIPlus
			// i.e. get the leftmost sibling of v
			// i.e. the leftmost child of the parent of v (which is passed in)
			var leftmostChildOfParentNodeId = parentNode.getFirstChildId();
			var vOMinus = this.nodes[leftmostChildOfParentNodeId];

			var sIPlus = vIPlus.mod;
			var sOPlus = vOPlus.mod;
			var sIMinus = vIMinus.mod;
			var sOMinus = vOMinus.mod;

			var nextRightVIMinusId = vIMinus.getNextRightId();
			var nextLeftVIPlusId = vIPlus.getNextLeftId();
			var nextRightVIMinus = this.nodes[nextRightVIMinusId];
			var nextLeftVIPlus = this.nodes[nextLeftVIPlusId];

			while (nextRightVIMinus != null && nextLeftVIPlus != null) {
				vIMinus = nextRightVIMinus;
				vIPlus = nextLeftVIPlus;

				var prevVOMinus = vOMinus;
				var newVOMinusId = vOMinus.getNextLeftId();
				vOMinus = this.nodes[newVOMinusId];

				var prevVOPlus = vOPlus;
				var newVOPlusId = vOPlus.getNextRightId();
				vOPlus = this.nodes[newVOPlusId];

				vOPlus.ancestorId = nodeId;

				var shift = (vIMinus.preliminaryX + sIMinus) - (vIPlus.preliminaryX + sIPlus) + this.getDistance(nextRightVIMinusId, nextLeftVIPlusId);

				if (shift > 0) {
					var ancestorOrDefaultId = this.getAncestorOrDefault(vIMinus, node, parentNode, defaultAncestorId);
					var ancestorOrDefault = this.nodes[ancestorOrDefaultId];
					this.moveSubtree(ancestorOrDefault, node, parentNode, shift);

					sIPlus = sIPlus + shift;
					sOPlus = sOPlus + shift;
				}

				if (vOMinus === null || vOMinus === undefined) {
					var temp = 42;
				}

				sIMinus = sIMinus + vIMinus.mod;
				sIPlus = sIPlus + vIPlus.mod;
				sOMinus = sOMinus + vOMinus.mod;
				sOPlus = sOPlus + vOPlus.mod;

				nextRightVIMinusId = vIMinus.getNextRightId();
				nextLeftVIPlusId = vIPlus.getNextLeftId();
				nextRightVIMinus = this.nodes[nextRightVIMinusId];
				nextLeftVIPlus = this.nodes[nextLeftVIPlusId];
			}

			if (nextRightVIMinus != null && vOPlus.getNextRightId() == null) {
				vOPlus.threadId = nextRightVIMinus.id;
				vOPlus.mod = vOPlus.mod + sIMinus - sOPlus;
			}

			if (nextLeftVIPlus != null && vOMinus.getNextLeftId() == null) {
				vOMinus.threadId = nextLeftVIPlus.id;
				vOMinus.mod = vOMinus.mod + sIPlus - sOMinus;
				defaultAncestorId = node.id;
			}

			return defaultAncestorId;
		}

		TreeLayout.prototype.executeShifts = function (nodeId) {
			var node = this.nodes[nodeId];

			// subtree shifts start at 0
			var shift = 0;
			var change = 0;

			// hrm...
			// rightmost child moves 0
			var children = node.getChildrenReverse();
			for (var i = 0; i < children.length; i++) {
				var childId = children[i];
				var child = this.nodes[childId];
				change = change + child.change;

				child.preliminaryX = child.preliminaryX + shift;
				child.mod = child.mod + shift;

				shift = shift + child.shift + change;
			}
		}

		/**
		 * FirstWalk(node)
		 * if v is a leaf
		 *   let prelim(v) = 0
		 * else
		 *   let defaultAncestor be the leftmost child of v
		 *   for all children w of v from left to right
		 *   FirstWalk(w)
		 *   Apportion(w, defaultAncestor)
		 * ExecuteShifts(v)
		 * let midpoint = (prelim(leftmost child of v) + prelim(rightmost child of v)) / 2
		 * if v has a left sibling w
		 *   let prelim(v) = prelim(w) + distance
		 *   let mod(v) = prelim(v) - midpoint
		 * else
		 *    let prelim(v) = midpoint
		 */
		TreeLayout.prototype.firstWalk = function (nodeId, leftSiblingId) {

			var node = this.nodes[nodeId];
			var leftSibling = this.nodes[leftSiblingId];

			if (node.isLeaf()) {
				// leaf = end of branch
				if (leftSibling != null) {
					// no left sibling = rightmost sibling
					// move node the requisite distance to the right
					node.preliminaryX = leftSibling.preliminaryX + this.getDistance(nodeId, leftSiblingId);
				}
			} else {
				// not a leaf - node has children

				// let defaultAncestor be the leftmost child of v
				var defaultAncestorId = node.getFirstChildId();

				var previousChildId = null;

				var children = node.getChildren();
				for (var i = 0; i < children.length; i++) {

					var childId = children[i];

					this.firstWalk(childId, previousChildId === null ? null : previousChildId);

					defaultAncestorId = this.apportion(childId, defaultAncestorId, previousChildId, nodeId);

					previousChildId = childId;
				}

				// i swear that with this on, it doesnt work
				//this.executeShifts(nodeId);

				var firstChildId = node.getFirstChildId();
				var lastChildId = node.getLastChildId();

				var firstChild = this.nodes[firstChildId];
				var lastChild = this.nodes[lastChildId];

				var midpoint = (firstChild.preliminaryX + lastChild.preliminaryX) / 2.0;

				if (leftSibling != null) {
					// v has a left sibling
					node.preliminaryX = leftSibling.preliminaryX + this.getDistance(nodeId, leftSiblingId);

					node.mod = node.preliminaryX - midpoint;
				} else {
					// v has no left sibling
					// v is halfway between its first and last child
					node.preliminaryX = midpoint;
				}
			}
		}

		TreeLayout.prototype. secondWalk = function(v, m, level, levelStart) {

			// The rootLocation affects the way how x and y are changed and in what direction
			var levelChangeSign = this.getLevelChangeSign();
			var levelChangeOnYAxis = this.isLevelChangeInYAxis();

			var levelSize = this.sizeOfLevel[level];

			var nodeId = v;
			var node = this.nodes[nodeId];

			var x = node.preliminaryX + m;

			var y;
			if (this.alignmentInLevel == "center") {
				y = levelStart + levelChangeSign * (levelSize / 2);
			} else if (this.alignmentInLevel == "towardsRoot") {
				y = levelStart + levelChangeSign * (getNodeThickness(v) / 2);
			} else {
				// default is Away From Root
				y = levelStart + levelSize - levelChangeSign * (this.getNodeThickness(v) / 2);
			}

			if (!levelChangeOnYAxis) {
				var t = x;
				x = y;
				y = t;
			}

			// set final position
			// pass in x, y, and tree boundaries
			// normalized x = x_relativeToRoot - treeLayout.boundsLeft
			// normalized y = y_relativeToRoot - treeLayout.boundsTop
			//node.setPosition(new NormalizedPosition(this, x, y));

			var normalizedX = x;
			var normalizedY = y;
			//var normalizedX = x - this.boundsLeft;
			//var normalizedY = y - this.boundsTop;

			node.posX = normalizedX;
			node.posY = normalizedY;

			// grow tree bounds, if needed
			this.updateBounds(v, x, y);

			if (!node.isLeaf()) {
				var nextLevelStart =
					levelStart
					//+ (levelSize + configuration.GetGapBetweenLevels(level + 1))
					+ (levelSize + 3)
					* levelChangeSign;

				var children = node.getChildren();
				for (var i = 0; i < children.length; i++) {
					var childId = children[i];
					var child = this.nodes[childId];
					this.secondWalk(child.id, m + node.mod, level + 1, nextLevelStart);
				}
			}
		}

		TreeLayout.prototype.calcNodeBounds = function() {
			for (var nodeId in this.nodes) {
				var node = this.nodes[nodeId];
				var x = node.posX - node.width / 2;
				var y = node.posY - node.height / 2;

				node.rectX = x;
				node.rectY = y;
			}

			//return nodeBounds;
		}

		TreeLayout.prototype.setSigmaPositions = function () {
			var sigmaNodes = this.sigInst.graph.nodes();
			var sigmaEdges = this.sigInst.graph.edges();

			for (var sigmaNodeId in sigmaNodes) {
				var sigmaNode = sigmaNodes[sigmaNodeId];
				var treeNode = this.nodes[sigmaNode.id];

				sigmaNode.x = treeNode.rectX;
				sigmaNode.y = treeNode.rectY;
			}
		}

		TreeLayout.prototype.calcTree = function(rootNodeId) {

			/*this.addNode(rootNodeId, -1);
			this.addNode("n1", rootNodeId);
			this.addNode("n2", rootNodeId);
			this.addNode("n11", "n1");
			this.addNode("n12", "n1");
			this.addNode("n21", "n2");*/

			var sigmaNodes = this.sigInst.graph.nodes();
			var sigmaEdges = this.sigInst.graph.edges();

			for (var nodeIndex in sigmaNodes) {
				var node = sigmaNodes[nodeIndex];
				var foundParent = false;

				// find parent node id
				for (var edgeIndex in sigmaEdges) {
					var edge = sigmaEdges[edgeIndex];

					if (edge.target === node.id) {
						//var parentId = edge.source;
						this.addNode(node.id, edge.source, 1, 1);
						foundParent = true;
						break;
					}
				}

				if (!foundParent) {
					// its the/a root node
					this.addNode(node.id, -1, 1, 1);
				}
			}

			this.firstWalk(rootNodeId, null);

			this.calcSizeOfLevels(rootNodeId, 0);

			var rootNode = this.nodes[rootNodeId];
			this.secondWalk(rootNodeId, rootNode.preliminaryX * -1, 0, 0);

			this.calcNodeBounds();

			this.setSigmaPositions();
		}

		TreeLayout.prototype.addNode = function (nodeId, parentNodeId, width, height) {
			var newNode = new Node(nodeId, parentNodeId, 1, 1);
			var parentNode = this.nodes[parentNodeId];

			this.nodes[nodeId] = newNode;

			if (parentNode != null) {
				parentNode.addChild(nodeId);
			}

			var temp = 42;
		}

		TreeLayout.prototype.getRandomColor = function () {
			var letters = '0123456789ABCDEF'.split('');
			var color = '#';
			for (var i = 0; i < 6; i++) {
				color += letters[Math.floor(Math.random() * 16)];
			}
			return color;
		}
	}

	/**
	* Sigma Interface
	*/
	sigma.prototype.startBuchheim = function (rootNodeId) {

		if (!this.treeLayout) {
			this.treeLayout = new TreeLayout(this);
		}

		this.treeLayout.calcTree("n0");

		return this;
	};

	sigma.prototype.stopBuchheim = function () {
		/*if (!this.supervisor)
			return this;

		// Pause algorithm
		this.supervisor.stop();*/

		return this;
	};

	sigma.prototype.killBuchheim = function () {
		if (!this.treeLayout)
			return this;

		/*// Stop Algorithm
		this.treeLayout.stop();

		// Kill Worker
		this.treeLayout.killWorker();*/

		// Kill supervisor
		this.treeLayout = null;

		return this;
	};

	sigma.prototype.configBuchheim = function (config) {
		/*if (!this.supervisor)
			this.supervisor = new Supervisor(this, config);

		this.supervisor.configure(config);*/

		return this;
	};

	sigma.prototype.isBuchheimRunning = function (config) {
		//return this.treeLayout && this.treeLayout.running;
		return true;
	};
}).call(this);
