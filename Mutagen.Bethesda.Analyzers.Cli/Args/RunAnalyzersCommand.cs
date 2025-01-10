﻿using CommandLine;
using Mutagen.Bethesda.Analyzers.Config.Topic;
using Mutagen.Bethesda.Analyzers.SDK.Topics;

namespace Mutagen.Bethesda.Analyzers.Cli.Args;

[Verb("run-analyzers", HelpText = "Run analyzers on a game installation")]
public class RunAnalyzersCommand : IMinimumSeverityConfiguration
{
    [Option('g', "GameRelease", Required = true, HelpText = "Game Release to target")]
    public GameRelease GameRelease { get; set; }

    [Option("PrintTopics", Required = false, HelpText = "Whether to print the topics being run")]
    public bool PrintTopics { get; set; } = false;

    [Option('s', "Severity", HelpText = "Minimum severity required in order to report")]
    public Severity MinimumSeverity { get; set; } = Severity.Suggestion;

    [Option('o', "OutputFilePath", HelpText = "Optional output file path to save the report")]
    public string? OutputFilePath { get; set; } = null;

    [Option("CustomDataFolder", HelpText = "Optional custom data folder to use for the analysis")]
    public string? CustomDataFolder { get; set; } = null;

    [Option('t', "NumThreads", HelpText = "Number of threads to use")]
    public int? NumThreads { get; set; }
}
