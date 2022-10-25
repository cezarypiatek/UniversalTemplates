using System.Collections.Generic;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace UniversalTemplates.Core;

[InitOnly]
internal class Source
{
    public string Path { get; set; }
    public string Content { get; set; }
    public Dictionary<string, string> SourceMetadata { get; set; }
}
