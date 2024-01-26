using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace WebApplication.Core.Common.Behaviours
{
    public class CommandLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<CommandLoggingBehaviour<TRequest, TResponse>> _logger;

        public CommandLoggingBehaviour(ILogger<CommandLoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestName = request?.GetType().Name;

            _logger.LogInformation($"[START] {requestName}");
            TResponse response;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                response = await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    $"[END] {requestName}; Execution time={stopwatch.ElapsedMilliseconds}ms");
            }

            return response;
        }
    }
}
