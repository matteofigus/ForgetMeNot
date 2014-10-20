using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Router.Consumers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReminderService.Router
{
	public static class ConsumerExtensions
	{
		public static IConsume<TInput> WidenFrom<TInput, TOutput>(this IConsume<TOutput> handler)
			where TOutput : IMessage
			where TInput : TOutput
		{
			return null; //new WideningConsumer<TInput, TOutput>(handler);
		}

		public static IConsume<TInput> NarrowTo<TInput, TOutput>(this IConsume<TOutput> handler)
			where TInput : IMessage
			where TOutput : TInput
		{
			return null; //new NarrowingConsumer<TInput, TOutput>(handler);
		}
	}
}

