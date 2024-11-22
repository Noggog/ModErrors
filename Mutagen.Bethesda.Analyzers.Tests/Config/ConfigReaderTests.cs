﻿using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mutagen.Bethesda.Analyzers.Config;
using Xunit;

namespace Mutagen.Bethesda.Analyzers.Tests.Config;

public record TestConfig;

public class FuncProcessor : IConfigReaderProcessor<TestConfig>
{
    public Func<TestConfig, IReadOnlyList<string>, string, bool> Func { get; set; } = (_, _, _) => true;

    public bool Process(TestConfig config, IReadOnlyList<string> instructionParts, string value)
    {
        return Func(config, instructionParts, value);
    }
}

[method: SuppressMessage("ReSharper", "ContextualLoggerProblem", Justification = "Passed in")]
public class TestConfigReader(ILogger<ConfigReader<TestConfig>> logger, FuncProcessor processor)
{
    public FuncProcessor Processor { get; } = processor;
    public readonly IConfigReader<TestConfig> Reader = new ConfigReader<TestConfig>(logger, [processor]);
}

public class ConfigReaderTests
{
    [Theory]
    [AnalyzerInlineData("other.A123.severity = Warning")]
    public void TestThreePartAndValue(
        string line,
        TestConfigReader sut)
    {
        var ran = false;

        sut.Processor.Func = (_, parts, value) =>
        {
            ran = true;
            parts.Should().HaveCount(3);
            parts[0].Should().Be("other");
            parts[1].Should().Be("A123");
            parts[2].Should().Be("severity");
            value.Should().Be("Warning");
            return true;
        };

        sut.Reader.ReadInto(line.AsSpan(), new TestConfig());

        ran.Should().BeTrue();
    }

    [Theory]
    [AnalyzerInlineData("diagnostic.A123.severity = Warning And Gibberish")]
    public void TestThreePartAndValueWithSpaces(
        string line,
        TestConfigReader sut)
    {
        var ran = false;

        sut.Processor.Func = (_, parts, value) =>
        {
            ran = true;
            parts.Should().HaveCount(3);
            parts[0].Should().Be("diagnostic");
            parts[1].Should().Be("A123");
            parts[2].Should().Be("severity");
            value.Should().Be("Warning And Gibberish");
            return true;
        };

        sut.Reader.ReadInto(line.AsSpan(), new TestConfig());

        ran.Should().BeTrue();
    }

    [Theory]
    [AnalyzerInlineData("diagnostic.A123")]
    public void TestSkipNoValue(
        string line,
        TestConfigReader sut)
    {
        var ran = false;

        sut.Processor.Func = (_, parts, value) =>
        {
            ran = true;
            parts.Should().HaveCount(3);
            parts[0].Should().Be("diagnostic");
            parts[1].Should().Be("A123");
            value.Should().Be("");
            return true;
        };

        sut.Reader.ReadInto(line.AsSpan(), new TestConfig());

        ran.Should().BeFalse();
    }

    [Theory]
    [AnalyzerInlineData("diagnostic.A123.severity = Other #test")]
    public void TestComment(
        string line,
        TestConfigReader sut)
    {
        var ran = false;

        sut.Processor.Func = (_, parts, value) =>
        {
            ran = true;
            parts.Should().HaveCount(3);
            parts[0].Should().Be("diagnostic");
            parts[1].Should().Be("A123");
            parts[2].Should().Be("severity");
            value.Should().Be("Other");
            return true;
        };

        sut.Reader.ReadInto(line.AsSpan(), new TestConfig());

        ran.Should().BeTrue();
    }

    [Theory]
    [AnalyzerInlineData("")]
    [AnalyzerInlineData("#diagnostic.A123.severity = Warning")]
    public void TestSkipNoContent(
        string line,
        TestConfigReader sut)
    {
        var ran = false;

        sut.Processor.Func = (_, parts, _) =>
        {
            ran = true;
            parts.Should().HaveCount(0);
            return true;
        };

        sut.Reader.ReadInto(line.AsSpan(), new TestConfig());

        ran.Should().BeFalse();
    }
}
