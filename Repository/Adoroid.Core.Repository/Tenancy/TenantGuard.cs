namespace Adoroid.Core.Repository.Tenancy;

internal static class TenantGuard
{
    public static bool IsEmpty<T>(T? value)
    {
        if (value is null)
            return true;

        if (value is string text)
            return string.IsNullOrWhiteSpace(text);

        return EqualityComparer<T>.Default.Equals(value, default!);
    }

    public static bool AreEqual<T>(T? left, T? right) => EqualityComparer<T>.Default.Equals(left!, right!);
}
