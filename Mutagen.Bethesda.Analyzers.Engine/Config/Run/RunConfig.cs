using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Analyzers.Config.Run;

public interface IRunConfigLookup
{
    DirectoryPath? DataDirectoryPath { get; }
    IEnumerable<ModKey>? LoadOrderSetToMods { get; }
    FilePath? OutputFilePath { get; }
    bool PrintMetadata { get; set; }
}

public interface IRunConfig : IRunConfigLookup
{
    void OverrideDataDirectory(DirectoryPath path);
    void OverrideOutputFilePath(FilePath filePath);
    void OverrideLoadOrderSetToMods(IEnumerable<ModKey> mods);
    void OverridePrintMetadata(bool printMetadata);
}

public class RunConfig : IRunConfig
{
    public DirectoryPath? DataDirectoryPath { get; private set; }
    public FilePath? OutputFilePath { get; private set; }
    public bool PrintMetadata { get; set; }
    public IEnumerable<ModKey>? LoadOrderSetToMods { get; private set; }

    public void OverrideDataDirectory(DirectoryPath path) => DataDirectoryPath = path;
    public void OverrideOutputFilePath(FilePath filePath) => OutputFilePath = filePath;
    public void OverrideLoadOrderSetToMods(IEnumerable<ModKey> mods) => LoadOrderSetToMods = mods;
    public void OverridePrintMetadata(bool printMetadata) => PrintMetadata = printMetadata;
}
