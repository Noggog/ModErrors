using Mutagen.Bethesda.Analyzers.SDK.Drops;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.Analyzers.SDK.Analyzers;

/// <summary>
/// Object containing all the parameters available for a <see cref="IIsolatedRecordFrameAnalyzer{TMajor}"/>
/// </summary>
public readonly struct IsolatedRecordFrameAnalyzerParams<TMajor>
    where TMajor : IMajorRecordGetter
{
    public Type? AnalyzerType { get; init; }
    private readonly IReportDropbox _reportDropbox;
    private readonly IModGetter _mod;
    private readonly ReportContextParameters _parameters;

    public readonly MajorRecordFrame Frame;

    internal IsolatedRecordFrameAnalyzerParams(IReportDropbox reportDropbox,
        IModGetter mod,
        ReportContextParameters parameters,
        MajorRecordFrame frame)
    {
        _reportDropbox = reportDropbox;
        _mod = mod;
        _parameters = parameters;
        Frame = frame;
    }

    /// <summary>
    /// Reports a topic to the engine
    /// </summary>
    public void AddTopic(
        IFormattedTopicDefinition formattedTopicDefinition,
        params (string Name, object Value)[] metaData)
    {
        string? editorId = null;
        if (Frame.TryFindSubrecord(RecordTypes.EDID, out var edid))
        {
            editorId = edid.AsString(GameConstants.Get(_mod.GameRelease).Encodings.NonLocalized);
        }

        _reportDropbox.Dropoff(
            _parameters,
            _mod.ModKey,
            new MajorRecordIdentifier
            {
                FormKey = new FormKey(_mod.ModKey, Frame.FormID.Id(_mod.MasterStyle)),
                EditorID = editorId,
            },
            Topic.Create(formattedTopicDefinition, AnalyzerType, metaData));
    }
}

/// <summary>
/// Object containing all the parameters available for a <see cref="IIsolatedRecordFrameAnalyzer"/>
/// </summary>
public readonly struct IsolatedRecordFrameAnalyzerParams
{
    private readonly IReportDropbox _reportDropbox;
    private readonly IModGetter _mod;
    private readonly ReportContextParameters _parameters;
    public readonly Type AnalyzerType;

    public readonly MajorRecordFrame Frame;

    internal IsolatedRecordFrameAnalyzerParams(
        IModGetter mod,
        IReportDropbox reportDropbox,
        ReportContextParameters parameters,
        MajorRecordFrame frame,
        Type analyzerType)
    {
        AnalyzerType = analyzerType;
        _mod = mod;
        _reportDropbox = reportDropbox;
        _parameters = parameters;
        Frame = frame;
    }

    /// <summary>
    /// Reports a topic to the engine
    /// </summary>
    public void AddTopic(
        IFormattedTopicDefinition formattedTopicDefinition,
        params (string Name, object Value)[] metaData)
    {
        string? editorId = null;
        if (Frame.TryFindSubrecord(RecordTypes.EDID, out var edid))
        {
            editorId = edid.AsString(GameConstants.Get(_mod.GameRelease).Encodings.NonLocalized);
        }

        _reportDropbox.Dropoff(
            _parameters,
            _mod.ModKey,
            new MajorRecordIdentifier
            {
                FormKey = new FormKey(_mod.ModKey, Frame.FormID.Id(_mod.MasterStyle)),
                EditorID = editorId,
            },
            Topic.Create(formattedTopicDefinition, AnalyzerType, metaData));
    }
}
