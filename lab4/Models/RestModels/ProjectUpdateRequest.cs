namespace Trs.Models.RestModels;

public class ProjectUpdateRequest : ProjectCreationRequest
{
    public byte[] Timestamp { get; set; }
}
