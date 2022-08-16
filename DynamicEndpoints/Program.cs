using DynamicEndpoints.Miscellaneous;
using DynamicEndpoints.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton(ActionDescriptorChangeProvider.Instance);
builder.Services.AddHostedService<AssemblyPreloaderHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.UseEndpoints(endpoint => endpoint.MapControllerRoute("default", "{controller}"));

app.Run();
