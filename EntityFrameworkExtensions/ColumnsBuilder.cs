using EntityFrameworkExtensions.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExtensions
{
    public class ColumnsBuilder
    {
        private CreateMergeOperation _operation;

        public ColumnsBuilder(CreateMergeOperation operation)
        {
            _operation = operation;
        }

        public virtual AddColumnOperation Column<T>(
        string? type = null,
        bool? unicode = null,
        int? maxLength = null,
        bool rowVersion = false,
        string? name = null,
        bool nullable = false,
        object? defaultValue = null,
        string? defaultValueSql = null,
        string? computedColumnSql = null,
        bool? fixedLength = null,
        string? comment = null,
        string? collation = null,
        int? precision = null,
        int? scale = null,
        bool? stored = null)
        {
            var operation = new AddColumnOperation
            {
                Table = _operation.TableName,
                Name = name!,
                ClrType = typeof(T),
                ColumnType = type,
                IsUnicode = unicode,
                MaxLength = maxLength,
                IsRowVersion = rowVersion,
                IsNullable = nullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql,
                IsFixedLength = fixedLength,
                Comment = comment,
                Collation = collation,
                Precision = precision,
                Scale = scale,
                IsStored = stored
            };
            return operation;
        }
    }
}
