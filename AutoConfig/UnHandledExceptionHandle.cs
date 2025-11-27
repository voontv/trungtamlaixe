using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ttlaixe.Exceptions;
using System;
using System.Net;

namespace Ttlaixe.Handlers
{
    public class UnHandledExceptionHandle : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is { } exception)
            {
                if (!(exception is BaseException))
                {
                    context.Result = new ObjectResult(exception.Message)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };

                    var ex = exception;
                    while (ex != null)
                    {
                        Console.Error.WriteLine(context.Exception.Message);
                        Console.Error.WriteLine(context.Exception.StackTrace);
                        ex = ex.InnerException;
                    }
                }
            }
        }
    }
}