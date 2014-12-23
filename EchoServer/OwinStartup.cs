using EchoServer;
using EchoServer.Storage;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
	public class Startup
	{
		public List<Node> Nodes = new List<Node>();

		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();

			var cancellation = new CancellationTokenSource();

			//Task.Run(() => RepeatActionEvery(() => ChatHub.SendSystemMessage(DateTimeOffset.Now.ToString()), TimeSpan.FromSeconds(10), cancellation.Token));
			Task.Run(() => RepeatActionEvery(() => ChatHub.SendSystemMessage(DateTimeOffset.Now.ToString()), TimeSpan.FromSeconds(30)));
			//Task.Run(() => RepeatActionEvery(() => ChatHub.AddOrRemoveRandomNode(), TimeSpan.FromSeconds(10)));
		}

		public static async Task RepeatActionEvery(Action action, TimeSpan interval)
		{
			while (true)
			{
				action();
				Task task = Task.Delay(interval);

				try
				{
					await task;
				}

				catch (TaskCanceledException)
				{
					return;
				}
			}
		}
	}
}
