using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Analyzers.Config.Analyzer;

public interface IAnalyzerConfigLookup
{
    DirectoryPath? DataDirectoryPath { get; }
    bool LoadOrderSetByDataDirectory { get; }
    IEnumerable<ModKey>? LoadOrderSetToMods { get; }
    FilePath? OutputFilePath { get; }
}

public interface IAnalyzerConfig : IAnalyzerConfigLookup
{
    void OverrideDataDirectory(DirectoryPath path);
    void OverrideLoadOrderSetByDataDirectory(bool value);
    void OverrideOutputFilePath(FilePath filePath);
    void OverrideLoadOrderSetToMods(IEnumerable<ModKey> mods);
}

public class AnalyzerConfig : IAnalyzerConfig
{
    public DirectoryPath? DataDirectoryPath { get; private set; }
    public bool LoadOrderSetByDataDirectory { get; private set; }
    public FilePath? OutputFilePath { get; private set; }
    public IEnumerable<ModKey>? LoadOrderSetToMods { get; private set; }

    public void OverrideDataDirectory(DirectoryPath path) => DataDirectoryPath = path;
    public void OverrideLoadOrderSetByDataDirectory(bool value) => LoadOrderSetByDataDirectory = value;
    public void OverrideOutputFilePath(FilePath filePath) => OutputFilePath = filePath;
    public void OverrideLoadOrderSetToMods(IEnumerable<ModKey> mods) => LoadOrderSetToMods = mods;
}
