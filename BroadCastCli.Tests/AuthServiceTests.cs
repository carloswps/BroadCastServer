using System.Net;
using BroadcastServer.Services;
using Moq;
using Moq.Protected;

namespace BroadCastCli.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        var handleMock = new Mock<HttpMessageHandler>();
        handleMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized });

        var httpClient = new HttpClient(handleMock.Object);
        var authService = new AuthService("http://localhost", httpClient);

        var result = await authService.LoginAsync("invalid", "credentials");
        Assert.Null(result);
    }
}