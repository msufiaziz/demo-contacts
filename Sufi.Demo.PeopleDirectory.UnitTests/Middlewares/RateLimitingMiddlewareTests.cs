using Microsoft.AspNetCore.Http;
using Moq;
using Sufi.Demo.PeopleDirectory.UI.Server.Middlewares;
using System.Net;

namespace Sufi.Demo.PeopleDirectory.UnitTests.Middlewares
{
    public class RateLimitingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly RateLimitingMiddleware _middleware;

        public RateLimitingMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _middleware = new RateLimitingMiddleware(_nextMock.Object, maxRequests: 2, timeSpan: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task InvokeAsync_ShouldAllowRequest_WhenRateLimitNotExceeded()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _nextMock.Verify(next => next(context), Times.Once);
            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldBlockRequest_WhenRateLimitExceeded()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            // Simulate two allowed requests
            await _middleware.InvokeAsync(context);
            await _middleware.InvokeAsync(context);

            // Act - Third request should be blocked
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(429, context.Response.StatusCode); // Too Many Requests
            Assert.Contains("Rate limit exceeded", context.Response.Body.ToString());
        }

        [Fact]
        public async Task InvokeAsync_ShouldResetRateLimit_AfterTimeSpanElapsed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            // Simulate two allowed requests
            await _middleware.InvokeAsync(context);
            await _middleware.InvokeAsync(context);

            // Wait for the time span to elapse
            await Task.Delay(TimeSpan.FromSeconds(6));

            // Act - New request should be allowed
            await _middleware.InvokeAsync(context);

            // Assert
            _nextMock.Verify(next => next(context), Times.Exactly(3));
            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldHandleUnknownClientIdentifier()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = null; // Unknown client identifier

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _nextMock.Verify(next => next(context), Times.Once);
            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}
