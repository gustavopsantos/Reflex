using System;

using JetBrains.Annotations;

namespace Reflex.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SourceGeneratorInjectableAttribute : Attribute
    {
    }
}