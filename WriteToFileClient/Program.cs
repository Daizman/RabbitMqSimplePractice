using BrokerLib.Services;

RabbitMqMessageReceiver<Point> receiver = new("points", "rabbitmq");
receiver.AddHandler(async point =>
{
    await using StreamWriter writer = new("/home/data/points.txt", append: true);
    await writer.WriteLineAsync(point.ToString());
});

Console.WriteLine("Application is running. Press Ctrl+C to exit.");
// Use a cancellation token to handle graceful shutdown
var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => {
    e.Cancel = true; // prevent the process from terminating.
    cancellationTokenSource.Cancel();
};
try
{
    // Block here until CTRL+C or cancel is triggered
    Task.Delay(Timeout.Infinite, cancellationTokenSource.Token).Wait();
}
catch (OperationCanceledException)
{
    Console.WriteLine("Shutting down...");
}

record Point(int X, int Y);