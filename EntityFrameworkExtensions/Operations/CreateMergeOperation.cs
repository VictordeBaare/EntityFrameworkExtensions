using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExtensions.Operations
{
    public class CreateMergeOperation : MigrationOperation
    {
        public CreateMergeOperation(string tableName, List<MergeProperty> mergeProperties)
        {
            TableName = tableName;
            MergeProperties = mergeProperties;
        }

        public string TableName { get; }

        public List<MergeProperty> MergeProperties { get; }
    }

    public class MergeProperty
    {
        public string Name { get; set; }

        public string DbType { get; set; }

        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }
    }
}
