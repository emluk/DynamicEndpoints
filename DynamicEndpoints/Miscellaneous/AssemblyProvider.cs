using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace DynamicEndpoints.Miscellaneous;

public class AssemblyProvider
{
    public static Assembly CreateOrGetAssembly(string name, out List<Diagnostic> diagnostic, string templatePath = "Templates/default.txt")
    {
        diagnostic = new List<Diagnostic>();
        var assemblyName = $"{name}.dll";
        var outputDll = Path.Join("GeneratedEndpointAssemblies", assemblyName);
        if (File.Exists(outputDll)) // if the assembly already exists, just load it.
        {
            return Assembly.LoadFrom(outputDll);
        }

        var code = GenerateCode(name, templatePath);
        if (string.IsNullOrEmpty(code))
        {
            return null;
        }

        // get assembly references
        var references = new List<PortableExecutableReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
            .ToList()
            .ForEach(a =>
            {
                var reference = MetadataReference.CreateFromFile(Assembly.Load(a).Location);
                references.Add(reference);
            });
        
        // parse code from edited template
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        // IMPORTANT: Do not insert a path into Create(). It will fail and complain  with CS8203 about illegal characters in assembly name.
        var compilation = CSharpCompilation.Create(assemblyName,
            new[]
            {
                CSharpSyntaxTree.ParseText(code, parseOptions)
            }, 
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // compile
        var emitResult = compilation.Emit(outputDll);
        diagnostic = emitResult.Diagnostics.ToList();

        if (emitResult.Success) return Assembly.LoadFrom(outputDll);
        
        File.Delete(outputDll);
        return null;
    }

    private static string GenerateCode(string name, string templatePath)
    {
        string content;
        // read template
        using (var reader = new StreamReader(templatePath))
        {
            content = reader.ReadToEnd();
        }

        // replace placeholders in template
        var code = content.Replace("{PLACEHOLDER}", name);
        return code;
    }
}