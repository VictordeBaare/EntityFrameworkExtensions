using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace EntityFrameworkExtensions
{
    public static class Template
    {
        public static string GetTypeWithMergeStatement(ITable table)
        {
            var builder = new StringBuilder();

            builder.AppendLine(GetMergeTableType(table));
            builder.AppendLine("GO");
            builder.AppendLine(GetMergeStatement(table));

            return builder.ToString();
        }

        public static string GetMergeStatement(ITable table)
        {
            var builder = new IndentedStringBuilder();
            var columns = table.Columns.OrderBy(x => x.Name).ToList();

            builder
                .AppendLine($"CREATE PROCEDURE {table.SchemaQualifiedName}Merge")
                .IncrementIndent()
                .AppendLine($"@DataTable {table.SchemaQualifiedName}Type READONLY,")
                .DecrementIndent()
                .AppendLine("AS")
                .AppendLine("BEGIN")
                .IncrementIndent()
                .AppendLine("SET NOCOUNT ON;")
                .AppendLine("CREATE TABLE #output (");

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var primaryKeyColumns = table.PrimaryKey?.Columns;
                builder
                    .AppendLine($"{(i == 0 ? "" : ",")}{column.StoreType} {column.StoreType} {(column.IsNullable ? "NULL" : "NOT NULL")} {(primaryKeyColumns?.Contains(column) == true ? "PRIMARY KEY" : "")}");
            }

            builder
                .AppendLine(",Id_Intern bigint NULL")
                .AppendLine(",MergeAction nvarchar(10) NOT NULL-- 'INSERT', 'UPDATE', or 'DELETE'")
                .AppendLine(",WasDeleted bit NOT NULL")
                .AppendLine(",WasUpdated bit NOT NULL")
                .AppendLine(")")
                .AppendLine("MERGE ")
                .AppendLine($"INTO {table.SchemaQualifiedName} WITH(HOLDLOCK) AS [Target] ")
                .AppendLine("USING @DataTable AS[Source] ")
                .AppendLine("ON[Source].Id = [Target].Id ")
                .AppendLine("WHEN MATCHED ")
                .AppendLine("AND ISNULL([Source].Delete_Flag, 'FALSE') = 'FALSE' ")
                .AppendLine("THEN UPDATE ")
                .AppendLine("SET");

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                builder.AppendLine($"{(i == 0 ? "" : ",")}[Target].{column.Name} = [Source].{column.Name}");
            }

            builder
                .AppendLine("WHEN MATCHED ")
                .AppendLine("AND[Source].Delete_Flag = 'TRUE' ")
                .AppendLine("THEN DELETE ")
                .AppendLine("WHEN NOT MATCHED BY TARGET ")
                .AppendLine("THEN INSERT ")
                .AppendLine("(");

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                builder.AppendLine($"{(i == 0 ? "" : ",")}{column.Name}");
            }

            builder
                .AppendLine(")")
                .AppendLine("VALUES")
                .AppendLine("(");

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                builder.AppendLine($"{(i == 0 ? "" : ",")}[Source].{column.Name}");
            }

            builder
                .AppendLine(")")
                .AppendLine()
                .AppendLine("OUTPUT");

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                builder.AppendLine($"{(i == 0 ? "" : ",")}IIF($action = 'DELETE', deleted.[{column.Name}], inserted.[{column.Name}])");
            }

            builder
                .AppendLine(",[Source].Id_Intern ")
                .AppendLine(",$action-- 'INSERT', 'UPDATE', or 'DELETE' ")
                .AppendLine("INTO #output");

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                builder.AppendLine($"{(i == 0 ? "" : ",")}[{column.Name}]");
            }

            builder
                .AppendLine(",Id_Intern")
                .AppendLine(",MergeAction")
                .AppendLine("); ")
                .AppendLine("SELECT #output.*")
                .AppendLine("  FROM #output ")
                .AppendLine("IF OBJECT_ID('tempdb..#output') IS NOT NULL ")
                .AppendLine("BEGIN ")
                .AppendLine("DROP TABLE #output ")
                .AppendLine("END ")
                .AppendLine("END ");

            return builder.ToString();
        }

        public static string GetMergeTableType(ITable table)
        {
            var builder = new StringBuilder();

            builder
                .AppendLine($"DROP TYPE IF EXISTS {table.SchemaQualifiedName}Type;")
                .AppendLine("GO")
                .AppendLine($"CREATE TYPE {table.SchemaQualifiedName}Type AS TABLE (");

            var columns = table.Columns.ToList();
            for (int i = 0; i < table.Columns.Count(); i++)
            {
                var column = columns[i];
                builder
                    .AppendLine($"{(i == 0 ? "" : ",")}{column.StoreType} {column.StoreType} {(column.IsNullable ? "NULL" : "NOT NULL")}");
            }

            builder
                .AppendLine(",[Delete_Flag] [bit] NULL")
                .AppendLine(",[Id_Intern] [bigint] NOT NULL");

            builder
                .AppendLine(")")
                .AppendLine("GO");

            return builder.ToString();
        }
    }
}
