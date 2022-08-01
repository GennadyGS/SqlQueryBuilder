namespace System.Runtime.CompilerServices;

/// <summary>
/// Indicates the attributed type is to be used as an interpolated string handler.
/// </summary>
/// <remarks>Required to be defined explicitly due to using netstandard2.0
/// rather than net6.0. Should be removed after migration to net6.0.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class InterpolatedStringHandlerAttribute : Attribute
{
}