namespace Mutagen.Bethesda.Analyzers.Config.Run;

public class RunConfigProvider
{
    private readonly Lazy<IRunConfigLookup> _config;
    public IRunConfigLookup Config => _config.Value;

    public RunConfigProvider(RunConfigBuilder builder)
    {
        _config = new Lazy<IRunConfigLookup>(builder.Build);
    }
}
