using System;

namespace Reflex
{
	internal static class TypeInfoCache
	{
		internal static readonly LazyDictionary<Type, TypeInfo> Cache = new LazyDictionary<Type, TypeInfo>(BakeTypeInfo.Bake);
	}
}