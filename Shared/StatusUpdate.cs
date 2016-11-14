using NServiceBus;

namespace Shared
{
	public class StatusUpdate : IMessage
	{
		public string ControllerSagaId { get; set; }
	}
}
