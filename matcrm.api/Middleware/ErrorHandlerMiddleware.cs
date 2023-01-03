using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.api.Middleware
{

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IErrorLogService _errorLogService;
        public ErrorHandlerMiddleware(RequestDelegate next, IErrorLogService errorLogService)
        {
            _next = next;
            _errorLogService = errorLogService;
        }

        public async Task Invoke(HttpContext context, IErrorLogService errorLogService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                errorLogService.LogException(error);

                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case AppException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    case TimeoutException e:
                        response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        break;
                    case DllNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ApplicationException e:
                        response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        break;
                    case ArgumentException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case BadHttpRequestException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;                                                                                
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var result = JsonSerializer.Serialize(new { success = false, statusCode = response.StatusCode,errorMessage = error?.Message });
                await response.WriteAsync(result);
                //return new OperationResult<Nullable> (true, System.Net.HttpStatusCode.OK,"", intProviderAppSecretListDtoList);
            }

        }
    }
}