using SmartAnalyzers.CSharpExtensions.Annotations;

namespace UniversalTemplates.Core;

interface IUniversalTemplate
{
    string Transform(Template template, UniversalTemplateContext context);
}

[InitRequired]
public class Template
{
    public string Content { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}
