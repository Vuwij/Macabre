using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Environment
{
    /// <summary>
	/// Initializes a new instance of the <see cref="overWorldMap"/> struct.
	/// </summary>
	/// <param name="topleft_">Topleft Corner</param>
	/// <param name="bottomright_">Bottomright Corner</param>
	public struct OverWorldMap
    {
        public Vector2 topleft;
        public Vector2 bottomright;
        public Vector2 topright;
        public Vector2 bottomleft;

        public Vector2 top;
        public Vector2 bottom;
        public Vector2 left;
        public Vector2 right;

        public Vector2 center;
        public float width;
        public float height;



        /// <summary>
        /// Initializes a new instance of the <see cref="overWorldMap"/> struct.
        /// </summary>
        /// <param name="center_">X/Y coordinates of the center</param>
        /// <param name="width_">Width.</param>
        /// <param name="height_">Height.</param>
        public OverWorldMap(Vector2 center_, float width_, float height_)
        {
            this.center = center_;
            this.width = width_;
            this.height = height_;

            this.top = center - new Vector2(0, height / 2.0f);
            this.bottom = center + new Vector2(0, height / 2.0f);

            this.left = center - new Vector2(width / 2.0f, 0);
            this.right = center + new Vector2(width / 2.0f, 0);

            this.topleft = center + new Vector2(-width / 2.0f, -height / 2.0f);
            this.bottomright = center + new Vector2(width / 2.0f, height / 2.0f);

            this.topright = center + new Vector2(width / 2.0f, -height / 2.0f);
            this.bottomleft = center + new Vector2(-width / 2.0f, height / 2.0f);

        }

        public void print()
        {
            Debug.Log("Center: " + center + ", Width: " + width + ", Height:" + height);
        }
    }
}
