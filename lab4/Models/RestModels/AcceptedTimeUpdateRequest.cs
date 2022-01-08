using System.ComponentModel.DataAnnotations;

namespace Trs.Models.RestModels;

public class AcceptedTimeUpdateRequest
{
    [Range(0, int.MaxValue)]
    public int Time { get; set; }
    public byte[] Timestamp { get; set; }
}
