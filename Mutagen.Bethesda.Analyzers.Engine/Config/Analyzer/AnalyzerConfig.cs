﻿using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Analyzers.Config.Analyzer;

public interface IAnalyzerConfigLookup
{
    DirectoryPath? DataDirectoryPath { get; }
    public bool LoadOrderSetByDataDirectory { get; }
    public IEnumerable<ModKey>? LoadOrderSetToMods { get; }
    GameRelease? GameRelease { get; }
    public FilePath? OutputFilePath { get; }
}

public interface IAnalyzerConfig : IAnalyzerConfigLookup
{
    void OverrideDataDirectory(DirectoryPath path);
    void OverrideLoadOrderSetByDataDirectory(bool value);
    void OverrideGameRelease(GameRelease release);
    void OverrideOutputFilePath(FilePath filePath);
    void OverrideLoadOrderSetToMods(IEnumerable<ModKey> mods);
}

public class AnalyzerConfig : IAnalyzerConfig
{
    public DirectoryPath? DataDirectoryPath { get; private set; }
    public bool LoadOrderSetByDataDirectory { get; private set; }
    public GameRelease? GameRelease { get; private set; }
    public FilePath? OutputFilePath { get; private set; }
    public IEnumerable<ModKey>? LoadOrderSetToMods { get; private set; }

    public void OverrideDataDirectory(DirectoryPath path) => DataDirectoryPath = path;
    public void OverrideLoadOrderSetByDataDirectory(bool value) => LoadOrderSetByDataDirectory = value;
    public void OverrideGameRelease(GameRelease release) => GameRelease = release;
    public void OverrideOutputFilePath(FilePath filePath) => OutputFilePath = filePath;
    public void OverrideLoadOrderSetToMods(IEnumerable<ModKey> mods) => LoadOrderSetToMods = mods;
}
