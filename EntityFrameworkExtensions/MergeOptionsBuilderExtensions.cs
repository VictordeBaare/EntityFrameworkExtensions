using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace EntityFrameworkExtensions
{
    public static class MergeOptionsBuilderExtensions
    {
        public static OperationBuilder<CreateMergeOperation> CreateMerge(this MigrationBuilder migrationBuilder, string name)
        {
            var operation = new CreateMergeOperation(name);
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
