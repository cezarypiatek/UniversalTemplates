using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using UniversalTemplates.Core;
using UniversalTemplates.DataSourceReaders;
using UniversalTemplates.TemplateEngines;

namespace UniversalTemplates;

public class Program
{

    // TODO: file for defining transformation of multiple files
    // TODO: generate output file name base on the template - stripe the extension
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
        var templateEngineOptions = new Option<string?>("--templateEngine") ;
        transformCommand.AddOption(templateEngineOptions);
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

        var skipContextOption = new Option<bool>("--doNotWrapInputInContext");
        transformCommand.AddOption(skipContextOption);
        
        transformCommand.SetHandler(async (valuesPath, templatePath, outputPath, sourceMetadata, arguments, skipContext, templateEngineName) =>
        {
            var inputData = ExpandPath(valuesPath)
                .Select(p => (path:p, content: File.ReadAllText(p)))
                .Select(p => GetDataReader(p.path).Read(new Source
                {
                    Content = p.content,
                    Path = p.path,
                    SourceMetadata = ToDictionary(sourceMetadata)
                })).OfType<object>().ToArray();


            var data = MergeInputs(inputData);


            var templatePayload = await File.ReadAllTextAsync(templatePath);
            IUniversalTemplate templateEngine = (templateEngineName?.ToLower() ?? Path.GetExtension(templatePath).ToLower().Trim().TrimStart('.')) switch
            {
                "liquid" => new FluidUniversalTemplate(),
                "handlebars" or "hbs" => new HandlebarsUniversalTemplate(),
                "scriban" or "sbn" => new ScribanUniversalTemplate(),
                "t4" => new T4UniversalTemplate(),
                _ => throw new NotSupportedException("Not supported template engine")
            };
            var result = templateEngine.Transform
            (
                template: new Template
                {
                    Content = templatePayload,
                    FilePath = templatePath
                },
                context: skipContext ? data : new UniversalTemplateContext()
                {
                    data = data,
                    arguments = ToDictionary(arguments),
                    env = GetEnvironmentVariables()
                }
            );

            if (string.IsNullOrWhiteSpace(outputPath) == false)
            {
                await File.WriteAllTextAsync(outputPath, result, Encoding.UTF8, default);
            }
            else
            {
                Console.WriteLine(result);
            }


        }, valuesOptions, templateOptions, outputOptions, sourceMetadataOption, templateArgumentsOptions, skipContextOption, templateEngineOptions);
        
        rootCommand.AddCommand(transformCommand);
        rootCommand.SetHandler(() =>
        {
            Console.WriteLine("Unknown command");
        });

        _ = await rootCommand.InvokeAsync(args);
    }

    internal static object? MergeInputs(IReadOnlyList<object> inputData)
    {
        if (inputData.Count() < 2)
        {
            return inputData.FirstOrDefault();
        }

        if (inputData.All(x => x is IDictionary<string, object>))
        {
            var result = new Dictionary<string, object>();
            foreach (var d in inputData.OfType<IDictionary<string, object>>())
            {
                foreach (var (key, val) in d)
                {
                    result[key] = val;
                }
            }

            return result;
        }

        if (inputData.All(x => x is IEnumerable<object>))
        {
            return inputData.SelectMany(x => (x as IEnumerable<object>)!).Where(x => x != null).ToArray();

        }

        throw new InvalidOperationException("Cannot merge data sources with different format");
    }

    private static IDataSourceReader GetDataReader(string valuesPath)
    {
        return Path.GetExtension(valuesPath).ToLower() switch
        {
            ".json" => new JsonDataSourceReader(),
            ".xml" => new XmlDataSourceReader(),
            ".yaml" or "yml" => new YamlDataSourceReader(),
            ".csv" => new CsvDataSourceReader(),
            ".sql" => new SqlDataSourceReader(),
            _ => throw new NotSupportedException("Not supported data source")
        };
    }


    public static IReadOnlyList<string> ExpandPath(string pathGlob)
    {
        if (pathGlob.Split("*", 2) is {Length: 2} parts && string.IsNullOrWhiteSpace(parts[0]) == false)
        {
            var matcher = new Matcher().AddInclude("*" + parts[1]);
            return matcher.GetResultsInFullPath(parts[0]).ToArray();
        }

        if (pathGlob.StartsWith("*"))
        {
            var matcher = new Matcher().AddInclude(pathGlob);
            return matcher.GetResultsInFullPath(Environment.CurrentDirectory).ToArray();
        }

        return new []{ pathGlob };
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
