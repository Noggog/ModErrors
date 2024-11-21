﻿namespace Mutagen.Bethesda.Analyzers.Config;

public interface IConfigReaderProcessor<TConfig>
{
    bool Process(TConfig config, IReadOnlyList<string> instructionParts, string value);
}
