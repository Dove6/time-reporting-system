namespace Trs.Models.RestModels;

public class ManagedProjectListResponseEntry : ProjectListResponseEntry
{
    public bool Active { get; set; }
    public int Budget { get; set; }
    public int BudgetLeft { get; set; }
    public byte[] Timestamp { get; set; }
}
