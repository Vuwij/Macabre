using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class PixelItem : MonoBehaviour
    {
        [System.Serializable]
        public class Combination {
            public PixelItem with;
            public PixelItem result;
        }

        public Combination[] combinations;

        public bool isLargeItem;
    }
}
