using Reflex.Scripts;

namespace Reflex
{
	internal abstract class Resolver
	{
		internal abstract object Resolve(IContainer container);
	}
}