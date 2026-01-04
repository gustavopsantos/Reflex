using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    public interface IResolver : IDisposable
    {
        Lifetime Lifetime { get; }
        Resolution Resolution { get; }
        Container DeclaringContainer { get; set; }
        object Resolve(Container resolvingContainer);
    }
}