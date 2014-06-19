namespace ReminderService.Router
{
    /// <summary>
    /// A decorator over the IConsume interface.
    /// This decorator allows the consumer to handle more general message types and downcast (widen) them so
    /// that the inner consumer can handle the message.
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    /// <typeparam name="TExpected"></typeparam>
    public class WideningConsumer<TBase, TExpected> : IConsume<TBase> 
        where TExpected : TBase 
        where TBase : IMessage
    {
        private readonly IConsume<TExpected> _innerConsumer;

        public WideningConsumer(IConsume<TExpected> innerConsumer)
        {
            _innerConsumer = innerConsumer;
        }

        public void Handle(TBase instance)
        {
            if (instance is TExpected)
                _innerConsumer.Handle((TExpected)instance);
        }
    }
}
