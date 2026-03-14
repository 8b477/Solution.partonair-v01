using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace API.partonair_v01.MiddlewareCustomExceptions
{
    public class CustomExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = exception switch
            {
                InfrastructureLayerException ex => ex.ErrorType switch
                {
                    InfrastructureLayerErrorType.CancelationDatabaseException => 400,
                    InfrastructureLayerErrorType.EntityIsNullException => 400,
                    InfrastructureLayerErrorType.ResourceNotFoundException => 404,
                    InfrastructureLayerErrorType.ConcurrencyDatabaseException => 409,
                    InfrastructureLayerErrorType.CreateDatabaseException => 500,
                    InfrastructureLayerErrorType.NoActiveTransactionException => 500,
                    InfrastructureLayerErrorType.UpdateDatabaseException => 500,
                    InfrastructureLayerErrorType.UnexpectedDatabaseException => 500,
                    InfrastructureLayerErrorType.DatabaseConnectionErrorException => 503,
                    _ => 500
                },
                ApplicationLayerException ex => ex.ErrorType switch
                {
                    ApplicationLayerErrorType.EntityIsNotExistingException => 404,
                    ApplicationLayerErrorType.ConstraintViolationErrorException => 409,
                    ApplicationLayerErrorType.SaltParseBCryptException => 500,
                    ApplicationLayerErrorType.UnexpectedErrorException => 500,
                    
                    _ => 500
                },
                _ => StatusCodes.Status500InternalServerError
            };

            Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = $"https://httpstatuses.io/{httpContext.Response.StatusCode}",
                    Title = httpContext.Response.StatusCode switch
                    {
                        404 => "Resource not found",
                        503 => "Database unavailable",
                        409 => "Concurrency conflict",
                        499 => "Request canceled",
                        _ => "Internal error"
                    },
                    Detail = exception.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                    Extensions = new Dictionary<string, object?>
                    {
                        { "requestId", httpContext.TraceIdentifier },
                        { "traceId", activity?.Id }
                    }
                }
            });
        }

    }

}
