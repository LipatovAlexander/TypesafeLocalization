namespace TypesafeLocalization.Extensions;

public static class EnumerableExtensions
{
    public static IList<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> sources)
    {
        HashSet<T>? hashSet = null;
        foreach (var source in sources)
        {
            if (hashSet == null)
            {
                hashSet = new HashSet<T>(source);
            }
            else
            {
                hashSet.IntersectWith(source);
            }
        }

        return hashSet == null ? new List<T>() : hashSet.ToList();
    }
}
