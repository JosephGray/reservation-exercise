using Microsoft.OpenApi.Models;
using Reservation.Api.Services;
using Reservation.Infrastructure.InMemory;
using Reservation.Infrastructure.Repository;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reservations API - V1", Version = "v1" });

    string filePath = Path.Combine(AppContext.BaseDirectory, "Reservation.Api.xml");
    c.IncludeXmlComments(filePath);


});
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IProviderRepository, ProviderRepository>();
builder.Services.AddSingleton<IFreeSlotRepository, FreeSlotRepository>();
builder.Services.AddHostedService<ReservationCancellerService>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<Reservation.Api.Example.TimeSlotExampleProvider>();

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
