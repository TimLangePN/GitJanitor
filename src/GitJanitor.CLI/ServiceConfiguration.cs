using GitJanitor.Core;
using GitJanitor.Core.Interfaces;
using GitJanitor.IO;
using GitJanitor.IO.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitJanitor.CLI;

public class ServiceConfiguration
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddTransient<IGitRepositoryScanner, GitRepositoryScanner>();
        services.AddTransient<IGitRepositoryHandler, GitRepositoryHandler>();

        return services.BuildServiceProvider();
    }
}