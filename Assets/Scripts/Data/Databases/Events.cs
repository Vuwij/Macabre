using System;
using Objects.Movable.Characters;

namespace Data.Databases
{
	public class Events : Table
	{
		public Events(Database db) : base(db) {}

		const string tableName = "Game_Events";

		public struct EventData {

		}

		public EventData? GetEventData(string name) {
			ExecuteSQL("SELECT * FROM " + tableName + " WHERE Name like `" + name + "`");
			Reader.Read();
			return null;
		}

	}
}

