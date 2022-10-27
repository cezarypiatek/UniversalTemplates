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


        var templateArgumentsOptions = new Option<string[]>("--arguments")
        {
            AllowMultipleArgumentsPerToken = true
        };
        transformCommand.AddOption(templateArgumentsOptions);
        
        transformCommand.SetHandler(async (valuesPath, templatePath, outputPath, sourceMetadata, arguments) =>
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

            IUniversalTemplate templateEngine = Path.GetExtension(templatePath).ToLower() switch
            {
                ".liquid" => new FluidUniversalTemplate(),
                ".handlebars" or ".hbs" => new HandlebarsUniversalTemplate(),
                ".scriban" or ".sbn" => new ScribanUniversalTemplate(),
                ".t4" => new T4UniversalTemplate(),
                _ => throw new NotSupportedException("Not supported template engine")
            };

            var dataSourcePayload = await File.ReadAllTextAsync(valuesPath);
            var templatePayload = await File.ReadAllTextAsync(templatePath);

            var data = reader.Read(new Source
            {
                Content = dataSourcePayload,
                Path = valuesPath,
                SourceMetadata = ToDictionary(sourceMetadata)
            });

            var result = templateEngine.Transform(new Template()
            {
                Content = templatePayload,
                FilePath = templatePath
            }, new UniversalTemplateContext()
            {
                data = data,
                arguments = ToDictionary(arguments),
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


        }, valuesOptions, templateOptions, outputOptions, sourceMetadataOption, templateArgumentsOptions);
        
        rootCommand.AddCommand(transformCommand);
        rootCommand.SetHandler(() =>
        {
            Console.WriteLine("Unknown command");
        });

        _ = await rootCommand.InvokeAsync(args);
    }

    private static Dictionary<string, string> ToDictionary(string[] sourceMetadata)
    {
        return sourceMetadata.Select(x => x.Split('=', 2)).ToDictionary(x => x[0], x => x[1]) ?? new Dictionary<string, string>();
    }

    private static IReadOnlyDictionary<string, string> GetEnvironmentVariables()
    {
        var result = new Dictionary<string, string>();
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            result[entry.Key.ToString()!] = entry.Value!.ToString()!;
        }
        return result;
    }
}
