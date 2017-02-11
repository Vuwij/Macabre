using System;
using UnityEngine;
using System.Collections;
using Exceptions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Objects.Inanimate.Buildings.Components.Path
{
    public partial class VirtualPathController : InanimateObjectController, IOffsetable
    {
        protected override MacabreObject model
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [SerializeField]
        public RoomController destination;

        // Find the closest offset to enter into
        public Vector2 offset;
        public Vector2 newPosition
        {
            get { return (Vector2) transform.position + newPosition; }
        }


        public RoomController room
        {
            get
            {
                var room = GetComponentInParent<RoomController>();
                if (room == null) throw new MacabreException("Room not specified for the furniture: " + name);
                return room;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawIcon(transform.position + (Vector3)offset, "Light Gizmo.tiff", true);
        }
#endif
    }
}