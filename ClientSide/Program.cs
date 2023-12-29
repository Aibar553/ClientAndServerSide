using System;
using System.Threading;
using System.Threading.Tasks;

public class ChatClient
{
    private readonly MessageSourceClient messageSourceClient;

    public ChatClient()
    {
        messageSourceClient = new MessageSourceClient();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Client started. Type 'Exit' to quit.");

        messageSourceClient.Subscribe("Chat");
        messageSourceClient.Subscribe("List");

        var cts = new CancellationTokenSource();
        var clientTask = Task.Run(() => ListenForMessages(cts.Token), cts.Token);

        while (!cancellationToken.IsCancellationRequested)
        {
            string input = Console.ReadLine();
            if (input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
            {
                cts.Cancel();  // Cancel the CancellationTokenSource
            }
            else
            {
                // Send message to the server
                var messageSource = new MessageSource();
                messageSource.SendMessage("Chat", input);
            }
        }

        await clientTask;
    }

    private void ListenForMessages(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string topic = messageSourceClient.ReceiveMessage();
            string message = messageSourceClient.ReceiveMessage();

            if (topic == "List")
            {
                Console.WriteLine($"Unread messages: {message}");
            }
            else
            {
                Console.WriteLine($"Server: {message}");
            }
        }
    }
}
class Program
{
    static async Task Main(string[] args)
    {
        using (var cts = new CancellationTokenSource())
        {
            var server = new ChatServer();
            var client = new ChatClient();

            var serverTask = server.StartAsync(cts.Token);
            var clientTask = client.StartAsync(cts.Token);

            await Task.WhenAll(serverTask, clientTask);
        }
    }
}