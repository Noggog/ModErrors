﻿using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Analyzers.SDK.Results;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Analyzers.Skyrim
{
    public partial class MissingAssetsAnalyzer : IRecordAnalyzer<IStaticGetter>
    {
        public static readonly TopicDefinition<string> MissingStaticModel = MutagenTopicBuilder.FromDiscussion(
                90,
                "Missing Static Model file",
                Severity.Error)
            .WithFormatting<string>(MissingModelFileMessageFormat);

        public MajorRecordAnalyzerResult AnalyzeRecord(IRecordAnalyzerParams<IStaticGetter> param)
        {
            var res = new MajorRecordAnalyzerResult();
            CheckForMissingModelAsset(param.Record, res, MissingStaticModel);
            return res;
        }
    }
}
