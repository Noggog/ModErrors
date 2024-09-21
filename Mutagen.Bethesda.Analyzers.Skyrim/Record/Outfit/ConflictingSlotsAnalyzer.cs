﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace Mutagen.Bethesda.Analyzers.Skyrim.Record.Outfit;

public class ConflictingSlotsAnalyzer : IContextualRecordAnalyzer<IOutfitGetter>
{
    public static readonly TopicDefinition<Dictionary<string, List<IArmorGetter>>, BipedObjectFlag> ConflictingSlotsTopic = MutagenTopicBuilder.DevelopmentTopic(
            "Conflicting Slots",
            Severity.Warning)
        .WithFormatting<Dictionary<string, List<IArmorGetter>>, BipedObjectFlag>("Outfit entries {0} are conflicting in slot {1}");

    public IEnumerable<TopicDefinition> Topics { get; } = [ConflictingSlotsTopic];

    public RecordAnalyzerResult AnalyzeRecord(ContextualRecordAnalyzerParams<IOutfitGetter> param)
    {
        var outfit = param.Record;
        var result = new RecordAnalyzerResult();

        if (outfit.Items is null) return result;

        var armorsEntriesPerSlot = outfit.Items
            .SelectMany<IFormLinkGetter<IOutfitTargetGetter>,(IMajorRecordGetter Entry, BipedObjectFlag Slot, IArmorGetter Armor)>(item => item.TryResolve(param.LinkCache) switch
            {
                IArmorGetter armor => armor.GetSlots().Select(slot => ((IMajorRecordGetter) armor, slot, armor)),
                ILeveledItemGetter leveledItem => leveledItem.GetItems<IArmorGetter>(param.LinkCache).SelectMany(armor => armor.GetSlots().Select(slot => ((IMajorRecordGetter) leveledItem, slot, armor))),
                _ => []
            })
            .GroupBy(x => x.Slot)
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var (slot, entries) in armorsEntriesPerSlot)
        {
            var separateEntriesOccupyingSlots = entries.GroupBy(x => x.Entry.FormKey).ToList();
            if (separateEntriesOccupyingSlots.Count <= 1) continue;

            result.AddTopic(
                RecordTopic.Create(
                    outfit,
                    ConflictingSlotsTopic.Format(
                        separateEntriesOccupyingSlots.ToDictionary(x => x.First().Entry.EditorID ?? x.First().Entry.FormKey.ToString(), x => x.Select(x => x.Armor).ToList()),
                        slot),
                    x => x.Items));
        }

        return result;
    }
}