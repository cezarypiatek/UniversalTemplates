using System.Linq;
using Newtonsoft.Json.Linq;
using UniversalTemplates.Core;

namespace UniversalTemplates.DataSourceReaders;

class JsonDataSourceReader : IDataSourceReader
{
    object? ConvertJsonToObject(JToken xDocument)
    {
        return xDocument switch
        {
            JArray jArray => jArray.Select(ConvertJsonToObject).ToArray(),
            JObject jObject => jObject.Properties().ToDictionary(x => x.Name, x => ConvertJsonToObject(x.Value)),
            JValue jValue => jValue.Value,
            _ => null
        };
    }

    public object? Read(Source source)
    {
        var json = JToken.Parse(source.Content);

        if (json is JObject jo && jo.ContainsKey("$schema"))
        {
            jo.Remove("$schema");
        }

        return ConvertJsonToObject(json);
    }
}
