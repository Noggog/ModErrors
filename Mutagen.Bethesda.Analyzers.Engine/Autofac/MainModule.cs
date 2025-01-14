using Autofac;
using Mutagen.Bethesda.Analyzers.Config;
using Mutagen.Bethesda.Analyzers.Drivers;
using Mutagen.Bethesda.Analyzers.Engines;
using Mutagen.Bethesda.Autofac;
using Noggog.Autofac;
using Noggog.Autofac.Modules;

namespace Mutagen.Bethesda.Analyzers.Autofac;

public class MainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<NoggogModule>();
        builder.RegisterModule<MutagenModule>();
        builder.RegisterModule<ReflectionDriverModule>();
        builder.RegisterAssemblyTypes(typeof(IsolatedEngine).Assembly)
            .InNamespacesOf(
                typeof(ContextualEngine),
                typeof(ConfigReader<>))
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
        builder.RegisterType<ContextualDriver>()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterGeneric(typeof(InjectionDriverProvider<>))
            .As(typeof(IDriverProvider<>))
            .SingleInstance();
        builder.RegisterGeneric(typeof(ConfigReader<>))
            .As(typeof(ConfigReader<>));
        builder.RegisterGeneric(typeof(FilteredAnalyzerProvider<>))
            .As(typeof(IAnalyzerProvider<>));
    }
}
