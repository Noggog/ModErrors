﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;
namespace Mutagen.Bethesda.Analyzers.Skyrim.Record.Static;

public class MissingFieldsAnalyzer : IIsolatedRecordAnalyzer<IStaticGetter>
{
    public static readonly TopicDefinition MissingLod = MutagenTopicBuilder.DevelopmentTopic(
            "Missing LOD",
            Severity.Suggestion)
        .WithoutFormatting("Static has LOD flag but no LOD models");

    public IEnumerable<TopicDefinition> Topics { get; } = [MissingLod];

    public RecordAnalyzerResult? AnalyzeRecord(IsolatedRecordAnalyzerParams<IStaticGetter> param)
    {
        var @static = param.Record;

        if (@static.MajorFlags.HasFlag(Bethesda.Skyrim.Static.MajorFlag.HasDistantLOD) && @static.Lod is null)
        {
            return new RecordAnalyzerResult(
                RecordTopic.Create(
                    @static,
                    MissingLod.Format(),
                    x => x.Lod));
        }

        return null;
    }
}
