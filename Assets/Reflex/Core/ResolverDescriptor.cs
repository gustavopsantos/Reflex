using System;
using System.Linq;
using Reflex.Resolvers;

namespace Reflex.Core
{
    internal class ResolverDescriptor : IEquatable<ResolverDescriptor>
    {
        public Resolver Resolver { get; }
        public Type[] Contracts { get; }

        public ResolverDescriptor(Resolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }

		public bool Equals(ResolverDescriptor other)
		{
			return 
                Contracts.SequenceEqual(other.Contracts) && 
                Resolver.Concrete == other.Resolver.Concrete;
		}
	}
}