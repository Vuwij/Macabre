using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events
{
    public class EventKey
    {
        public string key = "";
        public EventKey(string key)
        {
            this.key = key;
        }
        public static implicit operator string(EventKey e)
        {
            return e.ToString();
        }
        public static implicit operator EventKey(string s)
        {
            return new EventKey(s);
        }
    }
}
