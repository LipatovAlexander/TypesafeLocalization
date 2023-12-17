namespace TypesafeLocalization.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> FindDuplicates<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
    {
        var duplicates = source
            .GroupBy(selector)
            .SelectMany(g => g.Skip(1))
            .Distinct()
            .ToArray();

        return duplicates;
    }
}
