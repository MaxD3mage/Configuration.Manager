using System.Text.Json;

namespace Configuration.Manager.Tests.Helpers;

public static class JsonHelper
{
    public static JsonElement CreateJsonElement(string json)
    {
        return JsonDocument.Parse(json).RootElement;
    }
}