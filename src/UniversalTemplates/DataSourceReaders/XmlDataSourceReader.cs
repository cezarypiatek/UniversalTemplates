using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UniversalTemplates.Core;

namespace UniversalTemplates.DataSourceReaders;

class XmlDataSourceReader : IDataSourceReader
{
    public object? Read(Source source)
    {
        Dictionary<string, object?>? ConvertXmlToObject(XElement? xDocument)
        {

            if (xDocument is { } docRoot)
            {
                var dictionary = new Dictionary<string, object?>();
                foreach (var attribute in docRoot.Attributes())
                {
                    dictionary[attribute.Name.LocalName] = attribute.Value;
                }

                if (docRoot.HasElements)
                {
                    var groups = docRoot.Elements().GroupBy(x => x.Name.LocalName).ToArray();

                    foreach (var group in groups)
                    {
                        dictionary[group.Key] = ConvertXmlToObject(group.FirstOrDefault());
                        dictionary[group.Key + "_items"] = group.Select(ConvertXmlToObject).ToArray();
                    }
                }
                else
                {
                    dictionary[docRoot.Name.LocalName] = docRoot.Value;
                }

                return dictionary;
            }

            return null;

        }


        var doc = XDocument.Load(new StringReader(source.Content));
        var data = new Dictionary<string, object>();

        if (doc.Root is { } r)
        {
            data[r.Name.LocalName] = ConvertXmlToObject(doc.Root);
        }

        return data;
    }
}
