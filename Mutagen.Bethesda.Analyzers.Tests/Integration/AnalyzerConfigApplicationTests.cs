using Autofac;
using FluentAssertions;
using Mutagen.Bethesda.Analyzers.Config;
using Mutagen.Bethesda.Analyzers.Engines;
using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Analyzers.Testing;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Analyzers.Tests.Integration;

public class AnalyzerConfigApplicationTests
{
    public static readonly TopicDefinition Suggestion = new(
        "A123",
        "TestTitle",
        Severity.Suggestion);
    public static readonly IContextualAnalyzer SuggestionAnalyzer;
    public static readonly TopicDefinition Warning = new(
        "A123",
        "TestTitle",
        Severity.Warning);
    public static readonly IContextualAnalyzer WarningAnalyzer;

    static AnalyzerConfigApplicationTests()
    {
        SuggestionAnalyzer = new TestAnalyzer(Suggestion);
        WarningAnalyzer = new TestAnalyzer(Warning);
    }

    public static async Task<TestDropoff> RunTest(Action<ContainerBuilder> containerAdjustment)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<TestModule>();
        containerAdjustment(builder);
        var container = builder.Build();
        var engine = container.Resolve<ContextualEngine>();
        await engine.Run();
        return container.Resolve<TestDropoff>();
    }

    [Theory]
    [AnalyzerAutoData]
    public async Task NoReturnActsNormally(IContextualAnalyzer testAnalyzer)
    {
        var dropoff = await RunTest(builder =>
        {
            testAnalyzer.Analyze(default);
            builder.RegisterInstance(testAnalyzer).AsSelf();
        });
        dropoff.Reports.Should().HaveCount(0);
    }

    [Fact]
    public async Task PassesDesiredSeverity()
    {
        var dropoff = await RunTest(builder =>
        {
            builder.RegisterInstance(WarningAnalyzer).AsImplementedInterfaces();
        });
        dropoff.Reports.Should().HaveCount(1);
    }

    [Fact]
    public async Task FiltersUndesiredSeverity()
    {
        var dropoff = await RunTest(builder =>
        {
            builder.RegisterInstance(SuggestionAnalyzer).AsImplementedInterfaces();
            var minSev = Substitute.For<IMinimumSeverityConfiguration>();
            minSev.MinimumSeverity.Returns(Severity.Warning);
            builder.RegisterInstance(minSev).As<IMinimumSeverityConfiguration>();
        });
        dropoff.Reports.Should().HaveCount(0);
    }

    [Fact]
    public async Task AdjustsTopicSeverity()
    {
        var dropoff = await RunTest(builder =>
        {
            builder.RegisterInstance(WarningAnalyzer).AsImplementedInterfaces();
            var minSev = Substitute.For<IMinimumSeverityConfiguration>();
            minSev.MinimumSeverity.Returns(Severity.Warning);
            builder.RegisterInstance(minSev).As<IMinimumSeverityConfiguration>();
            var sevLookup = Substitute.For<ISeverityLookup>();
            sevLookup.LookupSeverity(Warning).Returns(Severity.Suggestion);
            builder.RegisterInstance(sevLookup).As<ISeverityLookup>();
        });
        dropoff.Reports.Should().HaveCount(0);
    }
}
