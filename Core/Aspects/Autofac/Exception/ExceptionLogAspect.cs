using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Core.CrossCuttingConcerns.Logging;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Core.Utilities.Messages;

namespace Core.Aspects.Autofac.Exception
{
    /// <summary>
    /// ExceptionLogAspect
    /// </summary>
    public class ExceptionLogAspect : MethodInterception
    {
        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExceptionLogAspect(Type loggerService)
        {
            if (loggerService.BaseType != typeof(LoggerServiceBase))
            {
                throw new ArgumentException(AspectMessages.WrongLoggerType);
            }

            _loggerServiceBase = (LoggerServiceBase)ServiceTool.ServiceProvider.GetService(loggerService)!;
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>()!;
        }

        protected override void OnException(IInvocation invocation, System.Exception e)
        {
            var logDetailWithException = GetLogDetail(invocation);

            if (e is AggregateException agg)
            {
                logDetailWithException.ExceptionMessage = string.Join(Environment.NewLine, agg.InnerExceptions.Select(x => x.Message));
            }
            else
            {
                logDetailWithException.ExceptionMessage = e.Message;
            }

            _loggerServiceBase.Error(JsonConvert.SerializeObject(logDetailWithException));
        }

        private LogDetailWithException GetLogDetail(IInvocation invocation)
        {
            var parameters = invocation.GetConcreteMethod().GetParameters();
            var logParameters = invocation.Arguments
                .Select((arg, index) => new LogParameter
                {
                    Name = parameters[index].Name!,
                    Value = arg,
                    Type = arg?.GetType().Name ?? "object"
                })
                .ToList();

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "?";

            var logDetailWithException = new LogDetailWithException
            {
                MethodName = invocation.Method.Name,
                Parameters = logParameters,
                User = userName
            };

            return logDetailWithException;
        }
    }
}
