using Business.Constants;
using Castle.DynamicProxy;
using Core.Extensions;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.BusinessAspects.Autofac
{
    public class SecuredOperation : MethodInterception
    {
        private readonly string[] _roles;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecuredOperation(string roles)
        {
            _roles = roles.Split(',');
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>()
                                  ?? throw new InvalidOperationException("IHttpContextAccessor çözümlenemedi.");
        }

        protected override void OnBefore(IInvocation invocation)
        {
            var roleClaims = _httpContextAccessor.HttpContext?.User?.ClaimRoles();
            if (roleClaims == null)
                throw new Exception(Messages.AuthorizationDenied);

            foreach (var role in _roles)
            {
                if (roleClaims.Contains(role))
                    return;
            }

            throw new Exception(Messages.AuthorizationDenied);
        }
    }
}
