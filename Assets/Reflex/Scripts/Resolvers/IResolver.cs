using Reflex.Scripts;

namespace Reflex
{
	internal interface IResolver
	{
		object Resolve(IContainer container);
	}
}