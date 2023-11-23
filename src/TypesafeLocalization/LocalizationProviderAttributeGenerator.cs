﻿using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

[Generator]
public sealed class LocalizationProviderAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("LocalizationProviderAttribute.g.cs", SourceGenerationHelper.LocalizationProviderAttribute()));
    }
}
