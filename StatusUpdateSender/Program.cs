using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace StatusUpdateSender
{
	public class Program
	{
		static void Main(string[] args)
		{
			ServicePointManager.DefaultConnectionLimit = 500;
			ServicePointManager.UseNagleAlgorithm = false;
			ServicePointManager.Expect100Continue = false;

			AsyncMain().GetAwaiter().GetResult();
		}

		private static async Task AsyncMain()
		{
			var endpointConfiguration = new EndpointConfiguration("SagaConcurrency.StatusUpdateSender");
			endpointConfiguration.SendFailedMessagesTo("error");
			endpointConfiguration.UseSerialization<JsonSerializer>();
			endpointConfiguration.EnableInstallers();
			endpointConfiguration.PurgeOnStartup(true); //For debugging purposes only

			var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
			transport.ConnectionString(Properties.Settings.Default.AzureStorageConnectionString);

			var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
			persistence.ConnectionString(Properties.Settings.Default.AzureStorageConnectionString);

			var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

			var statusUpdates = new List<StatusUpdate>();
			for(int i = 0; i < 20; i++)
			{
				statusUpdates.Add(new StatusUpdate {ControllerSagaId = "id2"});
			}

			await Task.WhenAll(statusUpdates.Select(x => endpointInstance.Send("SagaConcurrency.StatusUpdateReceiver", x)));

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();

			await endpointInstance.Stop().ConfigureAwait(false);
		}
	}
}
