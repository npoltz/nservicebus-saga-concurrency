using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace StatusUpdateReceiver
{
	public class ControllerSaga : Saga<ControllerSagaData>,
		IAmStartedByMessages<StatusUpdate>
	{
		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ControllerSagaData> mapper)
		{
			mapper.ConfigureMapping<StatusUpdate>(message => message.ControllerSagaId).ToSaga(sagaData => sagaData.ControllerSagaId);
		}

		public async Task Handle(StatusUpdate message, IMessageHandlerContext context)
		{
			Data.ControllerSagaId = message.ControllerSagaId;
			Data.Counter++;

			Console.WriteLine($"Counter = {Data.Counter}");

			await Task.CompletedTask;
		}
	}
}
