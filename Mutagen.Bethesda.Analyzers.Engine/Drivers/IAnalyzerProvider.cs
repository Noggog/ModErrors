using Mutagen.Bethesda.Analyzers.Config.Topic;
using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;

namespace Mutagen.Bethesda.Analyzers.Drivers;

public interface IAnalyzerProvider<out TAnalyzer>
    where TAnalyzer : IAnalyzer
{
    IEnumerable<TAnalyzer> GetAnalyzers();
}

public class FilteredAnalyzerProvider<TAnalyzer>(TAnalyzer[] analyzers, ISeverityLookup severityLookup) : IAnalyzerProvider<TAnalyzer>
    where TAnalyzer : IAnalyzer
{

    public IEnumerable<TAnalyzer> GetAnalyzers()
    {
        return analyzers
            .Where(a => a.Topics.Any(topic => severityLookup.LookupSeverity(topic) != Severity.None));
    }
}
