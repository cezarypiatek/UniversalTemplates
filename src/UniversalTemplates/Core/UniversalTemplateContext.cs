using System.Collections.Generic;

namespace UniversalTemplates.Core;

internal class UniversalTemplateContext
{
    public object? data { get; set; }
    public IReadOnlyDictionary<string, string> arguments { get; set; } = null!;
    public IReadOnlyDictionary<string, string> env { get; set; } = null!;

}
