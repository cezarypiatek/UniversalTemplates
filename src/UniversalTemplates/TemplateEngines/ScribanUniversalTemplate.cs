using Scriban;
using UniversalTemplates.Core;

namespace UniversalTemplates.TemplateEngines;

class ScribanUniversalTemplate: IUniversalTemplate
{
    public string Transform(string template, UniversalTemplateContext context)
    {
        var sTemplate = Template.Parse(template);
        return sTemplate.Render(context);
    }
}
