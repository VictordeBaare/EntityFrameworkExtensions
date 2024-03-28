using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkExtensions
{
    public class SqlServerMergeMigrationSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public SqlServerMergeMigrationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer) : base(dependencies, commandBatchPreparer)
        {
        }

        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            //Debugger.Launch();
            switch (operation)
            {
                case CreateMergeOperation createoperation:
                    Generate(createoperation, builder, Dependencies.TypeMappingSource);
                    break;

                case DropMergeOperation dropoperation:
                    Generate(dropoperation, builder);
                    break;

                default:
                    base.Generate(operation, model, builder);
                    break;
            }
        }

        private void Generate(DropMergeOperation operation, MigrationCommandListBuilder builder)
        {
            //Drop stored procedure if exists
            DropStoredProcedure(operation.Name, builder);

            DropTableType(operation.Name, builder);
        }

        private static void DropTableType(string tableName, MigrationCommandListBuilder builder)
        {
            builder.AppendLine("IF TYPE_ID('dbo." + tableName + "Type') IS NOT NULL");
            builder.AppendLine("BEGIN");
            using (builder.Indent())
            {
                builder.AppendLine("DROP TYPE dbo." + tableName + "Type;");
            }
            builder.EndCommand();
        }

        private static void DropStoredProcedure(string tableName, MigrationCommandListBuilder builder)
        {
            builder.AppendLine("IF OBJECT_ID('dbo.Merge_" + tableName + "') IS NOT NULL");
            builder.AppendLine("BEGIN");
            using (builder.Indent())
            {
                builder.AppendLine("DROP PROCEDURE dbo.Merge_" + tableName + ";");
            }
            builder.EndCommand();
        }

        private void Generate(CreateMergeOperation operation, MigrationCommandListBuilder builder, IRelationalTypeMappingSource typeMappingSource)
        {
            DropTableType(operation.TableName, builder);

            DropStoredProcedure(operation.TableName, builder);

            builder.AppendLine("CREATE TYPE dbo." + operation.TableName + "Type AS TABLE");
            builder.AppendLine("(");
            using (builder.Indent())
            {
                for (int i = 0; i < operation.Columns.Count; i++)
                {
                    AddColumnOperation? column = operation.Columns[i];
                    builder.Append($"{column.Name} {column.ColumnType} {(column.IsNullable ? "NULL" : "NOT NULL")}");
                    if (i < operation.Columns.Count - 1)
                    {
                        builder.AppendLine(",");
                    }
                }
                builder.AppendLine("ShouldDelete bit");
            }
            builder.AppendLine(");");
            builder.EndCommand();

            builder.AppendLine("CREATE STORED PROCEDURE [dbo].[Merge_" + operation.TableName + "]");
            using (builder.Indent())
            {
                builder.AppendLine($"@SourceTable dbo.{operation.TableName}Type READONLY");
            }
            builder.AppendLine("AS");
            builder.AppendLine("BEGIN");
            using (builder.Indent())
            {
                builder.AppendLine("MERGE INTO " + operation.TableName + " AS Target");
                builder.AppendLine("USING @SourceTable AS Source");
                builder.AppendLine("ON Target.Id = Source.Id");
                builder.AppendLine("WHEN MATCHED AND Source.ShouldDelete = 0 THEN");
                builder.AppendLine("UPDATE SET");
                using (builder.Indent())
                {
                    for (int i = 0; i < operation.Columns.Count; i++)
                    {
                        AddColumnOperation? column = operation.Columns[i];
                        builder.AppendLine($"Target.{column.Name} = Source.{column.Name}");
                        if (i < operation.Columns.Count - 1)
                        {
                            builder.AppendLine(",");
                        }
                    }
                }
                builder.AppendLine("WHEN MATCHED AND Source.ShouldDelete = 1 THEN");
                using (builder.Indent())
                {
                    builder.AppendLine("DELETE");
                }
                builder.AppendLine("WHEN NOT MATCHED BY TARGET THEN");
                using (builder.Indent())
                {
                    builder.AppendLine("INSERT (Id, Name, Description)");
                    builder.AppendLine($"VALUES ({string.Join(',', operation.Columns.Select(x => $"Source.{x.Name}"))});");
                }
                builder.AppendLine();
                builder.AppendLine("OUTPUT $action, Inserted.Id, Deleted.Id;");
            }

            builder.EndCommand();
        }
    }
}
