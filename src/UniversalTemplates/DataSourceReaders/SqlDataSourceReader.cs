using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using UniversalTemplates.Core;

namespace UniversalTemplates.DataSourceReaders;

class SqlDataSourceReader : IDataSourceReader
{
    public object? Read(Source source)
    {
        var data = new List<Dictionary<string, object>>();
        if (source.SourceMetadata.TryGetValue("ConnectionString", out var connectionString) == false)
        {
            throw new InvalidOperationException("Missing ConnectionString in inputOptions");
        }
        using var myConnection = new SqlConnection(connectionString);
        var query = source.Content;
        var sqlCommand = new SqlCommand(query, myConnection);


        foreach (var (key, value) in source.SourceMetadata)
        {
            if (key.StartsWith("@"))
            {
                _ = sqlCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = key,
                    Value = value
                });
            }
        }

        myConnection.Open();
        using var oReader = sqlCommand.ExecuteReader();
        var columnSchema = oReader.GetColumnSchema();
        var headers = columnSchema.Select(x => x.ColumnName).ToArray();
        while (oReader.Read())
        {
            data.Add(headers.ToDictionary(x => x, x => oReader[x]));
        }

        myConnection.Close();
        return data;
    }
}
