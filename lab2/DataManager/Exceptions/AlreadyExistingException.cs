using System.Runtime.Serialization;

namespace Trs.DataManager.Exceptions;

[Serializable]
public class AlreadyExistingException : Exception
{
    public AlreadyExistingException()
    {}

    public AlreadyExistingException(Exception innerException) :
        base(null, innerException)
    {}

    protected AlreadyExistingException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {}
}
