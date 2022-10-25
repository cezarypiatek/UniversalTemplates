using UniversalTemplates.Core;
using YamlDotNet.Serialization;

namespace UniversalTemplates.DataSourceReaders;

class YamlDataSourceReader : IDataSourceReader
{
    public object? Read(Source source)
    {
        var deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize<object>(source.Content);
    }
}
