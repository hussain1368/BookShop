using Shop.Library.API.Domain;
using Shop.Library.API.Endpoints;
using Shop.Library.API.Features;
using Shop.Shared.Middlewares;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MassTransit;
using Shop.Library.API.Features.Books;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, lo => lo.Protocols = HttpProtocols.Http1);
    options.ListenAnyIP(5002, lo => lo.Protocols = HttpProtocols.Http2);
});

var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var services = builder.Services;

services.AddGrpc();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext<BookContext>(options => options.UseSqlServer(config.GetConnectionString("Default")));
services.AddMediatR(conf => conf.RegisterServicesFromAssembly(typeof(Book).Assembly));
services.AddAutoMapper(typeof(Book).Assembly);
services.AddValidatorsFromAssembly(typeof(Book).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

services.AddMassTransit(bus =>
{
    bus.SetKebabCaseEndpointNameFormatter();
    bus.AddConsumer<PurchaseAddedConsumer>();
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

app.MapGrpcService<BookService>();

app.UsePathBase("/api/v1");
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBookEndpoints();
app.Run();
