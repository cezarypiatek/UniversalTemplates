using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalTemplates.Core;
using UniversalTemplates.DataSourceReaders;
using UniversalTemplates.TemplateEngines;

namespace UniversalTemplates;

internal class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("UniversalTemplates command-line");

        var transformCommand = new Command("transform");
        var valuesOptions = new Option<string>("--input")
        {
            IsRequired = true
        };
        transformCommand.AddOption(valuesOptions);
        var templateOptions = new Option<string>("--template") {IsRequired = true};
        transformCommand.AddOption(templateOptions);
        var outputOptions = new Option<string>("--output")
        {
            IsRequired = false
        };
        transformCommand.AddOption(outputOptions);
        var sourceMetadataOption = new Option<string[]>("--inputOptions") {AllowMultipleArgumentsPerToken = true};
        transformCommand.AddOption(sourceMetadataOption);

        
        transformCommand.SetHandler(async (string valuesPath, string templatePath, string outputPath, string[] sourceMetadata) =>
        {
            IDataSourceReader reader = Path.GetExtension(valuesPath).ToLower() switch
            {
                ".json" => new JsonDataSourceReader(),
                ".xml" => new XmlDataSourceReader(),
                ".yaml" or "yml" => new YamlDataSourceReader(),
                ".csv" => new CsvDataSourceReader(),
                ".sql" => new SqlDataSourceReader(),
                _ => throw new NotSupportedException("Not supported data source")
            };

            var templateEngine = Path.GetExtension(templatePath).ToLower() switch
            {
                ".liquid" => new FluidUniversalTemplate(),
                _ => throw new NotSupportedException("Not supported template engine")
            };

            var dataSourcePayload = await File.ReadAllTextAsync(valuesPath);
            var templatePayload = await File.ReadAllTextAsync(templatePath);

            var data = reader.Read(new Source()
            {
                Content = dataSourcePayload,
                Path = valuesPath,
                SourceMetadata = sourceMetadata.Select(x => x.Split('=', 2)).ToDictionary(x => x[0], x => x[1]) ?? new Dictionary<string, string>()
            });

            var result = templateEngine.Transform(templatePayload, new UniversalTemplateContext()
            {
                data = data,
                arguments = new Dictionary<string, string?>(),
                env = GetEnvironmentVariables()
            });

            if (string.IsNullOrWhiteSpace(outputPath) == false)
            {
                await File.WriteAllTextAsync(outputPath, result, Encoding.UTF8, default);
            }
            else
            {
                Console.WriteLine(result);
            }


        }, valuesOptions, templateOptions, outputOptions, sourceMetadataOption);
        
        rootCommand.AddCommand(transformCommand);
        rootCommand.SetHandler(() =>
        {
            Console.WriteLine("Unknown command");
        });

        _ = await rootCommand.InvokeAsync(args);
    }

    private static IReadOnlyDictionary<string, string?> GetEnvironmentVariables()
    {
        var result = new Dictionary<string, string?>();
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            result[entry.Key.ToString()!] = entry.Value?.ToString();
        }
        return result;
    }
}
