using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class UdpServer
{
    private readonly UdpClient udpClient;
    private readonly CancellationTokenSource cts;

    public UdpServer(int port)
    {
        udpClient = new UdpClient(port);
        cts = new CancellationTokenSource();
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

                // Send confirmation back to the client
                var confirmationMessage = $"Server received: {receivedMessage}";
                var confirmationBytes = Encoding.UTF8.GetBytes(confirmationMessage);
                await udpClient.SendAsync(confirmationBytes, confirmationBytes.Length, receiveResult.RemoteEndPoint);

                if (receivedMessage.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    break;
            }
        }
        catch (ObjectDisposedException)
        {
            // Ignore the ObjectDisposedException when the UdpClient is disposed
        }

        Console.WriteLine("Server stopped.");
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
