using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exceptions
{
    /// <summary>
    /// This is a good exception that I want to catch
    /// </summary>
    [Serializable]
    public class MacabreUIException : MacabreException
    {
        public MacabreUIException() { }
        public MacabreUIException(string message) : base(message) { }
        public MacabreUIException(string message, Exception inner) : base(message, inner) { }
        protected MacabreUIException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
