using System.IO.Abstractions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mutagen.Bethesda.Analyzers.Cli.Args;
using Mutagen.Bethesda.Analyzers.Cli.Modules;
using Mutagen.Bethesda.Analyzers.Cli.Overrides;
using Mutagen.Bethesda.Analyzers.Config.Analyzer;
using Mutagen.Bethesda.Analyzers.Engines;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Analyzers.Skyrim;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Analyzers.Cli;

public static class RunAnalyzers
{
    public static async Task<int> Run(RunAnalyzersCommand command)
    {
        var lifetimeScope = GetContainer(command);

        var engine = lifetimeScope.Resolve<ContextualEngine>();
        var consumer = lifetimeScope.Resolve<IWorkConsumer>();

        PrintTopics(command, engine);

        consumer.Start();
        await engine.Run(CancellationToken.None);

        return 0;
    }

    private static void PrintTopics(RunAnalyzersCommand command, ContextualEngine engine)
    {
        if (!command.PrintTopics) return;

        Console.WriteLine("Topics:");
        var sb = new StructuredStringBuilder();
        foreach (var topic in engine.Drivers
                     .SelectMany(d => d.Analyzers)
                     .SelectMany(a => a.Topics)
                     .Distinct(x => x.Id))
        {
            topic.Append(sb);
        }

        foreach (var line in sb)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine();
        Console.WriteLine();
    }

    private static ILifetimeScope GetContainer(RunAnalyzersCommand command)
    {
        var services = new ServiceCollection();
        services.AddLogging(x => x.AddConsole());

        var builder = new ContainerBuilder();
        builder.Populate(services);
        builder.RegisterInstance(new FileSystem())
            .As<IFileSystem>();
        builder.RegisterModule(new RunAnalyzerModule(command));
        builder.RegisterInstance(new GameReleaseInjection(command.GameRelease))
            .AsImplementedInterfaces();
        builder.RegisterType<ConsoleReportHandler>().AsImplementedInterfaces();
        builder.RegisterInstance(command).AsImplementedInterfaces();
        builder.RegisterInstance(new NumWorkThreadsConstant(command.NumThreads)).AsImplementedInterfaces();

        if (command.CustomDataFolder is not null)
        {
            var dataDirectoryProvider = new DataDirectoryInjection(command.CustomDataFolder);
            builder.RegisterInstance(dataDirectoryProvider).As<IDataDirectoryProvider>();
        }

        if (command.UseDataFolderForLoadOrder)
        {
            builder.RegisterType<DataDirectoryEnabledPluginListingsProvider>().As<IEnabledPluginListingsProvider>();
            builder.RegisterType<NullPluginListingsPathProvider>().As<IPluginListingsPathProvider>();
            builder.RegisterType<NullCreationClubListingsPathProvider>().As<ICreationClubListingsPathProvider>();
        }

        builder.RegisterModule<SkyrimAnalyzerModule>();

        var container = builder.Build();

        return container.BeginLifetimeScope(b =>
        {
            var analyzerConfigProvider = container.Resolve<AnalyzerConfigProvider>();
            var analyzerConfig = analyzerConfigProvider.Config;

            if (analyzerConfig.DataDirectoryPath.HasValue)
            {
                var dataDirectoryProvider = new DataDirectoryInjection(analyzerConfig.DataDirectoryPath.Value);
                b.RegisterInstance(dataDirectoryProvider).As<IDataDirectoryProvider>();
            }

            if (analyzerConfig.LoadOrderSetByDataDirectory)
            {
                b.RegisterType<DataDirectoryEnabledPluginListingsProvider>().As<IEnabledPluginListingsProvider>();
                b.RegisterType<NullPluginListingsPathProvider>().As<IPluginListingsPathProvider>();
                b.RegisterType<NullCreationClubListingsPathProvider>().As<ICreationClubListingsPathProvider>();
            }
            else if (analyzerConfig.LoadOrderSetToMods is not null)
            {
                b.RegisterInstance(new InjectedEnabledPluginListingsProvider(analyzerConfig.LoadOrderSetToMods)).As<IEnabledPluginListingsProvider>();
                b.RegisterType<NullPluginListingsPathProvider>().As<IPluginListingsPathProvider>();
                b.RegisterType<NullCreationClubListingsPathProvider>().As<ICreationClubListingsPathProvider>();
            }

            if (analyzerConfig.GameRelease.HasValue)
            {
                var gameReleaseInjection = new GameReleaseInjection(analyzerConfig.GameRelease.Value);
                b.RegisterInstance(gameReleaseInjection).AsImplementedInterfaces();
            }

            if (analyzerConfig.OutputFilePath is not null)
            {
                b.RegisterType<CsvReportHandler>().AsImplementedInterfaces();
                b.RegisterInstance(new CsvInputs(analyzerConfig.OutputFilePath)).AsSelf().AsImplementedInterfaces();
            }
        });
    }
}
