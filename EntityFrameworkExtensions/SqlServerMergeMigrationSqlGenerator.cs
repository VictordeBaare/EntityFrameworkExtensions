using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using System.Diagnostics;

namespace EntityFrameworkExtensions
{
    public class SqlServerMergeMigrationSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public SqlServerMergeMigrationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer) : base(dependencies, commandBatchPreparer)
        {
        }

        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            Debugger.Launch();
            switch (operation)
            {
                case CreateMergeOperation createoperation:
                    var modelObject = model.FindEntityType(createoperation.TableName);
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
            builder.AppendLine("--TEST CREATE");
            builder.EndCommand();
        }

        private void Generate(CreateMergeOperation operation, MigrationCommandListBuilder builder, IRelationalTypeMappingSource typeMappingSource)
        {
            builder.AppendLine("--TEST DROP");

            builder.EndCommand();
        }
    }
}
