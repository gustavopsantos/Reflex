namespace Reflex
{
	internal interface IResolver
	{
		int Resolutions { get; }
		object Resolve(Container container);
	}
}