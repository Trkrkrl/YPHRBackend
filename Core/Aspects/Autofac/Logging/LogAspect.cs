using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Core.CrossCuttingConcerns.Logging;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Core.Utilities.Messages;

namespace Core.Aspects.Autofac.Logging
{
    /// <summary>
    /// LogAspect
    /// </summary>
    public class LogAspect : MethodInterception
    {
        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogAspect(Type loggerService)
        {
            if (loggerService.BaseType != typeof(LoggerServiceBase))
            {
                throw new ArgumentException(AspectMessages.WrongLoggerType);
            }

            _loggerServiceBase = (LoggerServiceBase)ServiceTool.ServiceProvider.GetService(loggerService)!;
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>()!;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _loggerServiceBase?.Info(GetLogDetail(invocation));
        }

        private string GetLogDetail(IInvocation invocation)
        {
            var logParameters = new List<LogParameter>();
            var parameters = invocation.GetConcreteMethod().GetParameters();

            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                logParameters.Add(new LogParameter
                {
                    Name = parameters[i].Name!,
                    Value = invocation.Arguments[i],
                    Type = invocation.Arguments[i]?.GetType().Name ?? "object"
                });
            }

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "?";

            var logDetail = new LogDetail
            {
                MethodName = invocation.Method.Name,
                Parameters = logParameters,
                User = userName
            };

            return JsonConvert.SerializeObject(logDetail);
        }
    }
}
