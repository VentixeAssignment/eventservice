using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:LocalDb"]));

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
app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.MapGrpcService<EventServiceGrpc>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
