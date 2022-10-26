using HandlebarsDotNet;
using UniversalTemplates.Core;

namespace UniversalTemplates.TemplateEngines;

class HandlebarsUniversalTemplate: IUniversalTemplate
{
    public string Transform(string template, UniversalTemplateContext context)
    {
        var hTemplate = Handlebars.Compile(template);
        return hTemplate(context);
    }
}
