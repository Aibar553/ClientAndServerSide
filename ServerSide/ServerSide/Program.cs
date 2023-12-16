using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class UdpServer
{
    static async Task Main()
    {
        var cts = new CancellationTokenSource();

        Console.WriteLine("UDP Server is running. Press any key to stop.");

        var serverTask = StartServerAsync(cts.Token);

        // Wait for a key to be pressed to stop the server
        Console.ReadKey();
        cts.Cancel();

        // Wait for the server task to complete
        await serverTask;
    }

    static async Task StartServerAsync(CancellationToken cancellationToken)
    {
        using (var udpClient = new UdpClient(12345))
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
        }

        Console.WriteLine("Server stopped.");
    }
}