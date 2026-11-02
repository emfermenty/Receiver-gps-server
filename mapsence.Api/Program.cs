using Infrastructure.Mapping.Profiles;
using mapsence.Application.Services;
using mapsence.Application.Services.Extensions;
using mapsence.Application.Services.Interfaces;
using mapsence.Application.Services.JwtSettings;
using mapsence.Infrastructure.Data;
using mapsence.Infrastructure.Repositories;
using mapsencetoWebApi.Repository;
using mapsenceto.Application.Services;
using mapsence.Infrastructure.Import;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddAutoMapper(
    typeof(UserProfile).Assembly,
    typeof(GeoDataProfile).Assembly,
    typeof(SensorProfile).Assembly,
    typeof(TrackProfile).Assembly
);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(connectionString,
        o => o.UseNetTopologySuite().MigrationsHistoryTable("__EFMigrationsHistory", "zxc")
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var importTransport = builder.Configuration["Import:Transport"];
if (importTransport is not ("queue" or "socket" or "auto"))
    throw new InvalidOperationException($"Неизвестный Import:Transport: {importTransport}");

builder.Services.AddSingleton<GeoDataImportRunner>();
if (importTransport is "queue" or "auto")
    builder.Services.AddHostedService<KafkaImportConsumer>();
if (importTransport is "socket" or "auto")
    builder.Services.AddHostedService<SocketImportServer>();

builder.Services.AddScoped<IGeoDataRepository, GeoDataRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<ITrackPointInfoRepository, TrackPointInfoRepository>();
builder.Services.AddScoped<ITrackViewRepository, TrackViewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGeoDataImportRepository, GeoDataImportRepository>();

builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGeoDataService, GeoDataService>();
builder.Services.AddScoped<IGeoDataImportService, GeoDataImportService>();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.AddAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new GeometryConverter());
    });

var app = builder.Build();

// В Docker схема создаётся при старте: у каждого экземпляра mapsence своя пустая БД
if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mapsence API V1");
    });
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();