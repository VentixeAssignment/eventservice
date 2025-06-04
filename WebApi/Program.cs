using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration["AllowedOrigins"];
var originArray = allowedOrigins?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

if (originArray == null || originArray.Length == 0)
{
    throw new Exception($"Appsettings not loaded correctly. {allowedOrigins}");
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(originArray)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:VentixeDb"]));

builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ImageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors();

app.MapGrpcService<EventServiceGrpc>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
