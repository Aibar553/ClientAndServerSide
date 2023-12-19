using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit.Sdk;

[TestClass]
public class UdpClientAppTests
{
    [TestMethod]
    public async Task StartAsync_ExitCommand_ShouldStopClient()
    {
        // Arrange
        var udpClientMock = new Mock<UdpClient>();
        var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);

        var chatClient = new UdpClientAppWrapper(udpClientMock.Object, serverEndpoint);

        // Act
        await chatClient.StartAsync();

        // Assert
        udpClientMock.Verify(uc => uc.SendAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()), Times.Once);
        udpClientMock.Verify(uc => uc.ReceiveAsync(), Times.Never); // No need to receive in this test
    }

    [TestMethod]
    public async Task StartAsync_ListCommand_ShouldReceiveUnreadMessages()
    {
        // Arrange
        var udpClientMock = new Mock<UdpClient>();
        var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);

        udpClientMock.SetupSequence(uc => uc.ReceiveAsync())
            .ReturnsAsync(new UdpReceiveResult(Encoding.UTF8.GetBytes("Unread message: Test"), serverEndpoint))
            .ReturnsAsync(new UdpReceiveResult(Encoding.UTF8.GetBytes("No unread messages"), serverEndpoint));

        var chatClient = new UdpClientAppWrapper(udpClientMock.Object, serverEndpoint);

        // Act
        await chatClient.StartAsync();

        // Assert
        udpClientMock.Verify(uc => uc.SendAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()), Times.Exactly(2));
        udpClientMock.Verify(uc => uc.ReceiveAsync(), Times.Exactly(2));
    }
}

public class UdpClientAppWrapper : UdpClientApp
{
    private readonly UdpClient udpClient;

    public UdpClientAppWrapper(UdpClient udpClient, IPEndPoint serverEndpoint)
        : base(serverEndpoint.Address.ToString(), serverEndpoint.Port)
    {
        this.udpClient = udpClient;
    }
}