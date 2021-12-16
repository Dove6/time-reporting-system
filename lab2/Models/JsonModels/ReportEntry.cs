using System.Text.Json.Serialization;
using Trs.DataManager.JsonHelpers;

namespace Trs.Models.JsonModels;

public class ReportEntry
{
    [JsonConverter(typeof(DateJsonConverter))]
    public DateTime Date { get; set; }
    public string Code { get; set; }
    public string Subcode { get; set; }
    public int Time { get; set; }
    public string Description { get; set; }
}
