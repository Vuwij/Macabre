using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Objects
{
	public class PixelBoxComparison
    {
        // Vertical For Sorting
        public bool NEvertical;
        public bool NWvertical;
        public bool SEvertical;
        public bool SWvertical;

        // Inclusive means partial overlap is allowed, Exclusive means no partial overlap allowed
        public bool NEinclusive;
        public bool NWinclusive;
        public bool SEinclusive;
        public bool SWinclusive;

        public bool NEexclusive;
        public bool NWexclusive;
        public bool SEexclusive;
        public bool SWexclusive;

        public bool Nexclusive;
        public bool Sexclusive;
        public bool Eexclusive;
        public bool Wexclusive;

        public bool Ninclusive => Nexclusive || (NEvertical && !NEexclusive) || (NWvertical && !NWexclusive);
        public bool Sinclusive => Sexclusive || (SEvertical && !SEexclusive) || (SWvertical && !SWexclusive);
        public bool Einclusive => Eexclusive || (NEvertical && !NEexclusive) || (SEvertical && !SEexclusive);
        public bool Winclusive => Wexclusive || (NWvertical && !NWexclusive) || (SWvertical && !SWexclusive);

        // Inside means on the inside border
        public bool NEinside;
        public bool NWinside;
        public bool SEinside;
        public bool SWinside;

        public bool NEoutside;
        public bool NWoutside;
        public bool SEoutside;
        public bool SWoutside;

        public bool NEandSWinside => NEinside && SWinside;
        public bool NWandSEinside => NWinside && SEinside;

        public bool inside => NEandSWinside && NWandSEinside;

        public bool NEandSWoutside => NEoutside || SWoutside;
        public bool NWandSEoutside => NWoutside || SEoutside;

        // Overlap
		public bool NEoverlap => !NEoutside && SWinside && !NWandSEoutside;
		public bool SWoverlap => !SWoutside && NEinside && !NWandSEoutside;
		public bool NWoverlap => !NWoutside && SEinside && !NEandSWoutside;
		public bool SEoverlap => !SEoutside && NWinside && !NEandSWoutside;

        public bool overlap => NEoverlap || SWoverlap || NWoverlap || SEoverlap;

        // Above and below, for ramps
        public bool aAbove;
        public bool aBelow;

        public bool Within => aAbove && aBelow;

        public int inFront
        {
            get
            {
                if (NEvertical || NWvertical)
                    return 1;
                else if (SEvertical || SWvertical)
                    return -1;
                else return 0;
            }
        }
    }

    public struct PixelCollision
    {
        public Direction direction;
        public PixelCollider pixelCollider;
    }
}
