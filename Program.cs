var builder = WebApplication.CreateBuilder(args);

// Enable Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Enable Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in production only if ENV variable ENABLE_SWAGGER = true
var enableSwagger = builder.Configuration
    .GetValue("ENABLE_SWAGGER", false);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Weather sample API
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


// --------------------------------------------------
// 📌 NEW ENDPOINT FOR APPLICATION INSIGHTS TESTING
// --------------------------------------------------
app.MapGet("/error-test", () =>
{
    throw new Exception("This is a test exception from /error-test");
})
.WithName("ErrorTest")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
