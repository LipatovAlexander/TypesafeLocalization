﻿namespace TypesafeLocalization.LightJson.Serialization;

// ReSharper disable NotAccessedField.Global

/// <summary>
/// Represents a position within a plain text resource.
/// </summary>
public struct TextPosition
{
    /// <summary>
    /// The column position, 0-based.
    /// </summary>
    public long Column;

    /// <summary>
    /// The line position, 0-based.
    /// </summary>
    public long Line;
}
