# UniversalTemplates
Universal Tool for transformating templates using various data source and template engines

## How to install
`UniversalTemplates` is distribute as a nuget package [UniversalTemplates](https://www.nuget.org/packages/SmartAnalyzers.UniversalTemplates/)


## How to Run

CSV

```
transform
--input "C:\Repositories\UniversalTemplatesPOC\UniversalTemplates\UniversalTemplates.Tests\sample.csv"
--template "C:\Repositories\UniversalTemplatesPOC\UniversalTemplates\UniversalTemplates.Tests\SampleCsv.liquid"
```

SQL

```
transform
--input "C:\Repositories\UniversalTemplatesPOC\UniversalTemplates\UniversalTemplates.Tests\Sample.sql"
--template "C:\Repositories\UniversalTemplatesPOC\UniversalTemplates\UniversalTemplates.Tests\SampleSql.liquid"
--inputOptions ConnectionString="Server=localhost,1444;Database=AdventureWorks;User Id=sa;Password=my_password;TrustServerCertificate=True"
```