namespace Trs.Models.RestModels;

public class AcceptedTimeListEntry
{
    public string Username { get; set; }
    public string Month { get; set; }
    public int DeclaredTime { get; set; }
    public int? AcceptedTime { get; set; }
}
