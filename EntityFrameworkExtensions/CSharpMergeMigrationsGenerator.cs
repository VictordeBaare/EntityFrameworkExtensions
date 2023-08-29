using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExtensions
{
    public class CSharpMergeMigrationsGenerator : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpMigrationsGenerator
    {
        public CSharpMergeMigrationsGenerator(MigrationsCodeGeneratorDependencies dependencies, CSharpMigrationsGeneratorDependencies csharpDependencies) : base(dependencies, csharpDependencies)
        {
        }

        protected override IEnumerable<string> GetNamespaces(IEnumerable<MigrationOperation> operations) => base.GetNamespaces(operations).Concat(new List<string> { typeof(MergeOptionsBuilderExtensions).Namespace });
    }
}
