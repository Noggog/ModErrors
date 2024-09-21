﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;
namespace Mutagen.Bethesda.Analyzers.Skyrim.Record.Placed.Object;

public class MultiBoundMarkerAnalyzer : IIsolatedRecordAnalyzer<IPlacedObjectGetter>
{
    public static readonly TopicDefinition MultiBoundMarker = MutagenTopicBuilder.DevelopmentTopic(
            "MultiBound Marker Placement",
            Severity.Warning)
        .WithoutFormatting("Placed Object is a MultiBound Marker which doesn't work in Skyrim");

    public IEnumerable<TopicDefinition> Topics { get; } = [MultiBoundMarker];

    public RecordAnalyzerResult? AnalyzeRecord(IsolatedRecordAnalyzerParams<IPlacedObjectGetter> param)
    {
        var placedObject = param.Record;

        if (placedObject.Base.FormKey == FormKeys.SkyrimSE.Skyrim.Static.RoomMarker.FormKey)
        {
            return new RecordAnalyzerResult(
                RecordTopic.Create(
                    placedObject,
                    MultiBoundMarker.Format(),
                    x => x.Base));
        }

        return null;
    }
}