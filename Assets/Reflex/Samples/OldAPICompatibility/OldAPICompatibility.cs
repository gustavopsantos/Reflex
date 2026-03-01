using System;
using Reflex.Core;

namespace Reflex.Compatibility.OldAPI
{
    public static class OldAPICompatibility
    {
        public static ContainerBuilder AddSingleton(this ContainerBuilder builder, Type concrete, params Type[] contracts)
        {
            return builder.RegisterType(concrete, contracts, Enums.Lifetime.Singleton, Enums.Resolution.Lazy);
        }

        public static ContainerBuilder AddSingleton(this ContainerBuilder builder, object instance, params Type[] contracts)
        {
            return builder.RegisterValue(instance, contracts);
        }

        public static ContainerBuilder AddSingleton<T>(this ContainerBuilder builder, Func<Container, T> factory, params Type[] contracts)
        {
            return builder.RegisterFactory(factory, contracts, Enums.Lifetime.Singleton, Enums.Resolution.Lazy);
        }

        public static ContainerBuilder AddTransient(this ContainerBuilder builder, Type concrete, params Type[] contracts)
        {
            return builder.RegisterType(concrete, contracts, Enums.Lifetime.Transient, Enums.Resolution.Lazy);
        }

        public static ContainerBuilder AddTransient(this ContainerBuilder builder, object instance, params Type[] contracts)
        {
            return builder.RegisterValue(instance, contracts);
        }

        public static ContainerBuilder AddTransient<T>(this ContainerBuilder builder, Func<Container, T> factory, params Type[] contracts)
        {
            return builder.RegisterFactory(factory, contracts, Enums.Lifetime.Transient, Enums.Resolution.Lazy);
        }

        public static ContainerBuilder AddScoped(this ContainerBuilder builder, Type concrete, params Type[] contracts)
        {
            return builder.RegisterType(concrete, contracts, Enums.Lifetime.Scoped, Enums.Resolution.Lazy);
        }

        public static ContainerBuilder AddScoped(this ContainerBuilder builder, object instance, params Type[] contracts)
        {
            return builder.RegisterValue(instance, contracts);
        }

        public static ContainerBuilder AddScoped<T>(this ContainerBuilder builder, Func<Container, T> factory, params Type[] contracts)
        {
            return builder.RegisterFactory(factory, contracts, Enums.Lifetime.Scoped, Enums.Resolution.Lazy);
        }
    }
}