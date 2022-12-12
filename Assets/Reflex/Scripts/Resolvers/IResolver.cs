using System;

namespace Reflex
{
	internal interface IResolver : IDisposable
	{
		Type Concrete { get; }
		int Resolutions { get; }
		object Resolve(Container container);
	}
}