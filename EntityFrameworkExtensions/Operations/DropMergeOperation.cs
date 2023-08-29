using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExtensions.Operations
{
    public class DropMergeOperation : MigrationOperation
    {
        public DropMergeOperation(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
