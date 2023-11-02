using System;
using Reflex.Core;

namespace Reflex.Resolvers
{
	internal sealed class ExtendResolverDecorator : Resolver
	{
		private readonly string _name;
		private readonly Action<ContainerDescriptor> _extend;
		private readonly Resolver _resolver;
		private Container _scope;
		
		public ExtendResolverDecorator(string name, Action<ContainerDescriptor> extend, Resolver resolver)
		{
			_name = name;
			_extend = extend;
			_resolver = resolver;
			Concrete = resolver.Concrete;
			RegisterCallSite();
		}

		public override object Resolve(Container container)
		{
			if (_scope == null)
			{
				_scope = container.Scope(_name, _extend);
			}

			return _resolver.Resolve(_scope);
		}
	}
}