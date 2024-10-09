﻿namespace Mutagen.Bethesda.Analyzers.SDK.Topics;

public record Topic
{
    public static Topic Create(
        IFormattedTopicDefinition formattedTopicDefinition,
        (string Name, object Value)[] metaData)
    {
        return new Topic
        {
            FormattedTopic = formattedTopicDefinition,
            Severity = formattedTopicDefinition.TopicDefinition.Severity,
            MetaData = metaData
        };
    }

    public TopicDefinition TopicDefinition => FormattedTopic.TopicDefinition;
    public required IFormattedTopicDefinition FormattedTopic { get; init; }
    public required Severity Severity { get; init; }
    public required (string Name, object Value)[] MetaData { get; init; }
}
