using System.Text.RegularExpressions;
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
        //"([a-Z]|[0-9])+";
        Regex regex = new Regex(@"^([a-z]|[A-Z]|[0-9])+$");
        if (!regex.IsMatch(name))
        {
            return BadRequest(
                $"{name} is not an allowed name for the endpoint. It can only have alphanumerical characters");
        }
        
        var assembly = AssemblyProvider.CreateOrGetAssembly(name, out var diagnostics);
        
        // add assembly to application parts (this adds a new controller in our case)
        if (assembly == null)
        {
            var errors = string.Join("\n", diagnostics.Select(diagnostic => diagnostic.ToString() ));
            return BadRequest(errors);
        }
        _partManager.ApplicationParts.Add(new AssemblyPart(assembly));

        // notify ASP net of the changes
        ActionDescriptorChangeProvider.Instance.HasChanged = true;
        ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        
        return Ok($"Created {name}");
    }
}