using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace EntityFrameworkExtensions
{
    public class MergeMigrationsModelDiffer : MigrationsModelDiffer
    {
        public MergeMigrationsModelDiffer(IRelationalTypeMappingSource typeMappingSource,
            IMigrationsAnnotationProvider migrationsAnnotationProvider,
            IRowIdentityMapFactory rowIdentityMapFactory,
            CommandBatchPreparerDependencies commandBatchPreparerDependencies)
            : base(typeMappingSource, migrationsAnnotationProvider, rowIdentityMapFactory, commandBatchPreparerDependencies)
        {
        }

        public override IReadOnlyList<MigrationOperation> GetDifferences(IRelationalModel? source, IRelationalModel? target)
        {
            var sourceTypes = GetEntityTypesContainingMergeAnnotation(source);
            var targetTypes = GetEntityTypesContainingMergeAnnotation(target);

            var diffContext = new DiffContext();
            var mergeMigrationOperations = Diff(sourceTypes, targetTypes, diffContext);
            return base.GetDifferences(source, target).Concat(mergeMigrationOperations).ToList();
        }

        private IEnumerable<MigrationOperation> Diff(
            IEnumerable<IEntityType> source,
            IEnumerable<IEntityType> target,
            DiffContext diffContext)
            => DiffCollection(source, target, diffContext, Diff, Add, Remove, (x, y, diff) => x.Name.Equals(y.Name, StringComparison.CurrentCultureIgnoreCase));

        private IEnumerable<MigrationOperation> Remove(IEntityType source, DiffContext context)
        {
            yield return new DropMergeOperation(source.Name);
        }

        private IEnumerable<MigrationOperation> Add(IEntityType target, DiffContext context)
        {
            yield return new CreateMergeOperation(target.GetDefaultTableName(), target.GetProperties().Select(x => new MergeProperty { DbType = x.GetColumnType(), Name = x.GetColumnName(), IsNullable = x.IsNullable, IsForeignKey = x.IsForeignKey(), IsPrimaryKey = x.IsPrimaryKey() }).ToList());
        }

        private IEnumerable<MigrationOperation> Diff(IEntityType source, IEntityType target, DiffContext context)
        {
            if (source == target)
            {
                yield break;
            }
            var dropOperations = Remove(source, context);
            foreach (var operation in dropOperations)
            {
                yield return operation;
            }

            var addOperations = Add(target, context);
            foreach (var operation in addOperations)
            {
                yield return operation;
            }
        }

        private static List<IEntityType>? GetEntityTypesContainingMergeAnnotation(IRelationalModel? relationalModel)
        {
            if (relationalModel == null)
            {
                return new List<IEntityType>();
            }

            return relationalModel.Model.GetEntityTypes().Where(x => x.GetAnnotations().Any(annotation => annotation.Name.Equals(Constants.Merge))).ToList();
        }
    }
}
