using LoggingService.RabbitMQ.Implementation;
using LoggingService.Services;
using Serilog;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog first
builder.Host.UseSerilog((context, services, configuration) => configuration
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("LogConnectionString"),
        sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = false }
    )
    .Filter.ByIncludingOnly(logEvent =>
        logEvent.Properties.TryGetValue("SourceContext", out var sourceContext) &&
        sourceContext.ToString().Contains("CustomerService")
    )
);

// Enable Serilog self-logging for debugging
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services with proper logger injection
builder.Services.AddSingleton<ILoggingService, LoggingService.Services.Implementation.LoggingService>();
builder.Services.AddHostedService<RabbitMqConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
