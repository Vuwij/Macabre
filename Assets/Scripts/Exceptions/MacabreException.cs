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
    public class MacabreException : Exception
    {
        public MacabreException() { }
        public MacabreException(string message) : base(message) { }
        public MacabreException(string message, Exception inner) : base(message, inner) { }
        protected MacabreException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
