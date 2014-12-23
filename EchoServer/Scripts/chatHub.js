$(function () {

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

		//chatGoogleChart.addNode(node, true);
		chatSigma.addNode(node, true);
	};

	chat.client.onDeleteNode = function (nodeId) {
		$('#discussion').append('<li><strong>Delete Node</strong>:&nbsp;&nbsp;' + nodeId + '</li>');

		//chatGoogleChart.deleteNode(nodeId, true);
		//chatSigma.deleteNode(nodeId, true);
	};

	chat.client.onTreeReceived = function (tree) {
		$('#discussion').append('<li><strong>Recieved Tree</strong>:&nbsp;&nbsp;</li>');

		//chatGoogleChart.refreshTree(tree, true);
		chatSigma.refreshTree(tree, true);
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

		$('#fancyTree').click(function () {
			chat.server.fancyTree();
		});

		chat.server.getTree();
	});
});
