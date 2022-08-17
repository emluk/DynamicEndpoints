using DynamicEndpoints.Miscellaneous;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace DynamicEndpoints.Controllers;

[ApiController]
[Route("[controller]")]
public class EndpointGeneratorController : ControllerBase
{
    
    private readonly ILogger<EndpointGeneratorController> _logger;
    private readonly ApplicationPartManager _partManager;
    public EndpointGeneratorController(ILogger<EndpointGeneratorController> logger, ApplicationPartManager partManager)
    {
        _logger = logger;
        _partManager = partManager;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] string name)
    {
        var assembly = AssemblyProvider.CreateOrGetAssembly(name, out var diagnostics);
        
        // add assembly to application parts (this adds a new controller in our case)
        if (assembly == null)
        {
            var errors = string.Join("\n", diagnostics.Select(diagnostic => diagnostic.ToString() ));
            _logger.LogError($"Compiling new endpoint '{name}' failed with diagnostics:\n{errors}");
            return BadRequest(errors);
        }
        _partManager.ApplicationParts.Add(new AssemblyPart(assembly));

        // notify ASP net of the changes
        ActionDescriptorChangeProvider.Instance.HasChanged = true;
        ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        _logger.LogInformation($"Added new endpoint {name}");
        return Ok($"Created {name}");
    }
}