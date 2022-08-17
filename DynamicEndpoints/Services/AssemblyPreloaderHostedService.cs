using DynamicEndpoints.Miscellaneous;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace DynamicEndpoints.Services;

public class AssemblyPreloaderHostedService : IHostedService
{
    private readonly ApplicationPartManager _partManager;
    private readonly ILogger<AssemblyPreloaderHostedService> _logger;
    public AssemblyPreloaderHostedService(ApplicationPartManager partManager, ILogger<AssemblyPreloaderHostedService> logger)
    {
        _partManager = partManager;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var assemblies = AssemblyProvider.GetAllGeneratedAssemblies();
        foreach (var assembly in assemblies)
        {
            _partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            _logger.LogInformation($"Loaded '{assembly.FullName} at startup");
        }

        _logger.LogInformation("Triggering update of available endpoints");
        ActionDescriptorChangeProvider.Instance.HasChanged = true;
        ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        _logger.LogInformation("Preloading assemblies done");
        return StopAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}