using System;
using Reflex.Core;

namespace Reflex.Compatibility.OldAPI
{
    public static class OldAPICompatibility
    {
        public static void AddSingleton(this ContainerBuilder builder, Type concrete, params Type[] contracts)
        {
            builder.RegisterType(concrete, contracts, Enums.Lifetime.Singleton, Enums.Resolution.Lazy);
        }

        public static void AddSingleton(this ContainerBuilder builder, object instance, params Type[] contracts)
        {
            builder.RegisterValue(instance, contracts);
        }

        public static void AddSingleton<T>(this ContainerBuilder builder, Func<Container, T> factory, params Type[] contracts)
        {
            builder.RegisterFactory(factory, contracts, Enums.Lifetime.Singleton, Enums.Resolution.Lazy);
        }

        public static void AddTransient(this ContainerBuilder builder, Type concrete, params Type[] contracts)
        {
            builder.RegisterType(concrete, contracts, Enums.Lifetime.Transient, Enums.Resolution.Lazy);
        }

        public static void AddTransient(this ContainerBuilder builder, object instance, params Type[] contracts)
        {
            builder.RegisterValue(instance, contracts);
        }

        public static void AddTransient<T>(this ContainerBuilder builder, Func<Container, T> factory, params Type[] contracts)
        {
            builder.RegisterFactory(factory, contracts, Enums.Lifetime.Transient, Enums.Resolution.Lazy);
        }

        public static void AddScoped(this ContainerBuilder builder, Type concrete, params Type[] contracts)
        {
            builder.RegisterType(concrete, contracts, Enums.Lifetime.Scoped, Enums.Resolution.Lazy);
        }

        public static void AddScoped(this ContainerBuilder builder, object instance, params Type[] contracts)
        {
            builder.RegisterValue(instance, contracts);
        }

        public static void AddScoped<T>(this ContainerBuilder builder, Func<Container, T> factory, params Type[] contracts)
        {
            builder.RegisterFactory(factory, contracts, Enums.Lifetime.Scoped, Enums.Resolution.Lazy);
        }
    }
}