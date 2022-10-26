using Scriban;
using UniversalTemplates.Core;
using Template = UniversalTemplates.Core.Template;

namespace UniversalTemplates.TemplateEngines;

class ScribanUniversalTemplate: IUniversalTemplate
{
    public string Transform(Template template, UniversalTemplateContext context)
    {
        var sTemplate = Scriban.Template.Parse(template.Content);
        return sTemplate.Render(context);
    }
}
