using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        TcpListener server = null;

        try
        {
            int port = 8888;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("Server is running. Waiting for connections...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected!");

                NetworkStream stream = client.GetStream();

                byte[] data = new byte[256];
                int bytesRead = stream.Read(data, 0, data.Length);
                string message = Encoding.Unicode.GetString(data, 0, bytesRead);

                Console.WriteLine($"Received from client: {message}");

                // Send acknowledgment to the client
                string acknowledgment = "Message received on the server.";
                data = Encoding.Unicode.GetBytes(acknowledgment);
                stream.Write(data, 0, data.Length);

                // Wait for acknowledgment from the client
                bytesRead = stream.Read(data, 0, data.Length);
                string clientAck = Encoding.Unicode.GetString(data, 0, bytesRead);
                Console.WriteLine($"Acknowledgment from client: {clientAck}");

                // Close the connection with the client
                client.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            server.Stop();
        }
    }
}