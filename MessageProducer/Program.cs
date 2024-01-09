using BrokerLib.Abstract;
using BrokerLib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RabbitMqMessagePublisher<Point> pub = new("points", "rabbitmq");
builder.Services.AddSingleton<IMessagePublisher<Point>>(pub);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/point", async (Point point, IMessagePublisher<Point> publisher) =>
{
    await publisher.PublishAsync(point);
});

app.Run();

public record Point(int X, int Y);
