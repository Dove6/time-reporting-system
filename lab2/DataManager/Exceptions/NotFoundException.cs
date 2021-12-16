using System.Runtime.Serialization;

namespace TRS.DataManager.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException()
    {}

    public NotFoundException(Exception innerException) :
        base(null, innerException)
    {}

    protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {}
}
