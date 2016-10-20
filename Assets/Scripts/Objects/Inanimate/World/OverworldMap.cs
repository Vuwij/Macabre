using UnityEngine;

namespace Objects.Inanimate.World
{
    public partial class Overworld : InanimateObject {
        public GameObject gameObject;

        public Vector2 center;
        public float width;
        public float height;

        public Vector2 topleft
        {
            get { return center + new Vector2(-width / 2.0f, -height / 2.0f); }
        }
        public Vector2 bottomright
        {
            get { return center + new Vector2(width / 2.0f, height / 2.0f); }
        }
        public Vector2 topright
        {
            get { return center + new Vector2(width / 2.0f, -height / 2.0f); }
        }
        public Vector2 bottomleft
        {
            get { return center + new Vector2(-width / 2.0f, height / 2.0f); }
        }

        public Vector2 top
        {
            get { return center - new Vector2(0, height / 2.0f); }
        }
        public Vector2 bottom
        {
            get { return center + new Vector2(0, height / 2.0f); }
        }
        public Vector2 left
        {
            get { return center - new Vector2(width / 2.0f, 0); }
        }
        public Vector2 right
        {
            get { return center + new Vector2(width / 2.0f, 0); }
        }
        
        public void Print()
        {
            Debug.Log("Center: " + center + ", Width: " + width + ", Height:" + height);
        }
    }
}