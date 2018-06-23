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
		public CharacterTask(string actionString) : base(actionString) {}

		public CharacterTask(TaskType taskType, params object[] arguments) {
			this.taskType = taskType;
			for (int i = 0; i < arguments.Length; ++i) {
				this.arguments.Add(arguments[i]);
			}
		}

		public override void ExecuteAction() {
			switch (taskType) {
			case TaskType.ANIMATE:
				break;
			case TaskType.ATTACK:
				break;
			case TaskType.CREATEITEM:
				break;
			case TaskType.GIVES:
				break;
			case TaskType.PUTS:
				break;
			case TaskType.TAKES:
				break;
			case TaskType.TELEPORT:
				break;
			case TaskType.UPDATESTAT:
				break;
			case TaskType.NAVIGATE:
				break;
			}
		}
	}
}

