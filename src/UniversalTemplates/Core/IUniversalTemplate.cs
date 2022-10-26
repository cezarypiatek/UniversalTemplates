namespace UniversalTemplates.Core;

interface IUniversalTemplate
{
    string Transform(string template, UniversalTemplateContext context);
}
