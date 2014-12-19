var chatGoogleChart = new function () {
	this.chart = null;
	this.data = null;

	this.initChart = function () {
		data = new google.visualization.DataTable();
		data.addColumn('string', 'Name');
		data.addColumn('string', 'Manager');

		chart = new google.visualization.OrgChart(document.getElementById('googleChart_div'));
	}

	this.addMessage = function (name, message) {
		// Html encode display name and message.
		var encodedName = $('<div />').text(name).html();
		var encodedMsg = $('<div />').text(message).html();

		// Add the message to the page.
		$('#discussion').append('<li><strong>' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
	}

	this.addNode = function (node, redraw) {
		if (node == null) {
			this.addMessage('Add NULL Node', '');
		} else {
			this.addMessage('Add Node', node.name + '(' + node.nodeId + ')');
		}

		var parentNodeId = '';
		if (node.parentNodeId != 0) { parentNodeId = '' + node.parentNodeId; }

		data.addRow([{ v: '' + node.nodeId, f: node.name + '(' + node.nodeId + ')' }, parentNodeId]);

		if (redraw === true) {
			chart.draw(data, { allowHtml: true });
		}
	}

	this.getRandomColor = function () {
		var letters = '0123456789ABCDEF'.split('');
		var color = '#';
		for (var i = 0; i < 6; i++) {
			color += letters[Math.floor(Math.random() * 16)];
		}
		return color;
	}

	this.deleteNode = function (nodeId, redraw) {
		if (nodeId == null) {
			this.addMessage("Delete NULL Node", "");
		} else {
			this.addMessage("Delete Node", "" + nodeId);
		}

		for (var i = 0; i < data.getNumberOfRows() ; i++) {
			var curNodeId = data.getValue(i, 0);

			if (curNodeId === nodeId) {
				data.removeRow(i);
				return;
			}
		}

		if (redraw === true) {
			chart.draw(data, { allowHtml: true });
		}
	}

	this.refreshTree = function (tree, redraw) {

		data.removeRows(0, data.getNumberOfRows());

		if (tree === null || tree.nodes === null) { return; }

		$(tree.nodes).each(function () {
			chatGoogleChart.addNode(this, false)
		});

		if (redraw === true) {
			chart.draw(data, { allowHtml: true });
		}
	}

	google.load("visualization", "1", { packages: ["orgchart"] });
	google.setOnLoadCallback(this.initChart);
}
