using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using System.Reflection;

namespace EntityFrameworkExtensions
{
    public static class MergeOptionsBuilderExtensions
    {
        public static OperationBuilder<CreateMergeOperation> CreateMerge<TColumns>(
            this MigrationBuilder migrationBuilder,
            string name,
            Func<ColumnsBuilder, TColumns> columns)
        {
            var operation = new CreateMergeOperation(name, new List<AddColumnOperation>());

            var builder = new ColumnsBuilder(operation);
            var columnsObject = columns(builder);

            foreach (var property in typeof(TColumns).GetTypeInfo().DeclaredProperties)
            {
                var addColumnOperation = ((AddColumnOperation)property.GetMethod!.Invoke(columnsObject, null)!);
                addColumnOperation.Name = property.Name;
                operation.Columns.Add(addColumnOperation);
            }
            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<CreateMergeOperation>(operation);
        }

        public static OperationBuilder<DropMergeOperation> DropMerge(this MigrationBuilder migrationBuilder, string name)
        {
            var operation = new DropMergeOperation(name);
            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<DropMergeOperation>(operation);
        }
    }
}
