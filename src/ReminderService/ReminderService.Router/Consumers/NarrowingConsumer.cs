using System;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router
{
	public class NarrowingConsumer<TInput, TOutput> : IConsume<TInput> 
		where TOutput : TInput 
		where TInput : IMessage
	{
		private readonly IConsume<TOutput> _innerConsumer;

		public void Handle(TInput message)
		{
			//this will throw if you cannot cast TWide to TNarrow
			_innerConsumer.Handle ((TOutput)message);
		}
	}
}

