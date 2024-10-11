using Mutagen.Bethesda.Analyzers.Skyrim.Record.Static;
using Mutagen.Bethesda.Analyzers.Testing.Frameworks;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.Analyzers.Skyrim.Tests.IsolatedRecords.Statics;

public class MissingAssetsAnalyzerTests
{
    [Theory, MutagenModAutoData]
    public void Typical(
        AssetLink<SkyrimModelAssetType> modelPath,
        AssetLink<SkyrimModelAssetType> existingModelPath,
        IsolatedRecordTestFixture<MissingAssetsAnalyzerStatic, Static, IStaticGetter> fixture)
    {
        fixture.Run(
            prepForError: rec => rec.Model = new Model()
            {
                File = modelPath
            },
            prepForFix: rec => rec.Model = new Model()
            {
                File = existingModelPath
            },
            MissingAssetsAnalyzerStatic.MissingStaticModel);
    }
}
