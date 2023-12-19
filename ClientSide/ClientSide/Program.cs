using System.Net.Sockets;
using System.Net;
using System.Text;


public class UdpClientApp
{
    private readonly UdpClient udpClient;
    private readonly IPEndPoint serverEndpoint;

    public UdpClientApp(string serverIpAddress, int serverPort)
    {
        udpClient = new UdpClient();
        serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort);
    }

    public async Task StartAsync()
    {
        Console.WriteLine("UDP Client is running. Type 'Exit' to stop.");

        while (true)
        {
            Console.Write("Enter a message: ");
            var message = Console.ReadLine();

            var messageBytes = Encoding.UTF8.GetBytes(message);

            await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndpoint);

            if (message.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (message.Equals("List", StringComparison.OrdinalIgnoreCase))
            {
                // Request unread messages from the server
                await ReceiveUnreadMessagesAsync();
            }
            else
            {
                // Receive confirmation from the server
                var receiveResult = await udpClient.ReceiveAsync();
                var confirmationMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
                Console.WriteLine($"Server confirmation: {confirmationMessage}");
            }
        }

        Console.WriteLine("Client stopped.");
    }

    private async Task ReceiveUnreadMessagesAsync()
    {
        Console.WriteLine("Requesting unread messages from the server...");

        while (true)
        {
            var receiveResult = await udpClient.ReceiveAsync();

            var confirmationMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
            Console.WriteLine($"Server: {confirmationMessage}");

            if (!confirmationMessage.StartsWith("Unread message:", StringComparison.OrdinalIgnoreCase))
                break;
        }
    }
}

class Program
{
    static async Task Main()
    {
        var chatClient = new UdpClientApp("127.0.0.1", 12345);
        await chatClient.StartAsync();
    }
}
