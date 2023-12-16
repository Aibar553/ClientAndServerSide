using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class UdpServer
{
    private readonly UdpClient udpClient;
    private readonly CancellationTokenSource cts;
    private readonly List<string> unreadMessages;

    public UdpServer(int port)
    {
        udpClient = new UdpClient(port);
        cts = new CancellationTokenSource();
        unreadMessages = new List<string>();
    }

    public async Task StartAsync()
    {
        Console.WriteLine("UDP Server is running. Press any key to stop.");

        var serverTask = StartListeningAsync(cts.Token);

        // Wait for a key to be pressed to stop the server
        Console.ReadKey();
        cts.Cancel();

        // Wait for the server task to complete
        await serverTask;
    }

    private async Task StartListeningAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for a message...");

                var receiveResult = await udpClient.ReceiveAsync();

                var receivedMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
                Console.WriteLine($"Received message: {receivedMessage} from {receiveResult.RemoteEndPoint}");

                ProcessMessage(receivedMessage, receiveResult.RemoteEndPoint);
            }
        }
        catch (ObjectDisposedException)
        {
            // Ignore the ObjectDisposedException when the UdpClient is disposed
        }

        Console.WriteLine("Server stopped.");
    }

    private void ProcessMessage(string message, IPEndPoint clientEndpoint)
    {
        if (message.Equals("Exit", StringComparison.OrdinalIgnoreCase))
        {
            // Handle client exit
            Console.WriteLine($"Client at {clientEndpoint} has disconnected.");
        }
        else if (message.Equals("List", StringComparison.OrdinalIgnoreCase))
        {
            // Handle List message type
            SendUnreadMessages(clientEndpoint);
        }
        else
        {
            // Handle regular message
            Console.WriteLine($"Received message: {message} from {clientEndpoint}");
            unreadMessages.Add(message);

            // Send confirmation back to the client
            SendConfirmation("Server received: " + message, clientEndpoint);
        }
    }

    private void SendConfirmation(string confirmationMessage, IPEndPoint clientEndpoint)
    {
        var confirmationBytes = Encoding.UTF8.GetBytes(confirmationMessage);
        udpClient.Send(confirmationBytes, confirmationBytes.Length, clientEndpoint);
    }

    private void SendUnreadMessages(IPEndPoint clientEndpoint)
    {
        if (unreadMessages.Count > 0)
        {
            foreach (var unreadMessage in unreadMessages)
            {
                SendConfirmation("Unread message: " + unreadMessage, clientEndpoint);
            }
            Console.WriteLine($"Sent {unreadMessages.Count} unread messages to {clientEndpoint}");
            unreadMessages.Clear();
        }
        else
        {
            SendConfirmation("No unread messages", clientEndpoint);
        }
    }
}

class Program
{
    static async Task Main()
    {
        var chatServer = new UdpServer(12345);
        await chatServer.StartAsync();
    }
}
