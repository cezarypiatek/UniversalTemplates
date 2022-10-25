using System.Collections.Generic;

namespace UniversalTemplates.Core;

internal class UniversalTemplateContext
{
    public object? data { get; set; }
    public IReadOnlyDictionary<string, object> arguments { get; set; } = new Dictionary<string, object>();
    public IReadOnlyDictionary<string, object> env { get; set; } = new Dictionary<string, object>();

}
