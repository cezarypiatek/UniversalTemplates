using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using UniversalTemplates.Core;

namespace UniversalTemplates.DataSourceReaders;

class CsvDataSourceReader : IDataSourceReader
{
    public object? Read(Source source)
    {
        var data = new List<Dictionary<string, object>>();
        using (var csvParser = new TextFieldParser(new StringReader(source.Content)))
        {
            csvParser.TextFieldType = FieldType.Delimited;
            csvParser.SetDelimiters(",");
            var headerRow = true;
            IReadOnlyList<string> headers = Array.Empty<string>();

            while (!csvParser.EndOfData)
            {
                if (csvParser.ReadFields() is { } fields)
                {
                    if (headerRow)
                    {
                        headerRow = false;
                        headers = fields.ToArray();
                    }
                    else
                    {
                        var row = new Dictionary<string, object>();
                        data.Add(row);
                        for (var i = 0; i < fields.Length; i++)
                        {
                            row[headers[i]] = fields[i];
                        }
                    }
                }
            }
        }

        return data;
    }
}
