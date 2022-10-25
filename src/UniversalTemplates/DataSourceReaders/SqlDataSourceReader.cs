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

        using var myConnection = new SqlConnection(source.SourceMetadata["ConnectionString"]);
        var query = source.Content;
        var oCmd = new SqlCommand(query, myConnection);
        myConnection.Open();
        using var oReader = oCmd.ExecuteReader();
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
