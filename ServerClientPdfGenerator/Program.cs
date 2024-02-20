using System.Reflection;
using Hangfire;
using Hangfire.MemoryStorage;
using ServerClientPdfGenerator.Hubs;
using ServerClientPdfGenerator.Services;
using ServerClientPdfGenerator.UIServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<IStorageProvider, BlobStorageProvider>();

builder.Services.AddRazorPages();
builder.Services.AddHangfireServer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();
app.MapHub<ReportsHub>("/hub");
app.Run();
