using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Practice01.Application;
using Practice01.Presentation;
using Practice01.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPresentation();
builder.Services.AddApplication();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();