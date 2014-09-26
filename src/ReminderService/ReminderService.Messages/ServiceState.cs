using System;
using System.Linq;
using System.Collections.Generic;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class ServiceState
	{
		public class MonitorItem : IEquatable<MonitorItem>
		{
			public string Name { get; set; }
			public DateTime TimeStamp { get; set; }
			public string Key { get; set; }
			public string Value { get; set; }

			public bool Equals (MonitorItem other)
			{
				if (other == null)
					return false;

				if (this.Name == other.Name && this.Key == other.Key)
					return true;
				else
					return false;
			}

			public override bool Equals (object obj)
			{
				if (obj == null)
					return false;

				MonitorItem item = obj as MonitorItem;
				if (item == null)
					return false;

				return Equals (item);
			}

			public override int GetHashCode ()
			{
				return Name.GetHashCode() + Key.GetHashCode ();
			}
		}

		public class MonitorGroup
		{
			private readonly Dictionary<string, MonitorItem> _items = new Dictionary<string, MonitorItem> ();

			public string Name { get; set; }
			public DateTime TimeStamp { get; set; }
			public List<MonitorItem> Items {
				get { return new List<MonitorItem>(_items.Values); }
			}

			public MonitorGroup (string name)
			{
				Ensure.NotNullOrEmpty(name, "name");
				Name = name;
				TimeStamp = SystemTime.UtcNow();
			}

			public MonitorGroup (MonitorItem item)
			{
				Ensure.NotNull(item, "item");

				Name = item.Name;
				TimeStamp = item.TimeStamp;
				AddOrUpdate(item);
			}

			public void AddOrUpdate(MonitorItem item)
			{
				if (item.Name != Name)
					throw new InvalidOperationException (string.Format("Cannot add item, since the name of the Item [{0}] differs from the Group [{1}]", item.Name, Name));

				if (!_items.ContainsKey (item.Key))
					_items.Add (item.Key, null);

				_items [item.Key] = item;
			}
		}
	}
}

