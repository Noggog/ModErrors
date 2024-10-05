﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Analyzers.Skyrim;

public partial class MissingAssetsAnalyzer : IIsolatedRecordAnalyzer<IArmorAddonGetter>
{
    public static readonly TopicDefinition<string, string?> MissingArmorAddonWorldModel = MutagenTopicBuilder.FromDiscussion(
            84,
            "Missing Armor Addon Model file",
            Severity.Error)
        .WithFormatting<string, string?>("Missing {0} Armor Addon Model file at {1}");

    public static readonly TopicDefinition<string, string?> MissingArmorAddonFirstPersonModel = MutagenTopicBuilder.FromDiscussion(
            85,
            "Missing Armor Addon 1st Person Model file",
            Severity.Error)
        .WithFormatting<string, string?>("Missing {0} 1st Person Armor Addon Model file at {1}");

    public void AnalyzeRecord(IsolatedRecordAnalyzerParams<IArmorAddonGetter> param)
    {
        var femaleWorldModel = param.Record.WorldModel?.Female?.File;
        if (!FileExistsIfNotNull(femaleWorldModel))
        {
            param.AddTopic(
                MissingArmorAddonWorldModel.Format("female", femaleWorldModel),
                x => x.WorldModel!.Female!.File);
        }

        var maleWorldModel = param.Record.WorldModel?.Male?.File;
        if (!FileExistsIfNotNull(maleWorldModel))
        {
            param.AddTopic(
                MissingArmorAddonWorldModel.Format("male", maleWorldModel),
                x => x.WorldModel!.Male!.File);
        }

        var femaleFirstPersonModel = param.Record.FirstPersonModel?.Female?.File;
        if (!FileExistsIfNotNull(femaleFirstPersonModel))
        {
            param.AddTopic(
                MissingArmorAddonFirstPersonModel.Format("female", femaleFirstPersonModel),
                x => x.FirstPersonModel!.Female!.File);
        }

        var maleFirstPersonModel = param.Record.FirstPersonModel?.Male?.File;
        if (!FileExistsIfNotNull(maleFirstPersonModel))
        {
            param.AddTopic(
                MissingArmorAddonFirstPersonModel.Format("male", maleFirstPersonModel),
                x => x.FirstPersonModel!.Male!.File);
        }
    }
}
