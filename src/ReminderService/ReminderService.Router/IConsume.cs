using ReminderService.Common;

namespace ReminderService.Router

{
	/// <summary>
	/// Implementors handle messages of type T
	/// </summary>
	public interface IConsume<T> where T : IMessage
    {
        void Handle(T msg);
    }
}
