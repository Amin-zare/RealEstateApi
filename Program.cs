using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Endpoints;
using RealEstateApi.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapCompanyEndpoints();
app.MapApartmentEndpoints();
app.MapWebhookEndpoints();
app.UseHttpsRedirection();

app.Run();
