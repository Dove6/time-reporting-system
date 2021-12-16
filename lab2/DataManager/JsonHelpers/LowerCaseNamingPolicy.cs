using System.Text.Json;

namespace Trs.DataManager.JsonHelpers;

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToLower();
}
