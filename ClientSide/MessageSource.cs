using NetMQ;
using NetMQ.Sockets;

public class MessageSource
{
    private readonly PublisherSocket publisherSocket;
    private readonly PullSocket pullSocket;

    public MessageSource()
    {
        publisherSocket = new PublisherSocket();
        publisherSocket.Bind("tcp://127.0.0.1:5556");

        pullSocket = new PullSocket();
        pullSocket.Bind("tcp://127.0.0.1:5557");
    }

    public void SendMessage(string topic, string message)
    {
        publisherSocket.SendMoreFrame(topic).SendFrame(message);
    }

    public string ReceiveMessage()
    {
        return pullSocket.ReceiveFrameString();
    }
}

public class MessageSourceClient
{
    private readonly SubscriberSocket subscriberSocket;

    public MessageSourceClient()
    {
        subscriberSocket = new SubscriberSocket();
        subscriberSocket.Connect("tcp://127.0.0.1:5556");
    }

    public void Subscribe(string topic)
    {
        subscriberSocket.Subscribe(topic);
    }

    public string ReceiveMessage()
    {
        return subscriberSocket.ReceiveFrameString();
    }
}
