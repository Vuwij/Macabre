using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events
{
    public class EventKey
    {
        readonly string key;
        public EventKey(string key_)
        {
            this.key = key_;
        }
        public static implicit operator string(EventKey e)
        {
            return e.ToString();
        }
        public static implicit operator EventKey(string s)
        {
            return new EventKey(s);
        }
        EventAction e = new EventAction();
    }

}
