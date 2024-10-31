using System;
using System.Threading.Tasks;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Services;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Bargeh.Users.Api.Tests;

public class SmsServiceTests
{
    private readonly SmsService _smsService;
    private readonly Mock<UsersDbContext> _dbContextMock;
    private readonly Mock<ILogger<UsersService>> _loggerMock;

    public SmsServiceTests()
    {
        _dbContextMock = new Mock<UsersDbContext>();
        _loggerMock = new Mock<ILogger<UsersService>>();
        _smsService = new SmsService(_dbContextMock.Object, _loggerMock.Object, null);
    }

    [Fact]
    public async Task SendVerification_ValidPhone_Success()
    {
        // Arrange
        var request = new SendVerificationRequest { Phone = "09123456789" };
        var callContext = new Mock<ServerCallContext>();

        // Act
        var response = await _smsService.SendVerification(request, callContext.Object);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task SendVerification_InvalidPhone_ThrowsRpcException()
    {
        // Arrange
        var request = new SendVerificationRequest { Phone = "invalid_phone" };
        var callContext = new Mock<ServerCallContext>();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _smsService.SendVerification(request, callContext.Object));
    }

    [Fact]
    public async Task ValidateVerificationCode_ValidCode_Success()
    {
        // Arrange
        var request = new ValidateVerificationCodeRequest { Phone = "09123456789", Code = "1234" };
        var callContext = new Mock<ServerCallContext>();

        // Act
        var response = await _smsService.ValidateVerificationCode(request, callContext.Object);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task ValidateVerificationCode_InvalidCode_ThrowsRpcException()
    {
        // Arrange
        var request = new ValidateVerificationCodeRequest { Phone = "09123456789", Code = "invalid_code" };
        var callContext = new Mock<ServerCallContext>();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _smsService.ValidateVerificationCode(request, callContext.Object));
    }
}
