using System;

using JetBrains.Annotations;

namespace Reflex.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Class)]
    public class SourceGeneratorInjectableAttribute : Attribute
    {
    }
}