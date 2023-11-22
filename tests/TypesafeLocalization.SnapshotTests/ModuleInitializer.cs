using System.Runtime.CompilerServices;

namespace TypesafeLocalization.SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
