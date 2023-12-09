using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        TcpClient client = null;

        try
        {
            int port = 8888;
            string serverIP = "127.0.0.1";

            client = new TcpClient(serverIP, port);
            Console.WriteLine("Connected to the server.");

            NetworkStream stream = client.GetStream();

            // Send message to the server
            Console.Write("Enter a message to send: ");
            string message = Console.ReadLine();
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine($"Sent to server: {message}");

            // Wait for acknowledgment from the server
            data = new byte[256];
            int bytesRead = stream.Read(data, 0, data.Length);
            string serverAck = Encoding.Unicode.GetString(data, 0, bytesRead);
            Console.WriteLine($"Acknowledgment from server: {serverAck}");

            // Send acknowledgment to the server
            string acknowledgment = "Acknowledgment from client.";
            data = Encoding.Unicode.GetBytes(acknowledgment);
            stream.Write(data, 0, data.Length);

            // Close the connection with the server
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
