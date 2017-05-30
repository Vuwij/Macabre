using UnityEngine;
using System.Collections.Generic;

namespace Objects.Unmovable
{
    public abstract partial class UnmovableObject : Object {
		public Object MacabreObject;
		protected List<Object> itemsToShowWhenEntered = new List<Object>();
		protected List<Object> itemsToHideWhenEntered = new List<Object>();

		void OnTriggerStay2D(Collider2D other) {}

		protected override void Start() {
			base.Start();
		}
    }
}
