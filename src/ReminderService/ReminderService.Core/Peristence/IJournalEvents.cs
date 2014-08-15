using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core
{
	public interface IJournalEvents
	{
		void Write(IMessage message);
	}
}

