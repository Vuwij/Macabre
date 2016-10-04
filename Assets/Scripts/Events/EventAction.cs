using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events
{
    public class EventAction
    {
        public delegate void Action();
        public event Action action;
        public bool repeatable = true;

        public List<EventAction> ExclusionList;
        public List<EventAction> PrerequisiteList;

        public int actionCount;

        public EventAction()
        {
            action += () => actionCount++;
        }

        public void Execute() { action(); }

    }
}
