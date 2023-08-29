using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkExtensions.Annotations
{
    public class MergeAnnotation : IAnnotation
    {
        public string Name => "Merge";

        public object? Value { get; }
    }
}
