using System.Text.Json;

namespace TRS.DataManager.JsonHelpers;

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToLower();
}