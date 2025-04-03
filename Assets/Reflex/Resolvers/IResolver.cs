using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    public interface IResolver : IDisposable
    {
        Lifetime Lifetime { get; }
        Resolution Resolution { get; }
        object Resolve(Container container);
    }
}