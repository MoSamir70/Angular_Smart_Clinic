//using AutoMapper;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using SmartClinic.Application.Interfaces;
//using SmartClinic.Application.Interfaces.Services;
//using SmartClinic.Application.Mapping;
//using SmartClinic.Infrastructure;
//using SmartClinic.Infrastructure.Data;
//using SmartClinic.Infrastructure.Services;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "Smart Clinic API", Version = "v1" });
//});

//builder.Services.AddAutoMapper(typeof(MappingProfile));
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SmartClinic.Application.Interfaces.IUnitOfWork).Assembly));

//var dbPath = Path.Combine(Environment.CurrentDirectory, "smartclinic.db");
//builder.Services.AddDbContext<SmartClinicDbContext>(options =>
//    options.UseSqlite($"Data Source={dbPath}"));

//try
//{
//    var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
//    builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp =>
//        StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnection + ",abortConnect=false"));
//    builder.Services.AddSignalR().AddStackExchangeRedis(redisConnection, options =>
//    {
//        options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("SmartClinic");
//    });
//}
//catch
//{
//    builder.Services.AddSignalR();
//}

//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IRedisService, RedisService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors();

//app.UseAuthorization();

//app.MapControllers();
//app.MapHub<SmartClinic.API.Hubs.QueueHub>("/hubs/queue");

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<SmartClinicDbContext>();
//    context.Database.EnsureCreated();
//}

//app.Run();










//-----------------------------------------------------------

//using AutoMapper;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using SmartClinic.Application.Interfaces;
//using SmartClinic.Application.Interfaces.Services;
//using SmartClinic.Application.Mapping;
//using SmartClinic.Infrastructure;
//using SmartClinic.Infrastructure.Data;
//using SmartClinic.Infrastructure.Services;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "Smart Clinic API", Version = "v1" });
//});

//builder.Services.AddAutoMapper(typeof(MappingProfile));
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SmartClinic.Application.Interfaces.IUnitOfWork).Assembly));

//var dbPath = Path.Combine(Environment.CurrentDirectory, "smartclinic.db");
//builder.Services.AddDbContext<SmartClinicDbContext>(options =>
//    options.UseSqlite($"Data Source={dbPath}"));

//// ==================== SignalR Configuration (مُحسّن) ====================
//// ==================== SignalR Configuration - Development Friendly ====================

//أهم سطر: نستخدم In-Memory فقط (بدون Redis نهائياً في التطوير)
//builder.Services.AddSignalR(options =>
//{
//    options.EnableDetailedErrors = true;   // مفيد للـ debugging
//});

//// لو عايز Redis في المستقبل (Production فقط)
//if (!builder.Environment.IsDevelopment())
//{
//    var redisConnection = builder.Configuration.GetConnectionString("Redis");
//    if (!string.IsNullOrEmpty(redisConnection))
//    {
//        builder.Services.AddSignalR()
//            //.AddStackExchangeRedis(redisConnection, options =>
//            //{
//            //    options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("SmartClinic");
//            //    options.Configuration.AbortOnConnectFail = false;
//            //});

//        Console.WriteLine("✅ Production Mode: SignalR with Redis Backplane");
//    }
//}
//else
//{
//    Console.WriteLine("✅ Development Mode: SignalR running with In-Memory (Redis completely disabled)");
//}

//// =================================================================================

//// =====================================================================

//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IRedisService, RedisService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();

//// CORS Configuration
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        if (builder.Environment.IsDevelopment())
//        {
//            // للتطوير - الـ Angular على port 4200
//            policy.WithOrigins("http://localhost:4200")
//                  .AllowAnyMethod()
//                  .AllowAnyHeader()
//                  .AllowCredentials();
//        }
//        else
//        {
//            // في الإنتاج غير ده للدومين بتاعك
//            policy.AllowAnyOrigin()
//                  .AllowAnyMethod()
//                  .AllowAnyHeader();
//        }
//    });
//});

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors();
//app.UseAuthorization();

//app.MapControllers();
//app.MapHub<SmartClinic.API.Hubs.QueueHub>("/hubs/queue");

//// Ensure Database is created
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<SmartClinicDbContext>();
//    context.Database.EnsureCreated();
//}

//app.Run();


//-----------------------------------

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartClinic.Application.Interfaces;
using SmartClinic.Application.Interfaces.Services;
using SmartClinic.Application.Mapping;
using SmartClinic.Infrastructure;
using SmartClinic.Infrastructure.Data;
using SmartClinic.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Smart Clinic API", Version = "v1" });
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SmartClinic.Application.Interfaces.IUnitOfWork).Assembly));

var dbPath = Path.Combine(Environment.CurrentDirectory, "smartclinic.db");
builder.Services.AddDbContext<SmartClinicDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// ==================== SignalR Configuration (In-Memory Only) ====================
// ==================== SignalR Configuration (In-Memory Only - No Redis) ====================

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("✅ Development Mode: SignalR running with In-Memory (Redis completely disabled)");
}
else
{
    Console.WriteLine("✅ Production Mode: SignalR configured");
}

// =====================================================================


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SmartClinic.API.Hubs.QueueHub>("/hubs/queue");

// Ensure Database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SmartClinicDbContext>();
    context.Database.EnsureCreated();
}

app.Run();