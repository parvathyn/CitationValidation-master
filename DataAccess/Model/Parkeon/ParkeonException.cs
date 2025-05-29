using System;
using System.Runtime.Serialization;

namespace DataAccess.Model.Parkeon
{
    [Serializable]
    public class ParkeonException : Exception
    {
        public ParkeonException(string message)
            : base(message) { }

        public ParkeonException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public ParkeonException(string message, Exception innerException)
            : base(message, innerException) { }

        public ParkeonException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected ParkeonException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
