﻿using Autofac;
using Mutagen.Bethesda.Analyzers.Autofac;
using Mutagen.Bethesda.Analyzers.Reporting.Drops;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Analyzers.SDK.Drops;

namespace Mutagen.Bethesda.Analyzers.Cli.Modules;

public class RunAnalyzerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Last registered runs first
        builder.RegisterType<PassToHandlerReportDropbox>().AsImplementedInterfaces();
        builder.RegisterDecorator<EditorIdEnricher, IReportDropbox>();
        builder.RegisterDecorator<MinimumSeverityFilter, IReportDropbox>();
        builder.RegisterDecorator<SeverityAdjuster, IReportDropbox>();
        builder.RegisterDecorator<DisallowedParametersChecker, IReportDropbox>();

        builder.RegisterType<ConsoleReportHandler>().AsImplementedInterfaces();

        builder.RegisterModule<MainModule>();
    }
}
