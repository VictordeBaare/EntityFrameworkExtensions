using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Diagnostics;

namespace EntityFrameworkExtensions
{
    public class CSharpMergeMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        public CSharpMergeMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies) : base(dependencies)
        {
            Debugger.Launch();
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
            builder.AppendLine($".CreateMerge(\"{operation.TableName}\", \"new List<MergeProperty> {{ {operation.MergeProperties.Select(x => )} }}\")");
        }

        private static void Generate(DropMergeOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($".DropMerge({operation.Name})");
        }
    }
}
