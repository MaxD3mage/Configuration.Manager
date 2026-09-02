using Microsoft.EntityFrameworkCore;
using Configuration.Manager.BusinessLogic.App.Services;
using Configuration.Manager.BusinessLogic.Core.Interfaces;
using Configuration.Manager.BusinessLogic.Repository.Data;
using Configuration.Manager.BusinessLogic.Repository.Repositories;
using Configuration.Manager.Web.Hubs;
using Configuration.Manager.Web.Middleware;
using Configuration.Manager.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddSignalR();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
app.MapHub<ConfigurationHub>("/hub/configurations");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();