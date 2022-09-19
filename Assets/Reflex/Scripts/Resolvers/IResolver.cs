using System;

namespace Reflex
{
	internal interface IResolver
	{
		Type Concrete { get; }
		int Resolutions { get; }
		object Resolve(Container container);
	}
}