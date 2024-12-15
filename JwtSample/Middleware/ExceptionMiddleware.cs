using JwtSample.Exceptions;
using JwtSample.Services.JsonService;
using Serilog;
using Serilog.Context;
using System.Net;

namespace JwtSample.Middleware;

internal class ExceptionMiddleware : IMiddleware
{
    private const string ResponseTemplate = "Response({correlationId}): {{Response Body: {Message}}}";
    private const string DefaultMessage = "Internal Server Error.";

    private readonly ISerializerService _jsonSerializer;
    private readonly string _correlationId;
    public ExceptionMiddleware(ISerializerService jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
        _correlationId = Guid.NewGuid().GetHashCode().ToString("X8");
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            using (LogContext.PushProperty("CorrelationId", _correlationId))
            {
                await next.Invoke(context);
            }
        }
        catch (Exception exception)
        {
            var baseException = exception.GetBaseException();

            var (message, statusCode) = baseException switch
            {
                JwtSampleException e when true => (e.Message, e.StatusCode),
                _ => (DefaultMessage, HttpStatusCode.InternalServerError)
            };

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                Log.Error(ResponseTemplate, _correlationId, baseException.Message);
                await context.Response.WriteAsync(_jsonSerializer.Serialize(new ErrorResult() { Message = message }));
            }
            else
            {
                Log.Warning("Response({correlationId}): Can't write error response. Response has already started.", _correlationId);
            }
        }
    }
}
