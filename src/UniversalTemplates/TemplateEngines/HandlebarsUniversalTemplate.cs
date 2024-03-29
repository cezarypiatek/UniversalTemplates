﻿using HandlebarsDotNet;
using UniversalTemplates.Core;

namespace UniversalTemplates.TemplateEngines;

class HandlebarsUniversalTemplate: IUniversalTemplate
{
    public string Transform(Template template, object? context)
    {
        var hTemplate = Handlebars.Compile(template.Content);
        return hTemplate(context);
    }
}
