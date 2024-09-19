﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace Mutagen.Bethesda.Analyzers.Skyrim.Record.Armor;

public class KeywordArmorTypeAnalyzer : IIsolatedRecordAnalyzer<IArmorGetter>
{
    public static readonly TopicDefinition<string, string> ArmorMatchingKeywordArmorType = MutagenTopicBuilder.FromDiscussion(
            0,
            "Armor keywords don't match their equipped armor type",
            Severity.Suggestion)
        .WithFormatting<string, string>("Has armor type {0} but doesn't have keyword {1}");

    public IEnumerable<TopicDefinition> Topics => [ArmorMatchingKeywordArmorType];

    public RecordAnalyzerResult? AnalyzeRecord(IsolatedRecordAnalyzerParams<IArmorGetter> param)
    {
        var armor = param.Record;

        // Armor with template armor inherit all relevant data from the template armor and should not be checked themselves
        if (!armor.TemplateArmor.IsNull) return null;

        // Armor with no slots are not relevant
        if (armor.BodyTemplate is null) return null;

        // Ignore armor with no keywords, these are usually skin armor
        if (armor.Keywords is null) return null;

        // Shields are always have the same keyword ArmorShield
        if (armor.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Shield)) return null;

        List<FormLink<IKeywordGetter>> matchingKeywords = armor.BodyTemplate.ArmorType switch
        {
            ArmorType.LightArmor => [FormKeys.SkyrimSE.Skyrim.Keyword.ArmorLight],
            ArmorType.HeavyArmor => [FormKeys.SkyrimSE.Skyrim.Keyword.ArmorHeavy],
            ArmorType.Clothing => [FormKeys.SkyrimSE.Skyrim.Keyword.ArmorClothing, FormKeys.SkyrimSE.Skyrim.Keyword.ArmorJewelry],
            _ => throw new InvalidOperationException()
        };

        foreach (var keyword in matchingKeywords)
        {
            if (armor.Keywords.Contains(keyword)) return null;
        }

        return new RecordAnalyzerResult(
            RecordTopic.Create(
                armor,
                ArmorMatchingKeywordArmorType.Format(armor.BodyTemplate.ArmorType.ToString(), string.Join(", ", matchingKeywords)),
                x => x.Keywords));
    }
}