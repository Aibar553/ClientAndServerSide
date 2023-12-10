using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class UdpServer
{
    private UdpClient udpServer;
    private Thread receiveThread;

    public UdpServer()
    {
        udpServer = new UdpClient(12345); // You can change the port number as needed
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            while (true)
            {
                byte[] receiveBytes = udpServer.Receive(ref remoteIpEndPoint);
                string receivedMessage = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine($"Received from {remoteIpEndPoint}: {receivedMessage}");

                if (receivedMessage.ToLower() == "exit")
                {
                    Console.WriteLine("Client requested exit. Closing server...");
                    break;
                }

                // Process the message if needed

                // Send confirmation back to the client
                byte[] sendBytes = Encoding.ASCII.GetBytes($"Received: {receivedMessage}");
                udpServer.Send(sendBytes, sendBytes.Length, remoteIpEndPoint);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
        }
        finally
        {
            udpServer.Close();
        }
    }

    public void StopServer()
    {
        udpServer.Close();
        receiveThread.Abort();
    }
}
class UdpClientApp
{
    static void Main()
    {
        UdpServer udpServer = new UdpServer();

        try
        {
            using (System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient())
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345); // Set the server IP and port
                Console.WriteLine("Enter 'Exit' to close the client.");

                while (true)
                {
                    Console.Write("Enter message: ");
                    string message = Console.ReadLine();

                    byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                    udpClient.Send(sendBytes, sendBytes.Length, serverEndPoint);

                    byte[] receiveBytes = udpClient.Receive(ref serverEndPoint);
                    string serverResponse = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine($"Server response: {serverResponse}");

                    if (message.ToLower() == "exit")
                    {
                        Console.WriteLine("Closing client...");
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
        }
        finally
        {
            udpServer.StopServer();
        }
    }
}