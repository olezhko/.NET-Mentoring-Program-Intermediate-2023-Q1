using CatalogService.Data;
using CatalogService.Services.Categories;
using CatalogService.Services.Products;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mainConnectionString = builder.Configuration.GetConnectionString("Main");
builder.Services.AddDbContext<CatalogServiceContext>(options => options.UseSqlServer(mainConnectionString));

builder.Services.AddTransient<ICategoriesService, CategoriesService>();
builder.Services.AddTransient<IProductsService, ProductsService>();

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
