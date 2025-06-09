using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shop.Sales.API.Domain;
using Shop.Sales.API.Endpoints;
using Shop.Sales.API.Features.Purchases;
using Shop.Shared;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddGrpcClient<BookProtocol.BookProtocolClient>(client =>
{
    client.Address = new Uri("http://localhost:5002");
});

services.AddDbContext<PurchaseContext>(options => options.UseSqlServer(config.GetConnectionString("Default")));
services.AddMediatR(conf => conf.RegisterServicesFromAssembly(typeof(Purchase).Assembly));
services.AddAutoMapper(typeof(Purchase).Assembly);

services.AddMassTransit(bus =>
{
    bus.SetKebabCaseEndpointNameFormatter();
    bus.AddConsumer<BookUpdatedConsumer>();
    bus.UsingRabbitMq((context, conf) =>
    {
        conf.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
        conf.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPurchaseEndpoints();
app.Run();
