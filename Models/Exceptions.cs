using System.Runtime.Serialization;

namespace GerenciamentoAPI.Models
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
        protected UserNotFoundException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
    }

    [Serializable]
    public class ActivityNotFoundException : Exception
    {
        public ActivityNotFoundException(string message) : base(message) { }
        protected ActivityNotFoundException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
    }

    [Serializable]
    public class UserValidationException : Exception
    {
        public UserValidationException(string message) : base(message) { }
        protected UserValidationException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
    }
}
