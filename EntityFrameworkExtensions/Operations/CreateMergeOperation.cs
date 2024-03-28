using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExtensions.Operations
{
    public class CreateMergeOperation : MigrationOperation
    {
        public CreateMergeOperation(string tableName, IEnumerable<AddColumnOperation> columns)
        {
            TableName = tableName;
            Columns = columns.ToList();
        }

        public string TableName { get; }

        public List<AddColumnOperation> Columns { get; }
    }
}
