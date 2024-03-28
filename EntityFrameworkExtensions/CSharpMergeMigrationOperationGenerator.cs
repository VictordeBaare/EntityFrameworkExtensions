using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExtensions
{
    public class CSharpMergeMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        public CSharpMergeMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies) : base(dependencies)
        {
            //Debugger.Launch();
        }

        protected override void Generate(MigrationOperation operation, IndentedStringBuilder builder)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            switch (operation)
            {
                case CreateMergeOperation create:
                    Generate(create, builder);
                    break;

                case DropMergeOperation drop:
                    Generate(drop, builder);
                    break;

                default:
                    base.Generate(operation, builder);
                    break;
            }
        }

        private static void Generate(CreateMergeOperation operation, IndentedStringBuilder builder)
        {
            builder.
                AppendLine($".CreateMerge(");
            using (builder.Indent())
            {
                builder.AppendLine($"name: \"{operation.TableName}\",");
                builder.AppendLine($"columns: table => new");
                builder.AppendLine($"{{");
                var map = new Dictionary<string, string>();
                using (builder.Indent())
                {
                    var scope = new List<string>();
                    for (var i = 0; i < operation.Columns.Count; i++)
                    {
                        var column = operation.Columns[i];
                        var propertyName = column.Name;
                        map.Add(column.Name, propertyName);

                        builder
                            .Append(propertyName)
                            .Append(" = table.Column<")
                            .Append(column.ClrType.FullName)
                            .Append(">(");

                        if (propertyName != column.Name)
                        {
                            builder
                                .Append("name: ")
                                .Append(column.Name)
                                .Append(", ");
                        }

                        if (column.ColumnType != null)
                        {
                            builder
                                .Append("type: ")
                                .Append($"\"{column.ColumnType}\"")
                                .Append(", ");
                        }

                        if (column.IsUnicode == false)
                        {
                            builder.Append("unicode: false, ");
                        }

                        if (column.IsFixedLength == true)
                        {
                            builder.Append("fixedLength: true, ");
                        }

                        if (column.IsRowVersion)
                        {
                            builder.Append("rowVersion: true, ");
                        }

                        builder.Append("nullable: ")
                            .Append(column.IsNullable.ToString().ToLower());

                        builder.Append(")");

                        if (i != operation.Columns.Count - 1)
                        {
                            builder.Append(",");
                        }

                        builder.AppendLine();
                    }
                }

                builder.AppendLine($"}}");
            }
            builder.AppendLine(");");
        }

        private static void Generate(DropMergeOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($".DropMerge(\"{operation.Name}\")");
        }
    }
}
