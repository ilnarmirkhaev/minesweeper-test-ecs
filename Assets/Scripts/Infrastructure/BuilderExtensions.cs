using Leopotam.EcsLite;
using VContainer;

namespace Infrastructure
{
    public static class BuilderExtensions
    {
        public static RegistrationBuilder RegisterSystem<T>(this IContainerBuilder builder) where T : IEcsSystem
        {
            return builder.Register<T>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        public static RegistrationBuilder RegisterPool<TComponent>(this IContainerBuilder builder)
            where TComponent : struct
        {
            return builder.Register<EcsPool<TComponent>>(resolver =>
            {
                return resolver.Resolve<EcsWorld>().GetPool<TComponent>();
            }, Lifetime.Scoped);
        }
    }
}