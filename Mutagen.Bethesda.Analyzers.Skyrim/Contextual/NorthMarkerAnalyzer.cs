﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;
namespace Mutagen.Bethesda.Analyzers.Skyrim.Contextual;

public class NorthMarkerAnalyzer : IContextualAnalyzer
{
    public static readonly TopicDefinition NoNorthMarker = MutagenTopicBuilder.FromDiscussion(
            0,
            "No North Marker",
            Severity.Suggestion)
        .WithoutFormatting("Missing north marker");

    public static readonly TopicDefinition<string> MoreThanOneNorthMarker = MutagenTopicBuilder.FromDiscussion(
            0,
            "More Than One North Marker",
            Severity.Suggestion)
        .WithFormatting<string>("Cell has multiple north markers {0} when only one is permitted");

    public IEnumerable<TopicDefinition> Topics { get; } = [NoNorthMarker, MoreThanOneNorthMarker];

    public ContextualAnalyzerResult? Analyze(ContextualAnalyzerParams param)
    {
        var result = new ContextualAnalyzerResult();

        foreach (var cell in param.LinkCache.PriorityOrder.WinningOverrides<ICellGetter>())
        {
            if (cell.IsExteriorCell()) continue;

            var northMarkers = cell.GetAllPlaced(param.LinkCache)
                .OfType<IPlacedObjectGetter>()
                .Where(placed => placed.Base.FormKey == FormKeys.SkyrimSE.Skyrim.Static.NorthMarker.FormKey)
                .ToArray();

            if (northMarkers.Length == 0)
            {
                result.AddTopic(
                    ContextualTopic.Create(
                        cell,
                        NoNorthMarker.Format()
                    ));
            }

            if (northMarkers.Length > 1)
            {
                result.AddTopic(
                    ContextualTopic.Create(
                        cell,
                        MoreThanOneNorthMarker.Format(string.Join(", ", northMarkers.Select(x => x.FormKey.ToString())))
                    ));
            }
        }

        return null;
    }
}