using System;
using Fluid;
using Fluid.Values;
using Humanizer;
using UniversalTemplates.Core;

namespace UniversalTemplates.TemplateEngines;

class FluidUniversalTemplate : IUniversalTemplate
{
    public string Transform(Template template, object? context)
    {
        var options = new TemplateOptions();
        options.Filters.AddFilter("camelCase", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Camelize()) );
        options.Filters.AddFilter("pascalCase", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Pascalize()) );
        options.Filters.AddFilter("titleCase", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Titleize()) );
        options.Filters.AddFilter("snakeCase", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Underscore()) );
        options.Filters.AddFilter("kebabCase", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Kebaberize()) );
        options.Filters.AddFilter("humanize", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Humanize()) );
        options.Filters.AddFilter("dehumanize", (input, arguments, templateContext) => new StringValue(input.ToStringValue().Dehumanize()) );
        var parser = new FluidParser();
        if (parser.TryParse(template.Content, out var ftemplate, out var error))
        {
            var result = ftemplate.Render(new TemplateContext(context, options));
            return result;
        }

        throw new InvalidOperationException(error);
    }
}
