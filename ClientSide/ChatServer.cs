using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class ChatServer
{
    private readonly MessageSource messageSource;
    private readonly List<string> unreadMessages;

    public ChatServer()
    {
        messageSource = new MessageSource();
        unreadMessages = new List<string>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Server started. Press any key to stop.");

        var serverTask = Task.Run(() => ListenForMessages(cancellationToken), cancellationToken);

        Console.ReadKey();

        // No need to cancel the cancellationToken here.
        // Cancellation will be signaled by the user pressing a key.

        await serverTask;
    }

    private void ListenForMessages(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string message = messageSource.ReceiveMessage(); // Corrected method name
            // Process incoming message

            // Example: Add unread message to the list
            unreadMessages.Add(message);

            // Broadcast list of unread messages to clients
            var listMessageSource = new MessageSource();
            listMessageSource.SendMessage("List", string.Join(", ", unreadMessages));
        }
    }
}
