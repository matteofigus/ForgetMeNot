using System;
using System.Data;
using Npgsql;
using System.Data.SqlClient;
using System.Data.Common;
using System.Reactive.Linq;
using ReminderService.Messages;
using System.Reactive.Disposables;

namespace ReminderService.Core.Persistence.Npgsql
{
	public static class RxExtensions
	{
		public static IObservable<T> ExecuteAsObservable<T>(this IDbCommand command, IDbConnection connection, Func<IDataReader, T> mapper)
		{
			return Observable.Create<T> (observer => {
				using (connection) {
					using(command) {
						try {
							command.Connection = connection;
							connection.Open();
							using (var reader = command.ExecuteReader()){
								while(reader.Read())
								{
									var value = mapper(reader);
									observer.OnNext(value);
								}
							}
						}
						catch (Exception ex) {
							observer.OnError(ex);
						}
					}
				}
				observer.OnCompleted();
				return Disposable.Empty;
			});
		}



		public static Func<IDataReader, ReminderMessage.Cancel> CancelMapper()
		{
			return (reader) => {
				return new ReminderMessage.Cancel(reader.GetGuid(0));
			};
		}
	}
}

