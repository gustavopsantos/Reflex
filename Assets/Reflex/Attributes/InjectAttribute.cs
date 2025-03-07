using System;
using JetBrains.Annotations;

namespace Reflex.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
        public string Identifier { get; } = null;
        
        public bool HasIdentifier => !string.IsNullOrEmpty(Identifier);

        public InjectAttribute()
        {
            
        }

        public InjectAttribute([NotNull] string identifier)
        {
            Identifier = identifier;
        }
        
    }
}