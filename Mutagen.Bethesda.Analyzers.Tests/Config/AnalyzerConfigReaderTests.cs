using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mutagen.Bethesda.Analyzers.Config;
using Mutagen.Bethesda.Analyzers.Config.Analyzer;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Noggog.Testing.IO;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Analyzers.Tests.Config;

[method: SuppressMessage("ReSharper", "ContextualLoggerProblem", Justification = "Passed in")]
public class AnalyzerConfigReader(
    ILogger<ConfigReader<IAnalyzerConfig>> logger,
    ProcessDataDirectoryPath p1,
    ProcessLoadOrderSetByDataDirectory p2,
    ProcessLoadOrderSetToMods p3,
    ProcessOutputFilePath p4)
{
    public readonly ConfigReader<IAnalyzerConfig> Reader = new(logger, [p1, p2, p2, p3, p4]);
}

public class AnalyzerConfigReaderTests
{
    [Theory, AnalyzerAutoData]
    public void TestDataDirectoryForwardSlash(
        AnalyzerConfig config,
        AnalyzerConfigReader sut)
    {
        var line = $"environment.data_directory = {PathingUtil.DrivePrefix}some/path";
        sut.Reader.ReadInto(line.AsSpan(), config);
        config.DataDirectoryPath.Should().NotBeNull();
        config.DataDirectoryPath!.Value.Path.Should().Be(Path.Combine(PathingUtil.DrivePrefix, "some", "path"));
    }

    [Theory, AnalyzerAutoData]
    public void TestDataDirectoryBackwardSlash(
        AnalyzerConfig config,
        AnalyzerConfigReader sut)
    {
        var line = @$"environment.data_directory = {PathingUtil.DrivePrefix}some\path";
        sut.Reader.ReadInto(line.AsSpan(), config);
        config.DataDirectoryPath.Should().NotBeNull();
        config.DataDirectoryPath!.Value.Path.Should().Be(Path.Combine(PathingUtil.DrivePrefix, "some", "path"));
    }

    [Theory]
    [AnalyzerInlineData("environment.load_order.set_by_data_directory = true")]
    [AnalyzerInlineData("environment.load_order.set_by_data_directory = false")]
    public void TestSetByDataDirectory(
        string line,
        IAnalyzerConfig config,
        AnalyzerConfigReader sut)
    {
        sut.Reader.ReadInto(line.AsSpan(), config);
        config.Received(1).OverrideLoadOrderSetByDataDirectory(Arg.Any<bool>());
    }

    [Theory]
    [AnalyzerInlineData("environment.load_order.set_to_mods = Skyrim.esm, Update.esm")]
    public void TestSetToMods(
        string line,
        AnalyzerConfig config,
        AnalyzerConfigReader sut)
    {
        sut.Reader.ReadInto(line.AsSpan(), config);
        config.LoadOrderSetToMods.Should().BeEquivalentTo([FormKeys.SkyrimSE.Skyrim.ModKey, Update.ModKey]);
    }

    [Theory, AnalyzerAutoData]
    public void TestOutputFilePathForwardSlash(
        AnalyzerConfig config,
        AnalyzerConfigReader sut)
    {
        var line = $"output_file = {PathingUtil.DrivePrefix}some/path";
        sut.Reader.ReadInto(line.AsSpan(), config);
        config.OutputFilePath.Should().NotBeNull();
        config.OutputFilePath!.Value.Path.Should().Be(Path.Combine(PathingUtil.DrivePrefix, "some", "path"));
    }

    [Theory, AnalyzerAutoData]
    public void TestOutputFilePathBackwardSlash(
        AnalyzerConfig config,
        AnalyzerConfigReader sut)
    {
        var line = $@"output_file = {PathingUtil.DrivePrefix}some\path";
        sut.Reader.ReadInto(line.AsSpan(), config);
        config.OutputFilePath.Should().NotBeNull();
        config.OutputFilePath!.Value.Path.Should().Be(Path.Combine(PathingUtil.DrivePrefix, "some", "path"));
    }
}
