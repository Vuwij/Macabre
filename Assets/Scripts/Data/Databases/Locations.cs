using System;
using Objects.Movable.Characters;

namespace Data.Databases
{
	public class Locations : Table
	{
		public Locations(Database db) : base(db) {}

		const string tableName = "Game_Events";

		public struct LocationData {

		}

		public LocationData? GetEventData(string name) {
			ExecuteSQL("SELECT * FROM " + tableName + " WHERE Name like `" + name + "`");
			Reader.Read();
			return null;
		}

	}
}

