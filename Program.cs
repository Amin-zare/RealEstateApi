using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Endpoints;
using RealEstateApi.Extensions;
using RealEstateApi.Middleware;
using RealEstateApi.Repositories;
using RealEstateApi.Services;





var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IApartmentService, ApartmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Enable CORS
app.UseCors();
app.UseMiddleware<ApiTokenMiddleware>();
app.UseHttpsRedirection();


app.MapCompanyEndpoints();
app.MapApartmentEndpoints();
app.MapWebhookEndpoints();

app.Run();
