using System.Net.Sockets;
using System.Net;
using System.Text;


class UdpClientApp
{
        static async Task Main()
        {
            using (var udpClient = new UdpClient())
            {
                var serverEndpoint = new IPEndPoint(IPAddress.Loopback, 12345);

                Console.WriteLine("UDP Client is running. Type 'Exit' to stop.");

                while (true)
                {
                    Console.Write("Enter a message: ");
                    var message = Console.ReadLine();

                    var messageBytes = Encoding.UTF8.GetBytes(message);

                    await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndpoint);

                    if (message.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    // Receive confirmation from the server
                    var receiveResult = await udpClient.ReceiveAsync();
                    var confirmationMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
                    Console.WriteLine($"Server confirmation: {confirmationMessage}");
                }
            }

            Console.WriteLine("Client stopped.");
        }
    }
