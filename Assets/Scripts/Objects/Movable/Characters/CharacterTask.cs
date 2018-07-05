using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Objects.Movable.Characters
{
	public class CharacterTask : GameTask
	{
		public CharacterTask(TaskType taskType, params object[] arguments) : base(taskType, arguments) {
		}
	}
}

