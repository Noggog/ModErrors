﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;
namespace Mutagen.Bethesda.Analyzers.Skyrim.Contextual;

public class NpcNotInCellFactionAnalyzer : IContextualAnalyzer
{
    public static readonly TopicDefinition<string?, string?> NpcNotInCellFaction = MutagenTopicBuilder.FromDiscussion(
            0,
            "Npc Not In Cell Faction",
            Severity.Suggestion)
        .WithFormatting<string?, string?>("Npc {0} is not in their cell owner faction {1}");

    public IEnumerable<TopicDefinition> Topics { get; } = [NpcNotInCellFaction];

    public ContextualAnalyzerResult Analyze(ContextualAnalyzerParams param)
    {
        var result = new ContextualAnalyzerResult();

        foreach (var cell in param.LinkCache.PriorityOrder.WinningOverrides<ICellGetter>())
        {
            if (cell.Owner.IsNull) continue;
            if (!param.LinkCache.TryResolve<IFactionGetter>(cell.Owner.FormKey, out var cellOwnerFaction)) continue;

            foreach (var placedNpc in cell.GetAllPlaced(param.LinkCache).OfType<IPlacedNpcGetter>())
            {
                var npc = placedNpc.Base.TryResolve(param.LinkCache);
                if (npc is null || !npc.IsUnique()) continue;

                // Skip npcs with cell owner faction
                if (npc.Factions.Any(r => r.Faction.FormKey == cellOwnerFaction.FormKey)) continue;

                // Skip if cell is inn and npc is not innkeeper or server
                if (!param.LinkCache.TryResolve<ILocationGetter>(cell.Location.FormKey, out var location)) continue;
                if (location.IsInnLocation() && !npc.HasFaction(
                        param.LinkCache,
                        editorId => editorId is not null
                                    && (editorId.Contains("JobInn", StringComparison.Ordinal)
                                        || editorId.Contains("JobBard", StringComparison.Ordinal))))
                {
                    continue;
                }

                // Skip prisoners
                if (npc.HasFaction(param.LinkCache, editorId => editorId is not null && editorId.Contains("Prisoner"))) continue;

                result.AddTopic(
                    ContextualTopic.Create(
                        placedNpc,
                        NpcNotInCellFaction.Format(npc.EditorID, cellOwnerFaction.EditorID)
                    )
                );
            }
        }

        return result;
    }
}