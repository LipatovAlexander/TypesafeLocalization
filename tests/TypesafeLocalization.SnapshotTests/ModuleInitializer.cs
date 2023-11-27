using System.Runtime.CompilerServices;
using DiffEngine;

namespace TypesafeLocalization.SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffTools.UseOrder(DiffTool.VisualStudioCode);
        VerifySourceGenerators.Initialize();
    }
}
