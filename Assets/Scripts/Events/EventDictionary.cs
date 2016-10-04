using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events
{
    public class EventDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    where TKey : EventKey
    where TValue : EventAction
    {
    }
}
