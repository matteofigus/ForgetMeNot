using NUnit.Framework;
using System;
using System.Data;

namespace ReminderService.Core.Tests.Persistence
{
	[TestFixture ()]
	public class ExtenstionTests
	{
		[Test ()]
		public void CanHandleGuid ()
		{
			var reminderId = Guid.NewGuid ();
			var reader = new FakeDataRecord ((col) => reminderId);
			Assert.AreEqual(reminderId, reader.Get<Guid>("reminderId"));
		}

		[Test ()]
		public void CanHandleString ()
		{
			var v = "astring value";
			var reader = new FakeDataRecord ((col) => v);
			Assert.AreEqual(v, reader.Get<string>("columnName"));
		}

		[Test]
		public void CanHandleNoDefaultValueForInt()
		{
			var reader = new FakeDataRecord ((col) => DBNull.Value);
			Assert.AreEqual (0, reader.Get<int> ("columnName"));
		}

		[Test]
		public void CanHandleNoDefaultValueForString()
		{
			var reader = new FakeDataRecord ((col) => DBNull.Value);
			Assert.AreEqual (null, reader.Get<string> ("columnName"));
		}

		[Test]
		public void CanHandleNoDefaultValueForGuid()
		{
			var reader = new FakeDataRecord ((col) => DBNull.Value);
			Assert.AreEqual (Guid.Empty, reader.Get<Guid> ("columnName"));
		}

		[Test]
		public void WillUseDefaultValueForNulls()
		{
			var reader = new FakeDataRecord ((col) => DBNull.Value);
			Assert.AreEqual(1, reader.Get<int>("columnName", 1));
		}

		[Test ()]
		[ExpectedException(typeof(InvalidCastException))]
		public void ThrowsIfNotExpectedType ()
		{
			var v = "astring value";
			var reader = new FakeDataRecord ((col) => v);
			Assert.AreEqual(DBNull.Value, reader.Get<int>("columnName"));
		}
	}

	public class FakeDataRecord : IDataRecord
	{
		private readonly Func<int, object> _columnValueGenerator;
		private readonly Func<string, object> _columnIndexerValue;
 
		public FakeDataRecord (Func<string, object> columnIndexerValue)
		{
			_columnIndexerValue = columnIndexerValue;
		}

		public bool GetBoolean (int i)
		{
			return (bool)_columnValueGenerator (i);
		}

		public byte GetByte (int i)
		{
			return (byte)_columnValueGenerator (i);
		}

		public long GetBytes (int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException ();
		}

		public char GetChar (int i)
		{
			return (char)_columnValueGenerator (i);
		}

		public long GetChars (int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException ();
		}

		public IDataReader GetData (int i)
		{
			throw new NotImplementedException ();
		}

		public string GetDataTypeName (int i)
		{
			throw new NotImplementedException ();
		}

		public DateTime GetDateTime (int i)
		{
			return (DateTime)_columnValueGenerator (i);
		}

		public decimal GetDecimal (int i)
		{
			return (decimal)_columnValueGenerator (i);
		}

		public double GetDouble (int i)
		{
			return (double)_columnValueGenerator (i);
		}

		public Type GetFieldType (int i)
		{
			return _columnValueGenerator (i).GetType();
		}

		public float GetFloat (int i)
		{
			return (float)_columnValueGenerator (i);
		}

		public Guid GetGuid (int i)
		{
			return (Guid)_columnValueGenerator (i);
		}

		public short GetInt16 (int i)
		{
			return (short)_columnValueGenerator (i);
		}

		public int GetInt32 (int i)
		{
			return (int)_columnValueGenerator (i);
		}

		public long GetInt64 (int i)
		{
			return (long)_columnValueGenerator (i);
		}

		public string GetName (int i)
		{
			throw new NotImplementedException ();
		}

		public int GetOrdinal (string name)
		{
			throw new NotImplementedException ();
		}

		public string GetString (int i)
		{
			return (string)_columnValueGenerator (i);
		}

		public object GetValue (int i)
		{
			return _columnValueGenerator (i);
		}

		public int GetValues (object[] values)
		{
			throw new NotImplementedException ();
		}

		public bool IsDBNull (int i)
		{
			return _columnValueGenerator (i) == DBNull.Value;
		}

		public int FieldCount {
			get {
				throw new NotImplementedException ();
			}
		}

		public object this [string index] {
			get {
				if (_columnIndexerValue == null)
					return DBNull.Value;

				return _columnIndexerValue (index);
			}
		}

		public object this [int index] {
			get {
				throw new NotImplementedException ();
			}
		}
	}
}

