using Mono.TextTemplating;
using UniversalTemplates.Core;

namespace UniversalTemplates.TemplateEngines;

class T4UniversalTemplate: IUniversalTemplate
{
    public string Transform(Template template, object? context)
    {
        var generator = new TemplateGenerator();
        var parsed = generator.ParseTemplate(template.FilePath, template.Content);
        var settings = TemplatingEngine.GetSettings(generator, parsed);
        settings.CompilerOptions = "-nullable:enable";
        return generator.PreprocessTemplate(parsed, template.FilePath, template.Content, settings, out _);
    }
}
