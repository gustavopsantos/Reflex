using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal interface IResolver : IDisposable
    {
        Lifetime Lifetime { get; }
        object Resolve(Container container);
    }
}