using System;
using System.Net;
using System.Threading.Tasks;
using NServiceBus;

namespace StatusUpdateReceiver
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
			var endpointConfiguration = new EndpointConfiguration("SagaConcurrency.StatusUpdateReceiver");
			endpointConfiguration.SendFailedMessagesTo("error");
			endpointConfiguration.UseSerialization<JsonSerializer>();
			endpointConfiguration.EnableInstallers();
			endpointConfiguration.PurgeOnStartup(true); //For debugging purposes only
			endpointConfiguration.LimitMessageProcessingConcurrencyTo(10);

			var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
			transport.ConnectionString(Properties.Settings.Default.AzureStorageConnectionString);

			var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
			persistence.ConnectionString(Properties.Settings.Default.AzureStorageConnectionString);

			var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();

			await endpointInstance.Stop().ConfigureAwait(false);
		}
	}
}
