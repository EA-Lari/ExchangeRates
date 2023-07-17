using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;

namespace Crawler.Main
{
    public class ExceptionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is Exception exception)
            {
                var result = new Microsoft.AspNetCore.Mvc.ObjectResult(exception)
                {
                    StatusCode = 500,
                    Value = new {successfull = false}
                };
                result.ContentTypes.Add(MediaTypeNames.Application.Json);
                context.Result = result;
                context.ExceptionHandled = true;
                _logger.LogError($"{exception.Message}:\n{exception.StackTrace}");
            }
        }
    }
}
