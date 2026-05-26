using CrossCuttingConcerns.Logging;
using CrossCuttingConcerns.Serilog;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace Application.Pipelines.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggableRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;

    public LoggingBehavior(IHttpContextAccessor httpContextAccessor, LoggerServiceBase loggerServiceBase)
    {
        _httpContextAccessor = httpContextAccessor;
        _loggerServiceBase = loggerServiceBase;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        List<LogParameter> logParameters = new List<LogParameter>()
        {
            new LogParameter
            {
                Name = typeof(TRequest).Name,
                Type = typeof(TRequest).Name,
                Value = request
            }
        };

        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User?.Identity?.Name;
        var requestPath = httpContext?.Request?.Path.Value ?? string.Empty;
        var requestMethod = httpContext?.Request?.Method ?? string.Empty;

        LogDetail logDetail = new LogDetail
        {
            Fullname = $"{typeof(TRequest).Namespace}.{typeof(TRequest).Name}",
            MethodName = $"[{requestMethod}] {requestPath} {typeof(TRequest).Name}",
            Parameters = logParameters,
            User = string.IsNullOrWhiteSpace(user) ? "Anonymous" : user
        };

        try
        {
            _loggerServiceBase.Info($"[START] {logDetail.Fullname} initiated by user: {logDetail.User}");

            TResponse response = await next();

            stopwatch.Stop();
            logDetail.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            _loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));
            return response;
        }
        catch (Exception)
        {
            stopwatch.Stop();
            logDetail.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            _loggerServiceBase.Error(JsonSerializer.Serialize(logDetail));
            throw;
        }
    }
}
