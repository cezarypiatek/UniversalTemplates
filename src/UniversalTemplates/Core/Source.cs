using System.Collections.Generic;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace UniversalTemplates.Core;

[InitOnly]
internal class Source
{
    public string Path { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Dictionary<string, string> SourceMetadata { get; set; } = null!;
}
