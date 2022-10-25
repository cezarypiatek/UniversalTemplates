using System;
using Fluid;
using UniversalTemplates.Core;

namespace UniversalTemplates.TemplateEngines;

class FluidUniversalTemplate : IUniversalTemplate
{
    public string Transform(string template, UniversalTemplateContext context)
    {
        var options = new TemplateOptions();
        var parser = new FluidParser();
        if (parser.TryParse(template, out var ftemplate, out var error))
        {
            var result = ftemplate.Render(new TemplateContext(context, options));
            return result;
        }

        throw new InvalidOperationException(error);
    }
}
