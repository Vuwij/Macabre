using System;
using Objects.Movable.Characters;

namespace Data.Databases
{
	public class Characters : Table
	{
		public Characters(Database db) : base(db) {}

		const string tableName = "Game_Characters";

		public struct CharacterData {
			public string name;
			public string description;
			public string attackDamage;
		}

		public CharacterData? GetCharacter(string name) {
			ExecuteSQL("SELECT * FROM " + tableName + " WHERE Name like `" + name + "`");
			Reader.Read();
			if (Reader.IsDBNull(0)) return null;
			CharacterData cData = new CharacterData {
				name = Reader.GetString (0),
				description = Reader.GetString (1),
				attackDamage = Reader.GetString (2)
			};
			return cData;
		}

	}
}

