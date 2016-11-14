using NServiceBus;

namespace StatusUpdateReceiver
{
	public class ControllerSagaData : ContainSagaData
	{
		public string ControllerSagaId { get; set; }
		public int Counter { get; set; }
	}
}
